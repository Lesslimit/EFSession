using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EFSession.DataStructures.Collections
{
    public class HashStack<T> : ICollection, IReadOnlyCollection<T>
    {
        private readonly ConcurrentStack<T> internalStack;
        private readonly ConcurrentDictionary<T, bool> internalSet;

        int ICollection.Count => internalStack.Count;

        object ICollection.SyncRoot => ((ICollection)internalStack).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)internalStack).IsSynchronized;

        int IReadOnlyCollection<T>.Count => internalStack.Count;

        public HashStack()
        {
            internalSet = new ConcurrentDictionary<T, bool>();
            internalStack = new ConcurrentStack<T>();
        }

        public HashStack(IEqualityComparer<T> equalityComparer)
        {
            internalSet = new ConcurrentDictionary<T, bool>(equalityComparer);
            internalStack = new ConcurrentStack<T>();
        }

        public bool Push(T item)
        {
            if (!internalSet.ContainsKey(item) && internalSet.TryAdd(item, true))
            {
                internalStack.Push(item);

                return true;
            }

            return false;
        }

        public T Pop()
        {
            T result;
            return internalStack.TryPop(out result) ? result : default(T);
        }

        public T Peek()
        {
            T result;
            return internalStack.TryPeek(out result) ? result : default(T);
        }

        public T[] ToArray()
        {
            return internalStack.ToArray();
        }

        public void Clear()
        {
            internalSet.Clear();
            internalStack.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return internalStack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            internalStack.CopyTo(array.OfType<T>().ToArray(), index);
        }
    }
}