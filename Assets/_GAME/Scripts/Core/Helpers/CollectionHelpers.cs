using System;
using System.Collections.Generic;

public static class CollectionHelpers
{
    public static int LastIndex<T>(this T[] array) => array.Length - 1;
    public static int LastIndex<T>(this List<T> list) => list.Count - 1;

    public static T LastItem<T>(this T[] array) => array[array.LastIndex()];
    public static T LastItem<T>(this List<T> list) => list[list.LastIndex()];

    public static bool IsIndexValid<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    public static bool IsIndexValid<T>(this List<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    public static bool IsIndexValid<T>(this IReadOnlyList<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    public static int IndexOf<T>(this T[] array, T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public static void Trim<T>(this List<T> list, int capacity)
    {
        list.RemoveRange(capacity, list.Count - capacity);
    }

    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        if (array == null)
            return;

        foreach (T item in array)
        {
            action.Invoke(item);
        }
    }

    public static void ForEach<T>(this IReadOnlyList<T> readOnlyList, Action<T> action)
    {
        if (readOnlyList == null)
            return;

        foreach (T item in readOnlyList)
        {
            action.Invoke(item);
        }
    }
}
