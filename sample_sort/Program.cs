using System.Collections.Concurrent;
using System.Collections.Immutable;

class SampleSort
{
    static void Main()
    {
        int[] A = new int[1024];
        Random rand = new();

        for (int i = 0; i < 1024; i++)
        {
            A[i] = i;
        }

        for (int i = 1023; i > 0; i--)
        {
            int j = rand.Next(i + 1);

            int temp = A[i];
            A[i] = A[j];
            A[j] = temp;
        }

        int[] S = ComputeSampleSort(A);

        PrintOutput(A, S);
        //PrintOutputToFile(A, S);

        ExitOrContinue();
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

    public static int[] ComputeSampleSort(int[] A)
    {
        int n = 1024;
        int m = 15;

        int[] S = new int[m];

        int[] output = new int[n];

        Random rand = new();

        for(int i = 0; i < m; i++)
        {
            int j = rand.Next(n);
            S[i] = A[j];
        }

        Array.Sort(S);

        ConcurrentBag<int>[] B = new ConcurrentBag<int>[m];
        for (int i = 0; i < m; i++)
        {
            B[i] = [];
        }

        Parallel.For(0, n, i =>
        {
            if (A[i] < S[0])
            {
                B[0].Add(A[i]);
            }
            else if (A[i] >= S[m - 1])
            {
                B[m - 1].Add(A[i]);
            }
            else
            {
                for(int j = 0; j < m - 1; j++)
                {
                    if (A[i] >= S[j] && A[i] < S[j + 1])
                    {
                        B[j].Add(A[i]);
                        break;
                    }
                }
            }
        });

        ImmutableSortedSet<int>[] sortedB = new ImmutableSortedSet<int>[m];
        Parallel.For(0, m, i =>
        {
            sortedB[i] = B[i].ToImmutableSortedSet();
        });

        int[] LengthB = new int[m];
        for (int i = 0; i < m; i++)
        {
            LengthB[i] = sortedB[i].Count;
        }

        int[] PrefixSumsB = ComputePrefixSums(LengthB);

        return output;
    }

    private static int[] GetUserInput()
    {
        Console.WriteLine("Enter the elements separated by SPACES (number of elements must be a power of 2): ");
        string[] input = Console.ReadLine().Split(' ');
        int n = input.Length;

        int[] A = new int[n];
        for (int i = 0; i < n; i++)
        {
            if (!int.TryParse(input[i], out A[i]))
            {
                Console.WriteLine($"Invalid input '{input[i]}'. Please enter only integers.");
                return GetUserInput();
            }
        }
        return A;
    }

    private static void PrintOutput(int[] A, int[] S)
    {
        //Console.Clear();
        Console.WriteLine("\n----------------------------");
        Console.WriteLine("Input array:");
        Console.WriteLine(string.Join(", ", A));
        Console.WriteLine("Sorted array using sample sort:");
        Console.WriteLine(string.Join(", ", S));
        Console.WriteLine("----------------------------\n");
    }

    private static void PrintOutputToFile(int[] A, int[] S)
    {
        string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string baseFileName = Path.Combine(projectRoot, "output");
        string fileName = $"{baseFileName}.txt";

        int fileIndex = 1;
        while (File.Exists(fileName))
        {
            fileName = $"{baseFileName}{fileIndex}.txt";
            fileIndex++;
        }

        using StreamWriter writer = new(fileName);
        writer.WriteLine("\n----------------------------");
        writer.WriteLine("Input array:");
        writer.WriteLine(string.Join(", ", A));
        writer.WriteLine("Sorted array using sample sort:");
        writer.WriteLine(string.Join(", ", S));
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