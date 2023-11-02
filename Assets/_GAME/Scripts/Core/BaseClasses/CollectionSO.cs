using UnityEngine;

namespace MemeFight
{
    public class CollectionSO<T> : BaseSO where T : Object
    {
        [Space(10)]
        public T[] items;

        /// <summary>
        /// Returns the total amount of items in the collection.
        /// </summary>
        public int Total => items.Length;
    }
}
