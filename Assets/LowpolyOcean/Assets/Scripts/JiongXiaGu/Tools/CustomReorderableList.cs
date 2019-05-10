using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu
{

    /// <summary>
    /// Show the linked list as <see cref="UnityEditorInternal.ReorderableList"/>,Generally inherit <see cref="CustomReorderableList{T}"/>
    /// </summary>
    [Serializable]
    public abstract class CustomReorderableList
    {
        public const string ListFieldName = "List";
    }

    /// <summary>
    /// Show the linked list as <see cref="UnityEditorInternal.ReorderableList"/>;
    /// </summary>
    [Serializable]
    public abstract class CustomReorderableList<T> : CustomReorderableList, IList<T>, IReadOnlyList<T>
    {
        public List<T> List;

        public CustomReorderableList() : this(new List<T>())
        {
        }

        public CustomReorderableList(List<T> list)
        {
            List = list;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomReorderableList<T>);
        }

        public bool Equals(CustomReorderableList<T> other)
        {
            return other != null &&
                   EqualityComparer<IList>.Default.Equals(List, other.List);
        }

        public override int GetHashCode()
        {
            return -771917257 + EqualityComparer<IList>.Default.GetHashCode(List);
        }
        
        public static bool operator ==(CustomReorderableList<T> list1, CustomReorderableList<T> list2)
        {
            return EqualityComparer<CustomReorderableList<T>>.Default.Equals(list1, list2);
        }

        public static bool operator !=(CustomReorderableList<T> list1, CustomReorderableList<T> list2)
        {
            return !(list1 == list2);
        }

        public static implicit operator List<T>(CustomReorderableList<T> item)
        {
            return item == null ? null : item.List;
        }

        public static implicit operator bool(CustomReorderableList<T> exists)
        {
            return exists != null && exists.List != null;
        }


        #region IList<T>, IReadOnlyList<T>

        public int Count
        {
            get { return ((IList<T>)List).Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList<T>)List).IsReadOnly; }
        }

        public T this[int index]
        {
            get { return ((IList<T>)List)[index]; }
            set { ((IList<T>)List)[index] = value; }
        }


        public int IndexOf(T item)
        {
            return ((IList<T>)List).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)List).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)List).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((IList<T>)List).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)List).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)List).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)List).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)List).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)List).GetEnumerator();
        }

        #endregion
    }
}
