namespace _23treeNew
{
    // Třída Node obsahuje základní metody pro práci s uzelm 2-3 stromu
    class Node
    {
        List<Node> children;                        // privátní seznam potomků 
        public Node parent { get; internal set; }    // rodičovský uzel
        public int key { get; private set; }        // max. klíč v podstromu tohoto uzlu

        /* Neudržujeme hodnoty l,m,r přímo v uzlu.
        Získáme je jako children[0].key, children[1].key, children[2].key
        */

        public Node(int value)
        {
            key = value;
            children = new List<Node>();
        }

        public int NoOfChildren => children.Count;

        // Vrátí potomka na pozici idx, počítáno od nuly
        // Případnou výjimku neřešíme
        public Node Child(int idx) => children[idx];

        // Přidá nového potomka a zatřídí ho do seznamu podle jeho klíče
        public void AddChild(Node node)
        {
            node.parent = this;
            var position = children.FindIndex(child => node.key <= child.key);
            position = position < 0 ? children.Count : position;
            children.Insert(position, node);
            UpdateKey();
        }

        // Odstraní potomka na pozici idx, počítáno od nuly
        // Případnou výjimku neřešíme
        public void RemoveChild(int idx)
        {
            if (idx >= 0 && idx < children.Count)
            {
                children[idx].parent = null;
                children.RemoveAt(idx);
                UpdateKey();
            }
        }

        // Přepočítá klíč, nutno volat vždy, když se něco změnilo u potomků 
        public void UpdateKey()
        {
            if (children.Any())
                key = children.Max(child => child.key);
        }

        // True pokud uzel je listem
        public bool isLeaf()
        {
            return !children.Any();
        }
    }
    // Tato třída bude obsahovat veškeré potřebné metody ke vkládání uzlů do stromu
    class Tree23
    {
        Node root;

        // Konstruktor vytváří strom sestávající z kořene
        public Tree23()
        {
            root = new Node(int.MaxValue);
        }

        // vrací list s nejmenším klíčem, který je aspoň tak velký jako 
        // pokud takový klíč ve stromu není, vrací nejpravější list
        public Node Search(int key)
        {
            return Search(root, key);
        }

        // vrací list podstromu  s nejmenším klíčem, který je aspoň tak velký jako 
        // pokud takový klíč v podstromu není, vrací nejpravější list
        Node Search(Node node, int key)
        {
            if (node.isLeaf())
                return node;

            int i = 0;
            while (i < node.NoOfChildren - 1 && key > node.Child(i).key)
                i++;
            return Search(node.Child(i), key);
        }

        // vloží do stromu nový list s klíčem key a případně strom vyváží
        // vrací false, pokud klíč už ve stromě je
        public bool Insert(int key)
        {
            Node leaf = Search(key);
            if (leaf.key == key && leaf.isLeaf())
            {
                return false;
            }

            Node newLeaf = new(key);

            // Nový uzel se má přidat k rodiči nalezeného listu
            // Pokud nalezený list nemá rodiče, znamená to, že strom sestává pouze z kořene 
            // Pak tedy přidáváme nový uzel jako potomka kořene

            // Bude-li tuto metodu volat více vláken, musíte nyní zamknout . 
            // Pokud se to nepovede, čekáte na odemčení.
            // Pak ale musíte znovu získat , protože se mezitím mohl změnit!
            // Jiné vlákno mohlo vyvažovat  a přepojit  na jiného rodiče. 
            // Pokus o zamčení se tedy musí dělat v cyklu while - DOPLŇTE ZDE
            while (true)
            {
                Node parent = leaf.parent ?? leaf;

                if (Monitor.TryEnter(parent, 50))
                {
                    try
                    {
                        if (parent == (leaf.parent ?? leaf))
                        {
                            parent.AddChild(newLeaf);
                            // Metoda Balance postupuje stromem vzhůru a vyvažuje postupně vyšší uzly.
                            // Vyvažovaný uzel vždy zamkne (zámky jsou reentrantní) a pak odemkne. 
                            Balance(parent);
                            return true;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(parent);
                    }
                }
            }
        }

        private void Balance(Node node)
        {
            if (node == null) return;

            bool lockAcquired = false;
            Monitor.Enter(node, ref lockAcquired);
            try
            {
                if (node.NoOfChildren <= 3)
                {
                    node.UpdateKey();
                }
                else
                {
                    Node sibling = new(0);
                    int middle = node.NoOfChildren / 2;
                    for (int i = node.NoOfChildren - 1; i >= middle; i--)
                    {
                        Node child = node.Child(i);
                        node.RemoveChild(i);
                        sibling.AddChild(child);
                    }
                    sibling.UpdateKey();
                    node.UpdateKey();

                    Node parent = node.parent;
                    if (parent == null)
                    {
                        Node newRoot = new(sibling.Child(0).key);
                        newRoot.AddChild(node);
                        newRoot.AddChild(sibling);
                        root = newRoot;
                    }
                    else
                    {
                        parent.AddChild(sibling);
                    }
                }
            }
            finally
            {
                if (lockAcquired) Monitor.Exit(node);
            }

            Balance(node.parent);
        }

        // Metoda vrací textový výpis celého stromu v jednotlivých řádcích pod sebou
        public override string ToString()
        {
            List<Node> level = new List<Node> { root };
            string result = "";
            while (level.Any())
            {
                List<Node> lowerLevel = new List<Node>();
                // Nodes of different parents will be separated by |
                Node parentOfFirstNodeInLevel = null;
                if (level.Any())
                {
                    parentOfFirstNodeInLevel = level[0].parent;
                }

                foreach (Node node in level)
                {
                    result += (node.parent == parentOfFirstNodeInLevel ? "" : "|  ")
                              + (node.key == int.MaxValue ? "+N" : node.key.ToString())
                              + " (" + node.NoOfChildren + ")  ";
                    parentOfFirstNodeInLevel = node.parent;
                    for (int i = 0; i < node.NoOfChildren; i++)
                        lowerLevel.Add(node.Child(i));
                }
                result += "\n";
                level = lowerLevel;
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Tree23 tree = new();
            int[] A = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
            int[] B = [15, 25, 35, 45, 55, 65, 75, 85, 95, 105];

            foreach (int i in A)
            {
                tree.Insert(i);
            }

            Parallel.ForEach(B, i =>
            {
                tree.Insert(i);
            });
            Console.WriteLine(tree.ToString());
        }
    }   
}