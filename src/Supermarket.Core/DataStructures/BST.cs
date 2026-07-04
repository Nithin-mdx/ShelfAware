namespace Supermarket.App.DataStructures;

// Binary search tree used for searching products by name
// Average O(log n) for insert and search,
public class BST<TKey, TValue> where TKey : IComparable<TKey>
{
    // Duplicate keys (two products with the same name) keep all their values on one node
    private sealed class Node
    {
        public TKey Key;
        public CstmLinkedList<TValue> Values = new();
        public Node? Left, Right;
        public Node(TKey key) => Key = key;
    }

    private Node? _root;
    public int Count { get; private set; }

    // Inserts a key-value pair into the tree
    public void Insert(TKey key, TValue value)
    {
        _root = Insert(_root, key, value);
        Count++;
    }

    private Node Insert(Node? node, TKey key, TValue value)
    {
        if (node is null) { var n = new Node(key); n.Values.Add(value); return n; }

        // smaller keys go left, larger go right, equal keys join the existing node
        int cmp = key.CompareTo(node.Key);
        if (cmp == 0) node.Values.Add(value);
        else if (cmp < 0) node.Left = Insert(node.Left, key, value);
        else node.Right = Insert(node.Right, key, value);
        return node;
    }

    // Returns all values stored under the given key, or an empty list if not found
    public CstmLinkedList<TValue> Search(TKey key)
    {
        // each comparison discards half of the remaining tree
        var node = _root;
        while (node is not null)
        {
            int cmp = key.CompareTo(node.Key);
            if (cmp == 0) return node.Values;
            node = cmp < 0 ? node.Left : node.Right;
        }
        return new CstmLinkedList<TValue>();
    }

    // Returns every value in ascending key order using an in-order traversal
    public CstmLinkedList<TValue> InOrder()
    {
        var result = new CstmLinkedList<TValue>();
        Walk(_root, result);
        return result;
    }

    private static void Walk(Node? node, CstmLinkedList<TValue> acc)
    {
        if (node is null) return;
        Walk(node.Left, acc);
        foreach (var v in node.Values) acc.Add(v);
        Walk(node.Right, acc);
    }
}
