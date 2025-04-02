using System.Collections.Concurrent;
using System.Collections.Immutable;

class SampleSort
{
    static void Main()
    {
        int[] A = GenerateNumbers(1024);
        int S = ComputeAcceleratedCascadingMaximumProblem(A);

        PrintOutput(A, S);
        //PrintOutputToFile(A, S);

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


    public static int ComputeAcceleratedCascadingMaximumProblem(int[] A)
    {
        return A.Max();
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

    private static void PrintOutputToFile(int[] A, int[] S)
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
        writer.WriteLine("\nSorted array using sample sort:");
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