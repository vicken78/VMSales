using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using VMSales.ViewModels;

namespace VMSales.Logic
{
    public class DataProcessor<T> where T : BaseViewModel // Constrain T to inherit BaseViewModel
    {
        // Compare method that returns ObservableCollection<T>
        public ObservableCollection<T> Compare(
            ObservableCollection<T> observableCollectionClean,
            ObservableCollection<T> observableCollectionDirty)
        {
            var differences = new ObservableCollection<T>();

            if (observableCollectionClean == null || observableCollectionDirty == null)
                throw new ArgumentNullException("Both collections must be non-null.");

            // Convert collections to lists for easier comparison
            var collectionListClean = observableCollectionClean.ToList();
            var collectionListDirty = observableCollectionDirty.ToList();

            // Get primary key property
            var primaryKey = GetPrimaryKeyProperty();

            // Compare properties for items that exist in both collections
            foreach (var cleanItem in collectionListClean)
            {
                var dirtyItem = collectionListDirty.FirstOrDefault(x => ArePrimaryKeysEqual(x, cleanItem, primaryKey));
                if (dirtyItem != null && !AreObjectsEqual(cleanItem, dirtyItem))  // Only when they are not equal
                {
                    dirtyItem.Action = "Update";  // Mark as Update
                    differences.Add(dirtyItem);
                }
            }

            // Determine which items were added to CollectionDirtySet
            var addedItems = collectionListDirty.Except(collectionListClean, new ObjectPrimaryKeyComparer(this, primaryKey)).ToList();
            if (addedItems.Any())
            {
                foreach (var item in addedItems)
                {
                    item.Action = "Insert";  // Mark as Insert
                    differences.Add(item);
                }
            }

            // Determine which items were removed from CollectionCleanSet
            var removedItems = collectionListClean.Except(collectionListDirty, new ObjectPrimaryKeyComparer(this, primaryKey)).ToList();
            if (removedItems.Any())
            {
                foreach (var item in removedItems)
                {
                    item.Action = "Delete";  // Mark as Delete
                    differences.Add(item);
                }
            }

            return differences;
        }

        // Helper Methods

        // Method to compare primary keys dynamically
        private bool ArePrimaryKeysEqual(T item1, T item2, PropertyInfo primaryKey)
        {
            if (primaryKey == null)
                throw new InvalidOperationException("Primary key property not found.");

            var key1 = primaryKey.GetValue(item1);
            var key2 = primaryKey.GetValue(item2);

            return key1 != null && key1.Equals(key2);
        }

        // Method to compare all properties of two objects
        private bool AreObjectsEqual(T item1, T item2)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value1 = property.GetValue(item1);
                var value2 = property.GetValue(item2);

                if (!Equals(value1, value2))
                {
                    return false;  // Property value differs, objects are not equal
                }
            }
            return true;  // All properties are equal
        }

        // Method to get the first property as primary key property info
        private PropertyInfo GetPrimaryKeyProperty()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var primaryKey = properties.FirstOrDefault();
            if (primaryKey == null)
                throw new InvalidOperationException("Primary key property not found.");
            return primaryKey;
        }

        // Nested class for comparing primary keys
        private class ObjectPrimaryKeyComparer : IEqualityComparer<T>
        {
            private readonly DataProcessor<T> _dataProcessor;
            private readonly PropertyInfo _primaryKey;

            public ObjectPrimaryKeyComparer(DataProcessor<T> dataProcessor, PropertyInfo primaryKey)
            {
                _dataProcessor = dataProcessor;
                _primaryKey = primaryKey;
            }

            public bool Equals(T x, T y)
            {
                return _dataProcessor.ArePrimaryKeysEqual(x, y, _primaryKey);
            }

            public int GetHashCode(T obj)
            {
                if (_primaryKey == null)
                    throw new InvalidOperationException("Primary key property not found.");
                var key = _primaryKey.GetValue(obj);
                return key != null ? key.GetHashCode() : 0;
            }
        }
    }
}