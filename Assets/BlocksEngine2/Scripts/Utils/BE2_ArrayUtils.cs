using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Utils
{
    public static class BE2_ArrayUtils
    {
        public static void Resize<T>(ref T[] array, int size)
        {
            T[] tempArray = array;
            array = new T[size];
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (size > i)
                    array[i] = tempArray[i];
            }
        }

        public static void Add<T>(ref T[] array, T value)
        {
            int length = array.Length;
            Resize<T>(ref array, length + 1);
            array[length] = value;
        }

        public static T[] AddReturn<T>(T[] array, T value)
        {
            int length = array.Length;
            T[] newArray = array;
            Resize<T>(ref newArray, length + 1);
            newArray[length] = value;
            return newArray;
        }

        public static void Remove<T>(ref T[] array, T value)
        {
            List<T> list = new List<T>();
            list.AddRange(array);
            list.Remove(value);
            array = list.ToArray();
        }

        // v2.10 - BE2_ArrayUtins FindAll and Find methods refactored to use System.Array class 
        public static T[] FindAll<T>(ref T[] array, System.Predicate<T> match)
        {
            return System.Array.FindAll(array, match);
        }

        // v2.10 - BE2_ArrayUtins FindAll and Find methods refactored to use System.Array class
        public static T Find<T>(ref T[] array, System.Predicate<T> match)
        {
            return System.Array.Find(array, match);
        }
    }
}
