using Supermarket.App.DataStructures;
using Supermarket.App.Models;

namespace Supermarket.App.Algorithms;

// Search and sort algorithms written by hand for the coursework
public static class SearchAlgo
{
    // Linear search - checks every item one by one, O(n), works on unsorted data
    public static CstmLinkedList<T> LinearSearch<T>(IEnumerable<T> items, Func<T, bool> match)
    {
        var result = new CstmLinkedList<T>();
        foreach (var item in items)
            if (match(item)) result.Add(item);
        return result;
    }

    // Binary search by product name on an array already sorted by name
    // O(log n), returns the index of the match or -1 if not found
    public static int BinarySearch(Product[] sorted, string name)
    {
        name = name.ToLower();
        int lo = 0, hi = sorted.Length - 1;
        while (lo <= hi)
        {
            // check the middle item and discard the half that cant contain the target
            int mid = (lo + hi) / 2;
            int cmp = sorted[mid].Name.ToLower().CompareTo(name);
            if (cmp == 0) return mid;
            if (cmp < 0) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }

    // Quicksort of products by name, O(n log n) average case
    public static void QuickSort(Product[] arr) => QuickSort(arr, 0, arr.Length - 1);

    private static void QuickSort(Product[] arr, int lo, int hi)
    {
        if (lo >= hi) return;

        // items smaller than the pivot end up on its left, larger on its right
        string pivot = arr[(lo + hi) / 2].Name.ToLower();
        int i = lo, j = hi;
        while (i <= j)
        {
            while (arr[i].Name.ToLower().CompareTo(pivot) < 0) i++;
            while (arr[j].Name.ToLower().CompareTo(pivot) > 0) j--;
            if (i <= j)
            {
                var temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
                i++; j--;
            }
        }

        // sort both halves the same way
        QuickSort(arr, lo, j);
        QuickSort(arr, i, hi);
    }
}
