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
        // Method to compare primary keys dynamically (handles 1 or 2 primary keys)
        private bool ArePrimaryKeysEqual(T item1, T item2, PropertyInfo[] primaryKeys)
        {
            if (primaryKeys == null || primaryKeys.Length == 0)
                throw new InvalidOperationException("Primary key properties not found.");

            foreach (var primaryKey in primaryKeys)
            {
                var key1 = primaryKey.GetValue(item1);
                var key2 = primaryKey.GetValue(item2);

                // If any primary key doesn't match, return false
                if (key1 == null || !key1.Equals(key2))
                {
                    return false;
                }
            }

            return true;
        }

        // Modified method to get one or two primary key property info
        private PropertyInfo[] GetPrimaryKeyProperties()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Assume that the primary key(s) are the first or second properties for now.
            // You can modify this to use a specific attribute or naming convention.
            var primaryKeys = properties.Take(2).ToArray(); // Get first or first two properties

            if (primaryKeys.Length == 0)
                throw new InvalidOperationException("Primary key properties not found.");

            return primaryKeys;
        }

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

            // Get primary key properties (supporting 1 or 2 primary keys)
            var primaryKeys = GetPrimaryKeyProperties();

            // Compare properties for items that exist in both collections
            foreach (var cleanItem in collectionListClean)
            {
                var dirtyItem = collectionListDirty.FirstOrDefault(x => ArePrimaryKeysEqual(x, cleanItem, primaryKeys));

                if (dirtyItem != null && !AreObjectsEqual(cleanItem, dirtyItem))  // Only when they are not equal
                {
                    dirtyItem.Action = "Update";  // Mark as Update
                    differences.Add(dirtyItem);
                }
            }

            // Determine which items were added to CollectionDirtySet
            var addedItems = collectionListDirty.Except(collectionListClean, new ObjectPrimaryKeyComparer(this, primaryKeys)).ToList();
            if (addedItems.Any())
            {
                foreach (var item in addedItems)
                {
                    item.Action = "Insert";  // Mark as Insert
                    differences.Add(item);
                }
            }

            // Determine which items were removed from CollectionCleanSet
            var removedItems = collectionListClean.Except(collectionListDirty, new ObjectPrimaryKeyComparer(this, primaryKeys)).ToList();
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

        // Nested class for comparing primary keys (updated for multiple keys)
        private class ObjectPrimaryKeyComparer : IEqualityComparer<T>
        {
            private readonly DataProcessor<T> _dataProcessor;
            private readonly PropertyInfo[] _primaryKeys;

            public ObjectPrimaryKeyComparer(DataProcessor<T> dataProcessor, PropertyInfo[] primaryKeys)
            {
                _dataProcessor = dataProcessor;
                _primaryKeys = primaryKeys;
            }

            public bool Equals(T x, T y)
            {
                return _dataProcessor.ArePrimaryKeysEqual(x, y, _primaryKeys);
            }

            public int GetHashCode(T obj)
            {
                if (_primaryKeys == null || _primaryKeys.Length == 0)
                    throw new InvalidOperationException("Primary key properties not found.");

                int hashCode = 17;
                foreach (var primaryKey in _primaryKeys)
                {
                    var key = primaryKey.GetValue(obj);
                    hashCode = hashCode * 23 + (key != null ? key.GetHashCode() : 0);
                }
                return hashCode;
            }

        }
        private bool AreObjectsEqual(T item1, T item2)
        {
            // Check if both items are the same reference
            if (ReferenceEquals(item1, item2))
                return true;

            // Check if either item is null
            if (item1 == null || item2 == null)
                return false;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value1 = property.GetValue(item1);
                var value2 = property.GetValue(item2);

                if (property.Name.Equals("IsSelected", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip this property
                }

                // Log or Debug the property names and values
                //Debug.WriteLine($"Comparing Property: {property.Name}, Value1: {value1}, Value2: {value2}");

                if (!Equals(value1, value2))
                {
                    // for testing
                    Debug.WriteLine($"Property '{property.Name}' differs. Objects are not equal.");
                    return false;  // Property value differs, objects are not equal
                }
            }

            // If all properties are equal
            return true;
        }

    }
}
    

/* OLD CODE
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

          
            foreach (var cleanItem in collectionListClean)
            {
                // Log the clean item details
                LogProductModel(cleanItem, "Clean Item");

                // Log and debug the primary key comparison
                var dirtyItem = collectionListDirty.FirstOrDefault(x => ArePrimaryKeysEqual(x, cleanItem, primaryKey));

                if (dirtyItem != null && !AreObjectsEqual(cleanItem, dirtyItem))  // Only when they are not equal
                {
                    dirtyItem.Action = "Update";  // Mark as Update
                    differences.Add(dirtyItem);
                }

                if (dirtyItem != null)
                {
                    // Log the dirty item details if found
                    LogProductModel(dirtyItem, "Dirty Item");
                }
                else
                {
                    //Debug.WriteLine("No matching dirty item found for clean item.");
                }
            }

            void LogProductModel(T item, string itemName)
            {
                //Debug.WriteLine($"Logging {itemName}:");

                foreach (var prop in item.GetType().GetProperties())
                {
                    var value = prop.GetValue(item, null) ?? "null";
                    //Debug.WriteLine($"{prop.Name}: {value}");
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
            // Check if both items are the same reference
            if (ReferenceEquals(item1, item2))
                return true;

            // Check if either item is null
            if (item1 == null || item2 == null)
                return false;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value1 = property.GetValue(item1);
                var value2 = property.GetValue(item2);

                if (property.Name.Equals("IsSelected", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip this property
                }



                // Log or Debug the property names and values
                //Debug.WriteLine($"Comparing Property: {property.Name}, Value1: {value1}, Value2: {value2}");

                if (!Equals(value1, value2))
                {
                    Debug.WriteLine($"Property '{property.Name}' differs. Objects are not equal.");
                    return false;  // Property value differs, objects are not equal
                }
            }

            // If all properties are equal
            return true;
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

*/