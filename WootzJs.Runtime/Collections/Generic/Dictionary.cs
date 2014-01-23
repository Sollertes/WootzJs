﻿using System.Linq;
using System.Runtime.WootzJs;

namespace System.Collections.Generic
{
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private JsObject storage = new JsObject();
        private List<Bucket> buckets = new List<Bucket>();
        private int count;
        private DictionaryKeys keys;

        public Dictionary()
        {
            keys = new DictionaryKeys(this);
        }

        public void Add(TKey key, TValue value)
        {
            var hashCode = key.GetStringHashCode();
            var bucket = storage[hashCode].As<Bucket>();
            if (bucket == null)
            {
                bucket = new Bucket(hashCode);
                storage[hashCode] = bucket.As<JsObject>();
                buckets.Add(bucket);
            }
            var existingItem = bucket.Items.SingleOrDefault(x => x.Key.Equals(key));
            if (existingItem == null)
            {
                bucket.Items.Add(new BucketItem(key, value));
                count++;
            }
            else
            {
                existingItem.Value = value;
            }
        }

        public bool Remove(TKey key)
        {
            var hashCode = key.GetStringHashCode();
            var bucket = storage[hashCode].As<Bucket>();
            if (bucket != null)
            {
                var items = bucket.Items.Where(x => x.Key.Equals(key)).GetEnumerator();
                if (items.MoveNext())
                {
                    var item = items.Current;
                    items.Dispose();
                    bucket.Items.Remove(item);
                    if (bucket.Items.Count == 0)
                    {
                        Jsni.delete(storage[hashCode]);
                    }
                    count--;
                    return true;
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            var hashCode = key.GetStringHashCode();
            var bucket = storage[hashCode].As<Bucket>();
            if (bucket == null)
                return false;

            return bucket.Items.Any(x => x.Key.Equals(key));
        }

        public void Clear()
        {
            foreach (var bucket in buckets)
            {
                var hashCode = bucket.HashCode;
                Jsni.delete(storage[hashCode]);
            }
            count = 0;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var hashCode = key.GetStringHashCode();
            var bucket = storage[hashCode].As<Bucket>();
            if (bucket == null)
            {
                value = default(TValue);
                return false;
            }
            var items = bucket.Items.GetEnumerator();
            if (!items.MoveNext())
            {
                value = default(TValue);
                return false;
            }
            value = items.Current.Value;
            items.Dispose();
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue result;
                if (!TryGetValue(key, out result))
                    throw new KeyNotFoundException(key.ToString());
                return result;
            }
            set
            {
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return keys; }
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        object IDictionary.this[object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey)key] = (TValue)value; }
        }

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException(); }
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.Contains(object key)
        {
            return false;
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        class Bucket
        {
            public string HashCode { get; set; }
            public List<BucketItem> Items = new List<BucketItem>();

            public Bucket(string hashCode)
            {
                HashCode = hashCode;
            }
        }

        class BucketItem
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public BucketItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        class DictionaryKeys : ICollection<TKey>
        {
            private Dictionary<TKey, TValue> dictionary;

            public DictionaryKeys(Dictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public bool Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            public int Count
            {
                get { return dictionary.Count; }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return dictionary.buckets.SelectMany(x => x.Items).Select(x => x.Key).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public object SyncRoot
            {
                get { return dictionary.SyncRoot; }
            }

            public bool IsSynchronized
            {
                get { return dictionary.IsSynchronized; }
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(TKey item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(TKey item)
            {
                throw new NotImplementedException();
            }
        }
    }
}