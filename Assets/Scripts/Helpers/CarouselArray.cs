using System;
using System.Collections.Generic;

public class CarouselArray<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;

    private int _currentIndex = 0;

    public int CurrentIndex => _currentIndex;
    public T Current => _items[_currentIndex];

    public CarouselArray(int capacity = 4)
    {
        _items = new T[capacity];
        _count = 0;
    }

    public CarouselArray(T[] array)
    {
        _items = array;
        _count = array.Length;
    }

    public CarouselArray(IEnumerable<T> collection)
    {
        _items = new T[4];
        _count = 0;

        foreach (var item in collection)
            Add(item);
    }

    
    public static CarouselArray<T> FromArray(T[] array)
    {
        var result = new CarouselArray<T>(array.Length);
        foreach (var item in array)
            result.Add(item);

        return result;
    }

    // Indexer (array[index])
    public T this[int index]
    {
        get
        {
            ValidateIndex(index);
            return _items[index];
        }
        set
        {
            ValidateIndex(index);
            _items[index] = value;
        }
    }

    public void Add(T item)
    {
        if (_count == _items.Length)
            Resize(_items.Length * 2);

        _items[_count++] = item;
    }

    public T Next()
    {
        if (Count == 0)
            throw new InvalidOperationException("Empty");

        _currentIndex = (_currentIndex + 1) % Count;
        return Current;
    }

    public T Previous()
    {
        if (Count == 0)
            throw new InvalidOperationException("Empty");

        _currentIndex = (_currentIndex - 1 + Count) % Count;
        return Current;
    }

    public T PeekNext()
    {
        return _items[(_currentIndex + 1) % Count];
    }

    public T PeekPrevious()
    {
        return _items[(_currentIndex - 1 + Count) % Count];
    }

    public T[] GetWindow(int radius)
    {
        if (Count == 0)
            return Array.Empty<T>();

        var result = new T[radius * 2 + 1];

        for (int i = -radius; i <= radius; i++)
        {
            int index = (_currentIndex + i + Count) % Count;
            result[i + radius] = _items[index];
        }

        return result;
    }

    public bool TrySetCurrent(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
            {
                _currentIndex = i;
                return true;
            }
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        ValidateIndex(index);

        for (int i = index; i < _count - 1; i++)
            _items[i] = _items[i + 1];

        _items[_count - 1] = default!;
        _count--;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
        _count = 0;
    }

    private void Resize(int newSize)
    {
        T[] newArray = new T[newSize];
        Array.Copy(_items, newArray, _count);
        _items = newArray;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
            yield return _items[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}