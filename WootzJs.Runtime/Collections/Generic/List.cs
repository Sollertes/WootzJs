﻿using System.Runtime.WootzJs;

namespace System.Collections.Generic
{
    public class List<T> : IList<T>
    {
        private JsArray storage = new Array().As<JsArray>();

        public List()
        {
        }

        public List(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }

        public int Count
        {
            get { return storage.length; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            for (var i = index; i < array.Length; i++)
            {
                array[i] = this[i - index];
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public int Add(object value)
        {
            storage.push(value.As<JsObject>());
            return storage.length;
        }

        public bool Contains(object value)
        {
            return storage.indexOf(value.As<JsObject>()) >= 0;
        }

        public void Clear()
        {
            storage.length = 0;
        }

        public int IndexOf(object value)
        {
            return storage.indexOf(value.As<JsObject>());
        }

        public void Insert(int index, object value)
        {
            storage.splice(index, 0, value.As<JsObject>());
        }

        public void Remove(object value)
        {
            var index = IndexOf(value);
            if (index >= 0)
                RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            storage.splice(index, 1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator(this);
        }

        public void Add(T item)
        {
            storage.push(item.As<JsObject>());
        }

        public bool Contains(T item)
        {
            return storage.indexOf(item.As<JsObject>()) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < array.Length; i++)
            {
                array[i] = this[i - arrayIndex];
            }
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public T this[int index]
        {
            get { return storage[index].As<T>(); }
            set { storage[index] = value.As<JsObject>(); }
        }

        public int IndexOf(T item)
        {
            return storage.indexOf(item.As<JsObject>());
        }

        public void Insert(int index, T item)
        {
            storage.splice(index, 0, item.As<JsObject>());
        }

        class ListEnumerator : IEnumerator<T>
        {
            private int index = -1;
            private List<T> list;

            public ListEnumerator(List<T> list)
            {
                this.list = list;
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                index++;
                return index < list.Count;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public T Current
            {
                get { return list[index]; }
            }
        }
    }
}