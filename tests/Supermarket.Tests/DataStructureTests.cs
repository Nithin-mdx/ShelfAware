using Supermarket.App.Algorithms;
using Supermarket.App.DataStructures;
using Supermarket.App.Models;
using Xunit;

namespace Supermarket.Tests;

public class CstmLinkedListTests
{
    private readonly CstmLinkedList<int> _list = new();

    [Fact]
    public void Add_IncreasesCount()
    {
        _list.Add(1);
        _list.Add(2);
        Assert.Equal(2, _list.Count);
    }

    [Fact]
    public void Remove_ExistingValue_ReturnsTrue()
    {
        _list.Add(1);
        _list.Add(2);
        _list.Add(3);
        Assert.True(_list.Remove(x => x == 2));
        Assert.Equal(2, _list.Count);
    }

    [Fact]
    public void Remove_MissingValue_ReturnsFalse()
    {
        _list.Add(1);
        Assert.False(_list.Remove(x => x == 99));
    }

    [Fact]
    public void ToArray_KeepsInsertionOrder()
    {
        _list.Add(1);
        _list.Add(2);
        _list.Add(3);
        Assert.Equal(new[] { 1, 2, 3 }, _list.ToArray());
    }

    [Fact]
    public void Find_ReturnsFirstMatch()
    {
        _list.Add(5);
        _list.Add(10);
        Assert.Equal(10, _list.Find(x => x > 5));
    }
}

public class BstTests
{
    private readonly BST<int, string> _tree = new();

    [Fact]
    public void Insert_And_Search_ReturnsValue()
    {
        _tree.Insert(5, "five");
        var found = _tree.Search(5);
        Assert.Equal(1, found.Count);
    }

    [Fact]
    public void Search_MissingKey_ReturnsEmpty()
    {
        _tree.Insert(1, "one");
        Assert.Empty(_tree.Search(99));
    }

    [Fact]
    public void Insert_DuplicateKey_KeepsBothValues()
    {
        _tree.Insert(2, "first");
        _tree.Insert(2, "second");
        Assert.Equal(2, _tree.Search(2).Count);
    }

    [Fact]
    public void InOrder_ReturnsValuesInKeyOrder()
    {
        _tree.Insert(5, "five");
        _tree.Insert(2, "two");
        _tree.Insert(8, "eight");
        _tree.Insert(1, "one");

        var all = _tree.InOrder().ToArray();
        Assert.Equal(new[] { "one", "two", "five", "eight" }, all);
    }
}

public class HashtableTests
{
    private readonly Hashtable<string, int> _table = new();

    [Fact]
    public void Put_And_TryGet_ReturnsValue()
    {
        _table.Put("a", 1);
        Assert.True(_table.TryGet("a", out var value));
        Assert.Equal(1, value);
    }

    [Fact]
    public void Put_SameKeyTwice_OverwritesValue()
    {
        _table.Put("a", 1);
        _table.Put("a", 10);
        _table.TryGet("a", out var value);
        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGet_MissingKey_ReturnsFalse()
    {
        Assert.False(_table.TryGet("missing", out _));
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrue()
    {
        _table.Put("a", 1);
        Assert.True(_table.Remove("a"));
        Assert.False(_table.TryGet("a", out _));
    }

    [Fact]
    public void Resize_KeepsAllEntries()
    {
        var table = new Hashtable<int, int>(4);
        for (int i = 0; i < 50; i++) table.Put(i, i * i);

        for (int i = 0; i < 50; i++)
        {
            Assert.True(table.TryGet(i, out var value));
            Assert.Equal(i * i, value);
        }
    }
}

public class SearchAlgoTests
{
    private static Product P(string name) => new() { Name = name };

    [Fact]
    public void LinearSearch_ReturnsAllMatches()
    {
        var data = new[] { 1, 2, 3, 4, 5, 6 };
        var even = SearchAlgo.LinearSearch(data, x => x % 2 == 0);
        Assert.Equal(new[] { 2, 4, 6 }, even.ToArray());
    }

    [Fact]
    public void LinearSearch_NoMatch_ReturnsEmpty()
    {
        var data = new[] { 1, 3, 5 };
        Assert.Empty(SearchAlgo.LinearSearch(data, x => x > 10));
    }

    [Fact]
    public void QuickSort_SortsByName()
    {
        var data = new[] { P("Milk"), P("Apple"), P("Rice"), P("Cola") };
        SearchAlgo.QuickSort(data);
        Assert.Equal("Apple", data[0].Name);
        Assert.Equal("Cola", data[1].Name);
        Assert.Equal("Milk", data[2].Name);
        Assert.Equal("Rice", data[3].Name);
    }

    [Fact]
    public void BinarySearch_FindsProduct()
    {
        var data = new[] { P("Milk"), P("Apple"), P("Rice"), P("Cola") };
        SearchAlgo.QuickSort(data);
        Assert.Equal(1, SearchAlgo.BinarySearch(data, "Cola"));
    }

    [Fact]
    public void BinarySearch_MissingName_ReturnsMinusOne()
    {
        var data = new[] { P("Apple"), P("Cola") };
        SearchAlgo.QuickSort(data);
        Assert.Equal(-1, SearchAlgo.BinarySearch(data, "Bread"));
    }
}
