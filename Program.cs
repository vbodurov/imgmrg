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
                var i = 0;
                do
                {
                    Console.WriteLine((++i)+" ERROR: "+ex);
                    ex = ex.InnerException;
                } while (ex != null);
                Console.ForegroundColor = color;
            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
