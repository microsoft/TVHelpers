// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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

    /// <summary>
    /// Convert this collection into an ObservableCollection instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns>ObservableCollection instance containing items of the type specified.</returns>
    public static ObservableCollection<R> ConvertToArray<T, R>(this IEnumerable<T> collection)
    {
        var list = new ObservableCollection<R>();
        foreach (var o in collection)
            list.Add((R)Convert.ChangeType(o, typeof(R)));
        return list;
    }

    /// <summary>
    /// Performs an action on each item within the collection.
    /// </summary>
    /// <typeparam name="T">Type of the item in the collection.</typeparam>
    /// <param name="source">Collection source.</param>
    /// <param name="action">Action to perform on each item.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var o in source)
            action(o);
    }
}
