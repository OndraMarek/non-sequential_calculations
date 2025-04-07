class AcceleratedCascadingMaximumProblem
{
    static void Main()
    {
        int[] A = GenerateNumbers(1024);
        int[] B = ParallelTreeMax(A);
        int S = ComputeAcceleratedCascadingMaximumProblem(B);

        PrintOutput(A, S);
        PrintOutputToFile(A, S);

        ExitOrContinue();
    }

    public static int[] GenerateNumbers(int n)
    {
        Random rand = new();
        HashSet<int> uniqueNumbers = [];

        while (uniqueNumbers.Count < n)
        {
            uniqueNumbers.Add(rand.Next(1, 10001));
        }

        return uniqueNumbers.ToArray();
    }

    public static int[] ParallelTreeMax(int[] A)
    {
        int n = A.Length;
        int[] B = new int[n];
        int logn = (int)Math.Log2(n);
        int loglogn = (int)Math.Log2(logn);

        Parallel.For(0,n, i =>
        {
            B[i] = A[i];
        });

        for (int h = 1; h <= loglogn; h++)
        {
            int step = n / (int)Math.Pow(2, h);
            Parallel.For(0, step, i =>
            {
                B[i] = Math.Max(B[2 * i], B[2 * i + 1]);
            });
            
        };
        return B.Take(n/ (int)Math.Pow(2, loglogn)).ToArray();
    }

    public static int ConstantTimeMax(int[] A)
    {
        int n = A.Length;
        int[,] B = new int[n, n];
        int[] M = new int[n];
        int b = 0;

        Parallel.For(0, n, i =>
        {
            Parallel.For(0, n, j =>
            {
                if (A[i] >= A[j])
                    B[i, j] = 1;
                else
                    B[i, j] = 0;
            });
        });

        Parallel.For(0, n, i =>
        {
            M[i] = 1;
            Parallel.For(0, n, j =>
            {
                if (B[i, j] == 0) { 
                    M[i] = 0;
                }
            });
        });

        Parallel.For(0, n, i =>
        {
            if (M[i] == 1)
            {
                b = A[i];
            }
        });

        return b;
    }

    public static int ComputeAcceleratedCascadingMaximumProblem(int[] A)
    {
        int n = A.Length;
        int logn = (int)Math.Log2(n);
        int k = (int)Math.Log2(logn);

        for (int i = k - 1; i >= 0; i--)
        {
            int numBlocks = (int)Math.Pow(2, Math.Pow(2, k) - Math.Pow(2, k - i));
            int blockSize = (int)Math.Pow(2, Math.Pow(2, k - i - 1));

            Parallel.For(0, numBlocks, j =>
            {
                int blockStartIdx = j * blockSize;
                A[j] = ConstantTimeMax(A[blockStartIdx..(blockStartIdx + blockSize)]);
            });
        }

        return A[0];
    }

    private static void PrintOutput(int[] A, int S)
    {
        Console.Clear();
        Console.WriteLine("\n----------------------------");
        Console.WriteLine("Input array:");
        Console.WriteLine(string.Join(", ", A));
        Console.WriteLine("\nMax number from input using accelerated cascading:");
        Console.WriteLine(S);
        Console.WriteLine("----------------------------\n");
    }

    private static void PrintOutputToFile(int[] A, int S)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string baseFileName = Path.Combine(currentDirectory, "output");
        string fileName = $"{baseFileName}.txt";

        int fileIndex = 1;
        while (File.Exists(fileName))
        {
            fileName = $"{baseFileName}{fileIndex}.txt";
            fileIndex++;
        }

        using StreamWriter writer = new(fileName);
        Console.WriteLine($"Output written to file: {fileName}\n");
        writer.WriteLine("\n----------------------------");
        writer.WriteLine("Input array:");
        writer.WriteLine(string.Join(", ", A));
        writer.WriteLine("\nMax number from input using accelerated cascading:");
        writer.WriteLine(S);
        writer.WriteLine("----------------------------\n");
    }

    private static void ExitOrContinue()
    {
        Console.WriteLine("Press 1 to continue or any other key to exit.");
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.KeyChar == '1')
        {
            Main();
        }
        else
        {
            Environment.Exit(0);
        }
    }
}