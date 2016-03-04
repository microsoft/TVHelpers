using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Convert this collection into an ObservableCollection instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns>ObservableCollection instance containing items of the type specified.</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
    {
        var list = new ObservableCollection<T>();
        foreach (var o in collection)
            list.Add(o);
        return list;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var o in source)
            action(o);
    }
}
