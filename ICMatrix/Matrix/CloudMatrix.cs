using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ICMatrix.Data;

namespace ICMatrix.Matrix
{
    public class CloudMatrix
    {
        private int matrixSize;
        private ICProviderService provider;
        private bool init;
        private int[][] matrixA;
        private int[][] matrixB;
        public CloudMatrix(ICProviderService provider, int size)
        {
            this.provider = provider;
            this.matrixSize = size;
        }

        public bool Init()
        {
            var ret = this.provider.InitDataSet(this.matrixSize);
            if (ret.Result.Success)
            {
                this.init = true;
                matrixA = new int[matrixSize][];
                matrixB = new int[matrixSize][];
            }
            return this.init;
        }
        public void FillDataSets()
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < matrixSize; i++)
            {
                // make requests to row and column data
                var aRow = provider.GetDataSetAsync("A", "row", i, true);
                taskList.Add(aRow);
                var bCol = provider.GetDataSetAsync("B", "col", i, false);
                taskList.Add(bCol);
             }
            Task.WaitAll(taskList.ToArray());
             foreach (Task<Tuple<DataResponse, int, bool>> t in taskList)
            {
                if (t.Result.Item3)
                {
                    matrixA[t.Result.Item2] = t.Result.Item1.Value;
                }
                else
                {
                    matrixB[t.Result.Item2] = t.Result.Item1.Value;
                }
            }
        }

        public void FillDataSetsParallel()
        {
            var result = Parallel.For(0, matrixSize, (i, state) =>
            {
                // Console.WriteLine($"Thread {i} Started");
                var aRow = provider.GetDataSet("A", "row", i);
                var bCol = provider.GetDataSet("B", "col", i);
                matrixA[i] = aRow.Value;
                matrixB[i] = bCol.Value;
                // Console.WriteLine($"Thread {i} Finished");
            });
           
        }

        public void ValidateMatrixProduct()
        {
           string[] resultMatrix = new string[matrixSize * matrixSize];
            Parallel.For(0, matrixSize, i =>
            {
                Parallel.For(0, matrixSize, j =>
                {
                    resultMatrix[matrixSize * i + j] = Multiply(matrixA[i], matrixB[j], matrixSize);
                });
            });
            var bigString = String.Join(string.Empty, resultMatrix);

            string bigHash = MD5HashIt(bigString);
            Console.WriteLine($"MD5-Hash: {bigHash}");
            var ret = this.provider.ValidateDataSet(bigHash);
            Console.WriteLine($"result: {ret.Value}");

        }
        public static string Multiply(int[] row, int[] col, int size)
        {
            int sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += row[i] * col[i];
            }

            return sum.ToString();
        }

        public static string MD5HashIt(string bigString)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(bigString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to decimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString());
                }
                return sb.ToString();
            }
        }

    }
}
