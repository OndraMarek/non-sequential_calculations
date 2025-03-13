class PrefixSums
{
    static void Main()
    {
        int[] A = getUserInput();
        int[] S = ComputePrefixSums(A);

        printOutput(A, S);

        exitOrContinue();
    }

    public static int[] ComputePrefixSums(int[] A)
    {
        int n = A.Length;
        int[] S = new int[n];

        if (n == 1)
        {
            S[0] = A[0];
            return S;
        }

        int[] Y = new int[n / 2];
        int[] Z = new int[n / 2];

        Parallel.For(0, n / 2, i =>
        {
            Y[i] = A[2 * i] + A[2 * i + 1];
        });

        Z = ComputePrefixSums(Y);

        S[0] = A[0];
        Parallel.For(1, n, i =>
        {
            if (i % 2 == 0)
            {
                S[i] = Z[i / 2 - 1] + A[i];
            }
            else
            {
                S[i] = Z[i / 2];
            }
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
        Console.WriteLine("Input array:");
        Console.WriteLine(string.Join(", ", A));
        Console.WriteLine("Prefix sums:");
        Console.WriteLine(string.Join(", ", S));
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