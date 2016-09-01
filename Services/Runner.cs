using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using imgmrg.Config;
using imgmrg.Models;
using Newtonsoft.Json;

namespace imgmrg.Services
{
    public interface IRunner
    {
        void Execute();
    }
    internal sealed class Runner : IRunner
    {
        void IRunner.Execute()
        {
            var sw = Stopwatch.StartNew();
            ConfigData config = null;
            try
            {
                config = GetConfig();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving configuration: "+ex.Message, ex);
            }

            foreach (var t in config.transformations)
            {
                if(!File.Exists(t.input_rgb)) throw new ApplicationException("Cannot find RGB file '"+t.input_rgb+"'");
                if(!File.Exists(t.input_a)) throw new ApplicationException("Cannot find A file '"+t.input_a+"'");
                if(string.IsNullOrEmpty(t.output)) throw new ApplicationException("No output file specified for '"+t.input_a+"'");
            }

            foreach (var t in config.transformations)
            {
                try
                {
                    var originalRgb = Image.FromFile(t.input_rgb);
                    var originalA = Image.FromFile(t.input_a);
                    var output = Merge(originalRgb, originalA);
                    if (File.Exists(t.output))
                    {
                        File.Delete(t.output);
                    }
                    var outDir = Path.GetDirectoryName(t.output);
                    if (outDir!= null && !Directory.Exists(outDir))
                    {
                        Directory.CreateDirectory(outDir);
                    }
                    output.Save(t.output);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error creating file from  RGB:"+t.input_rgb+" A:"+t.input_a, ex);
                }
                
                Console.WriteLine("Output:"+t.output);
            }

            sw.Stop();

            
            Console.WriteLine("Execution time:"+sw.Elapsed);
        }

        private static ConfigData GetConfig()
        {
            var jcs = ConfigurationManager.GetSection("jsonData") as JsonConfigSection;
            if (jcs == null)
            {
                throw new Exception(".config is missing '/configuration/jsonData/add[name=imgmrg]' section");
            }
            var jsonString = jcs.JsonFiles["imgmrg"];
            return JsonConvert.DeserializeObject<ConfigData>(jsonString);
        }

        private static Image Merge(Image originalImageRGB, Image originalImageA)
        {
            if(originalImageRGB.Width != originalImageA.Width)
                throw new ArgumentException("Width of both images must be the same");
            if(originalImageRGB.Height != originalImageA.Height)
                throw new ArgumentException("Height of both images must be the same");

            Bitmap newImage = new Bitmap(originalImageRGB);
            BitmapData originalDataRGB = (originalImageRGB as Bitmap).LockBits(new Rectangle(0, 0, originalImageRGB.Width, originalImageRGB.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData originalDataA = (originalImageA as Bitmap).LockBits(new Rectangle(0, 0, originalImageA.Width, originalImageA.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData newData = (newImage as Bitmap).LockBits(new Rectangle(0, 0, originalImageRGB.Width, originalImageRGB.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int originalStride = originalDataRGB.Stride;
            System.IntPtr originalScanRGB0 = originalDataRGB.Scan0;
            System.IntPtr originalScanA0 = originalDataA.Scan0;

            System.IntPtr newScan0 = newData.Scan0;

            unsafe
            {
                byte* pOriginalRGB = (byte*)(void*)originalScanRGB0;
                byte* pOriginalA = (byte*)(void*)originalScanA0;
                byte* pNew = (byte*)(void*)newScan0;

                int nOffset = originalStride - originalImageRGB.Width * 4;

//                byte red, green, blue, alpha;
                byte red, alpha;

                for (int y = 0; y < originalImageRGB.Height; ++y)
                {
                    for (int x = 0; x < originalImageRGB.Width; ++x)
                    {
//                        blue = pOriginalRGB[0];
//                        green = pOriginalRGB[1];
                        red = pOriginalRGB[2];
                        alpha = pOriginalA[0];

                        pNew[0] = red;// BLUE 
                        pNew[1] = red;// GREEN
                        pNew[2] = red;// RED
                        pNew[3] = alpha;// ALPHA


                        pOriginalRGB += 4;
                        pOriginalA += 4;
                        pNew += 4;
                    }
                    pOriginalRGB += nOffset;
                    pOriginalA += nOffset;
                    pNew += nOffset;
                }
            }
            (originalImageRGB as Bitmap).UnlockBits(originalDataRGB);
            (originalImageA as Bitmap).UnlockBits(originalDataA);
            (newImage as Bitmap).UnlockBits(newData);
            return newImage;
        }
    }
}