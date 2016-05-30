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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public static class DependencyObjectExtensions
{
    /// <summary>
    /// Finds a child object by name.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="name"></param>
    /// <returns>DependencyObject instance with the name specified else null.</returns>
    public static DependencyObject GetDescendantByName(this DependencyObject element, string name)
    {
        if (element == null)
            return null;
        else if (element is FrameworkElement && (element as FrameworkElement).Name == name)
            return element;

        DependencyObject result = null;

        if (element is Control)
            (element as Control).ApplyTemplate();

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            DependencyObject visual = VisualTreeHelper.GetChild(element, i) as DependencyObject;
            result = GetDescendantByName(visual, name);
            if (result != null)
                break;
        }
        return result;
    }

    /// <summary>
    /// Finds a parent object by type.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="type"></param>
    /// <returns>DependencyObject instance if found else null.</returns>
    public static DependencyObject GetAncestorByType(this DependencyObject element, Type type)
    {
        if (element == null)
            return null;
        else if (element.GetType() == type)
            return element;
        else
            return GetAncestorByType(VisualTreeHelper.GetParent(element), type);
    }

    /// <summary>
    /// Finds a parent object by type.
    /// </summary>
    /// <typeparam name="T">Type of the ancestor to find.</typeparam>
    /// <param name="child">Object whose tree to search.</param>
    /// <param name="stopAtAncestor"></param>
    /// <returns>Instance of the object with matching type if found else null.</returns>
    public static T GetFirstAncestorByType<T>(this object child, object stopAtAncestor) where T : class
    {
        var ancestor = (DependencyObject)child;

        ancestor = VisualTreeHelper.GetParent(ancestor);

        while (ancestor != null && ancestor != stopAtAncestor)
        {
            T ancestorAsT = ancestor as T;
            if (ancestorAsT != null)
                return ancestorAsT;
            else
                ancestor = VisualTreeHelper.GetParent(ancestor);
        }

        return null;
    }

    /// <summary>
    /// Finds a child by type.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="type"></param>
    /// <returns>DependencyObject instance if found else null</returns>
    public static DependencyObject GetDescendantByType(this DependencyObject element, Type type)
    {
        if (element.GetType() == type)
            return element;

        DependencyObject foundElement = null;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetChild(element, i) as DependencyObject;
            foundElement = GetDescendantByType(dependencyObject, type);
            if (foundElement != null)
                break;
        }

        return foundElement;
    }
}