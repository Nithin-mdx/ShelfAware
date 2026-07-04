using System.Collections;

namespace Supermarket.App.DataStructures;

// Custom singly linked list used as the main in-memory store for products
// Add is O(1) using  the tail pointer  
public class CstmLinkedList<T> : IEnumerable<T>
{
    // Each node holds one value and a link to the next node in the chain
    private sealed class Node
    {
        public T Value;
        public Node? Next;
        public Node(T value) => Value = value;
    }

    private Node? _head;
    private Node? _tail;
    public int Count { get; private set; }

    // Adds a value to the end of the list
    public void Add(T value)
    {
        var node = new Node(value);
        if (_head is null) _head = _tail = node;
        else { _tail!.Next = node; _tail = node; }
        Count++;
    }

    // Removes the first value matching the condition, returns true if something was removed
    public bool Remove(Func<T, bool> match)
    {
        Node? prev = null;
        for (var cur = _head; cur is not null; prev = cur, cur = cur.Next)
        {
            if (!match(cur.Value)) continue;

            // unlink the node by pointing the previous one straight past it
            if (prev is null) _head = cur.Next; else prev.Next = cur.Next;
            if (cur == _tail) _tail = prev;
            Count--;
            return true;
        }
        return false;
    }

    // Returns the first value matching the condition or default if nothing matches
    public T? Find(Func<T, bool> match)
    {
        for (var cur = _head; cur is not null; cur = cur.Next)
            if (match(cur.Value)) return cur.Value;
        return default;
    }

    // Copies the list into an array so it can be sorted and binary searched
    public T[] ToArray()
    {
        var arr = new T[Count];
        var i = 0;
        for (var cur = _head; cur is not null; cur = cur.Next) arr[i++] = cur.Value;
        return arr;
    }

    // Lets the rest of the app loop over the list with foreach
    public IEnumerator<T> GetEnumerator()
    {
        for (var cur = _head; cur is not null; cur = cur.Next) yield return cur.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
