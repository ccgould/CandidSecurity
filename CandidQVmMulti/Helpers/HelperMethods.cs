using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidQVmMulti.Helpers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Sorts an ObservableCollection in place based on a key selector.
        /// </summary>
        /// <typeparam name="T">Type of items in the collection.</typeparam>
        /// <typeparam name="TKey">Type of the key to sort by.</typeparam>
        /// <param name="collection">The ObservableCollection to sort.</param>
        /// <param name="keySelector">Function to select the key for sorting.</param>
        /// <param name="ascending">True for ascending order, false for descending.</param>
        public static void Sort<T, TKey>(
            this ObservableCollection<T> collection,
            Func<T, TKey> keySelector,
            bool ascending = true)
        {
            if (collection == null || keySelector == null)
                throw new ArgumentNullException();

            var sortedItems = ascending
                ? collection.OrderBy(keySelector).ToList()
                : collection.OrderByDescending(keySelector).ToList();

            for (int i = 0; i < sortedItems.Count; i++)
            {
                var item = sortedItems[i];
                var oldIndex = collection.IndexOf(item);
                if (oldIndex != i)
                    collection.Move(oldIndex, i);
            }
        }
    }

}
