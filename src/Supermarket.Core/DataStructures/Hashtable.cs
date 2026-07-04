namespace Supermarket.App.DataStructures;

// Hash table with separate chaining, used for barcode lookups
// Average O(1) for Put, TryGet and Remove
public class Hashtable<TKey, TValue> where TKey : notnull
{
    // One key-value pair stored inside a bucket
    private class Entry
    {
        public TKey Key;
        public TValue Value;
        public Entry(TKey key, TValue value) { Key = key; Value = value; }
    }

    private CstmLinkedList<Entry>[] _buckets;
    public int Count { get; private set; }

    public Hashtable(int capacity = 16)
    {
        _buckets = new CstmLinkedList<Entry>[Math.Max(4, capacity)];
        for (int i = 0; i < _buckets.Length; i++) _buckets[i] = new CstmLinkedList<Entry>();
    }

    // Converts a key into a bucket index, masking the hash so it cant be negative
    private int IndexFor(TKey key) => (key.GetHashCode() & 0x7FFFFFFF) % _buckets.Length;

    // Adds a value under the given key, replacing any existing value for that key
    public void Put(TKey key, TValue value)
    {
        var bucket = _buckets[IndexFor(key)];
        bucket.Remove(e => e.Key.Equals(key));
        bucket.Add(new Entry(key, value));
        Count++;

        // grow once the table is 75% full to keep the chains short
        if (Count > _buckets.Length * 0.75) Resize();
    }

    // Looks up a value by key, returns false if the key is not present
    public bool TryGet(TKey key, out TValue value)
    {
        // the hash points straight at the right bucket so nothing else is scanned
        foreach (var e in _buckets[IndexFor(key)])
            if (e.Key.Equals(key)) { value = e.Value; return true; }
        value = default!;
        return false;
    }

    // Removes the entry with the given key, returns true if it existed
    public bool Remove(TKey key)
    {
        if (_buckets[IndexFor(key)].Remove(e => e.Key.Equals(key))) { Count--; return true; }
        return false;
    }

    // Doubles the bucket array and re-places every entry using the new size
    private void Resize()
    {
        var old = _buckets;
        _buckets = new CstmLinkedList<Entry>[old.Length * 2];
        for (int i = 0; i < _buckets.Length; i++) _buckets[i] = new CstmLinkedList<Entry>();
        foreach (var bucket in old)
            foreach (var e in bucket)
                _buckets[IndexFor(e.Key)].Add(e);
    }
}
