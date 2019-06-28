using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICMatrix.Data;
using ICMatrix.Matrix;
using System.Diagnostics;

namespace ICMatrix
{
    class Program
    {
        private static ICProviderService provider;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to InvestCloud Matrix Generator");
            Console.WriteLine("Initialzing........................................................................");
            provider = new ICProviderService();
            if (provider.Configured)
            {
                Console.WriteLine("Enter matrix size 2-1000 and hit enter to run the matrix generator \r\nType quit - To Exit\r\n");
                RunMatrixGenerator();
            }
            else
            {
                Console.WriteLine("Matrix DataSet Provider Configuration failure... Matrix Generator cannot be run now :-(");
            }
            Console.WriteLine("____________________________________________________________________");

            Console.WriteLine("Bye");

            Console.ReadKey();
        }

        private static bool Init()
        {
            return true;
        }
        private static void RunMatrixGenerator()
        {
            var input = Console.ReadLine();
            while (input != "quit")
            {
                if (int.TryParse(input, out int size))
                {
                    if(size > 2 && size <= 1000)
                    {
                        Console.WriteLine($"Started generating {size} X {size} matrix");
                        var myMatrix = new CloudMatrix(provider, size);
                        var progTimer = Stopwatch.StartNew();
                        if (myMatrix.Init())
                        {
                            var timer = Stopwatch.StartNew();
                            myMatrix.FillDataSets();
                            timer.Stop();
                            Console.WriteLine($"DataSet Download time: { timer.ElapsedMilliseconds} ms");

                            timer = Stopwatch.StartNew();
                            myMatrix.ValidateMatrixProduct();
                            timer.Stop();
                            Console.WriteLine($"Matrix Multiplication and Serialization time: {timer.ElapsedMilliseconds} ms");
                        }
                        progTimer.Stop();
                        Console.WriteLine($"Total run time: {progTimer.ElapsedMilliseconds} ms");

                        Console.WriteLine("InvestCloud Matrix Completed...................");
                        Console.WriteLine("_________________________________________________________________________________________");
                    }
                  
                    Console.WriteLine("Enter Matrix size 2-1000 and hit enter or quit");
                }
                input = Console.ReadLine();
            }
        }
    }
}
