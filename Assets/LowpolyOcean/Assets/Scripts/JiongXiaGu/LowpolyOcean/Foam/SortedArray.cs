using System.Collections;
using System.Collections.Generic;

namespace JiongXiaGu.LowpolyOcean
{

    /// <summary>
    /// Array for sorting, discarding extra elements when full
    /// </summary>
    public class SortedArray<T> : IEnumerable<T>
    {
        public T[] Values { get; }
        public IComparer<T> Comparer { get; set; }

        public T this[int index]
        {
            get { return Values[index]; }
            set { Values[index] = value; }
        }

        public SortedArray(int index) : this(index, Comparer<T>.Default)
        {
        }

        public SortedArray(int index, IComparer<T> comparer)
        {
            Values = new T[index];
            Comparer = comparer;
        }

        public void Add(T item)
        {
            for (int index = 0; index < Values.Length; index++)
            {
                var current = Values[index];
                if (Comparer.Compare(current, item) >= 0)
                {
                    for (int lastIndex = Values.Length - 1; lastIndex > index; lastIndex--)
                    {
                        var previousIndex = lastIndex - 1;
                        Values[lastIndex] = Values[previousIndex];
                    }

                    Values[index] = item;
                    return;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = default(T);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Values).GetEnumerator();
        }
    }
}
