using System;

namespace MunicipalServicesMVC.Models;

/// <summary>
/// A lightweight in-memory dynamic array store for <see cref="Issue"/> objects.
/// Custom implementation avoids using <see cref="List{T}"/> for demonstration purposes.
/// </summary>
public sealed class IssueStore
{
    private Issue[] _items;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssueStore"/> class.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity of the underlying array (default = 8).</param>
    public IssueStore(int initialCapacity = 8)
    {
        if (initialCapacity < 1)
        {
            initialCapacity = 8;
        }

        _items = new Issue[initialCapacity];
        _count = 0;
    }

    /// <summary>
    /// Gets the number of items currently stored.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Adds a new <see cref="Issue"/> to the store.
    /// Automatically resizes the array if necessary.
    /// </summary>
    /// <param name="item">The issue to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public void Add(Issue item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        if (_count == _items.Length)
        {
            Resize(_items.Length * 2);
        }

        _items[_count++] = item;
    }

    /// <summary>
    /// Finds an <see cref="Issue"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The issue ID.</param>
    /// <returns>The matching <see cref="Issue"/> if found; otherwise, <c>null</c>.</returns>
    public Issue? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;

        for (int i = 0; i < _count; i++)
        {
            if (_items[i].Id == id)
            {
                return _items[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a shallow copy of the store’s contents as an array.
    /// </summary>
    public Issue[] ToArray()
    {
        var copy = new Issue[_count];
        for (int i = 0; i < _count; i++)
        {
            copy[i] = _items[i];
        }
        return copy;
    }

    /// <summary>
    /// Resizes the internal array to a new capacity.
    /// </summary>
    /// <param name="newCapacity">The new array capacity.</param>
    private void Resize(int newCapacity)
    {
        var next = new Issue[newCapacity];
        for (int i = 0; i < _count; i++)
        {
            next[i] = _items[i];
        }
        _items = next;
    }
}
