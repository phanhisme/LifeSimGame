using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        //new array of items
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        SortUp(item);
        //add to the array
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);
        return firstItem;
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        if (item.HeapIndex < currentItemCount)
        {
            return Equals(items[item.HeapIndex], item);
        }
        else
            return false;
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            //make sure swap index is on high priority
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //if parent value < swap index => swap them
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                    return;
            }
            else
                return;
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }

            //no longer a priority to the parent -> break off the loop
            else
                break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    //swap if the child > parent
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

    public void Clear()
    {
        currentItemCount = 0;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
