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
        PrintOutputToFile(A, S);

        ExitOrContinue();
    }

    public static int[] ComputeSampleSort(int[] A)
    {
        return [];
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
        Console.Clear();
        Console.WriteLine("\n----------------------------");
        Console.WriteLine("Input array:");
        Console.WriteLine(string.Join(", ", A));
        Console.WriteLine("Sorted array using sample sort:");
        Console.WriteLine(string.Join(", ", S));
        Console.WriteLine("----------------------------\n");
    }

    private static void PrintOutputToFile(int[] A, int[] S)
    {
        int fileIndex = 1;
        string fileName = "output.txt";

        while (File.Exists(fileName))
        {
            fileName = $"output{fileIndex}.txt";
            fileIndex++;
        }

        StreamWriter writer = new(fileName);
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