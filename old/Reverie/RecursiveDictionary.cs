using System.Collections.Generic;

namespace Reverie
{
    public class RecursiveDictionary<TKey, TValue>
    {
        public RecursiveDictionary<TKey, TValue> Parent { get; set; }
        public IDictionary<TKey, TValue> Data { get; set; }

        public RecursiveDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Data = dictionary;
        }

        public RecursiveDictionary()
        {
            Data = new Dictionary<TKey, TValue>();
        }

        public TValue this[TKey key]
        {
            get
            {
                if (Data.ContainsKey(key))
                {
                    return Data[key];
                }
                if (Parent != null)
                {
                    return Parent[key];
                }
                throw new KeyNotFoundException($"Key {key} not found");
            }
            set => Data[key] = value;
        }

        public void Add(TKey key, TValue value)
        {
            Data.Add(key, value);
        }
    }
}
