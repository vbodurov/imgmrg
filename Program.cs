using System;
using imgmrg.Services;

namespace imgmrg
{
    class Program
    {
        static void Main(string[] args)
        {
            IRunner runner = new Runner();
            try
            {
                runner.Execute();
            }
            catch (Exception ex)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: "+ex);
                Console.ForegroundColor = color;
            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
