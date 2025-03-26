class PrefixSumsDivideAndConquer
{
    static void Main()
    {
        int[] X = getUserInput();
        int[] S = ComputePrefixSumsDivideAndConquer(X);

        printOutput(X, S);

        exitOrContinue();
    }

    public static int[] ComputePrefixSumsDivideAndConquer(int[] X)
    {
        int n = X.Length;
        int[] S = new int[n];

        if(n == 1)
        {
            S[0] = X[0];
            return S;
        }

        int[] Z1 = new int[n / 2];
        int[] Z2 = new int[n / 2];

        Parallel.For(0, n / 2, i =>
        {
            Z1[i] = X[i];
        });
        Parallel.For(0, n / 2, i =>
        {
            Z2[i] = X[n / 2 + i];
        });

        Parallel.Invoke(
            () => { Z1 = ComputePrefixSumsDivideAndConquer(Z1); },
            () => { Z2 = ComputePrefixSumsDivideAndConquer(Z2); }
        );

        Parallel.For(0, n / 2, i =>
        {
            S[i] = Z1[i];
        });
        Parallel.For(0, Z2.Length, i =>
        {
            S[n / 2 + i] = Z2[i] + Z1[Z1.Length - 1];
        });

        return S;
    }

    private static int[] getUserInput()
    {
        Console.WriteLine("Enter the elements separated by SPACES (number of elements must be a power of 2): ");
        string[] input = Console.ReadLine().Split(' ');
        int n = input.Length;

        if ((n & (n - 1)) != 0)
        {
            Console.WriteLine("The number of elements must be a power of 2.");
            return getUserInput();
        }

        int[] A = new int[n];
        for (int i = 0; i < n; i++)
        {
            if (!int.TryParse(input[i], out A[i]))
            {
                Console.WriteLine($"Invalid input '{input[i]}'. Please enter only integers.");
                return getUserInput();
            }
        }
        return A;
    }

    private static void printOutput(int[] A, int[] S)
    {
        Console.Clear();
        Console.WriteLine("\n----------------------------");
        Console.WriteLine("Input array:");
        Console.WriteLine(string.Join(", ", A));
        Console.WriteLine("Prefix sums using divide and conquer:");
        Console.WriteLine(string.Join(", ", S));
        Console.WriteLine("----------------------------\n");
    }

    private static void exitOrContinue()
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