using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Linq;

namespace VMSales.ChangeTrack
{
    public class ChangeTracker<DataRow> : IDisposable where DataRow : INotifyPropertyChanged
    {
        /// <summary>
        /// The generic change tracking class.  "DataRow" is the type (class) of the rows in the
        /// observable collection.  The row class must implement INotifyPropertyChanged
        /// </summary>
        /// <typeparam name="DataRow">data Type of the data rows</typeparam>

        // reference to the items collection being tracked
        private ObservableCollection<DataRow> Items;

        /// <summary>
        /// HashSet is used because it silently ignores duplicate adds
        /// </summary>
        private HashSet<DataRow> Created;
        private HashSet<DataRow> Updated;
        private HashSet<DataRow> Deleted;
        /// <summary>
        /// returns true if there are any changes.  Can be used as the
        /// enabler for a RoutedCommand to save the changes
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return ((Created.Count + Updated.Count + Deleted.Count) > 0);
            }
        }
        /// <summary>
        /// returns list of rows added since StartTracking or ClearTracking was called.
        /// </summary>
        public List<DataRow> RowsCreated
        {
            get
            {
                if (Created.Count == 0) return new List<DataRow>();
                return (List<DataRow>)Created.ToList();
            }
        }

        /// <summary>
        /// returns list of rows changed since StartTracking or ClearTracking was called.
        /// </summary>
        public List<DataRow> RowsUpdated
        {
            get
            {
                Debug.WriteLine(Updated.Count.ToString());
                if (Updated.Count == 0) return new List<DataRow>();
                return (List<DataRow>)Updated.ToList();
            }
        }

        /// <summary>
        /// returns list of rows deleted since StartTracking or ClearTracking was called.
        /// </summary>
        public List<DataRow> RowsDeleted
        {
            get
            {
                if (Deleted.Count == 0) return new List<DataRow>();
                return (List<DataRow>)Deleted.ToList();
            }
        }

        /// <summary>
        /// Default constructor.  Follow with StartTracking after observable collection is
        /// created and populated with intial data from data store.
        /// </summary>
        public ChangeTracker() { }

        /// <summary>
        /// Constructor to start tracking immediately
        /// </summary>
        /// <param name="observableCollection">collection to be tracked</param>
        public ChangeTracker(ObservableCollection<DataRow> observableCollection)
        {
            Contract.Requires(observableCollection != null);

            StartTracking(observableCollection);
        }

        /// <summary>
        /// Begin tracking changes to the observable collection. 
        /// </summary>
        /// <param name="observableCollection"></param>
        public void StartTracking(ObservableCollection<DataRow> observableCollection)
        {
            Contract.Requires(observableCollection != null);

            Created = new HashSet<DataRow>();
            Updated = new HashSet<DataRow>();
            Deleted = new HashSet<DataRow>();
            Items = observableCollection;

            // register events
            Items.CollectionChanged += CollectionChanged;
            foreach (DataRow row in Items)
            {
                row.PropertyChanged += PropertyChanged;
            }
        }

        /// <summary>
        /// this event handler tracks changes to the data in existing rows
        /// in the observable collection.  If a row is created, it is
        /// not tracked, as it will be sent to the backing store as a created row
        /// </summary>
        /// <param name="sender">The changed row emitting the event</param>
        /// <param name="e"></param>
        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Contract.Requires(Updated != null);
            Contract.Requires(Created != null);

            Updated.Add((DataRow)sender);
        }

        /// <summary>
        /// This event handler tracks adds and deletes to the observable collection.
        /// note that moves are not tracked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (DataRow row in e.NewItems)
                    {
                        // there is no need to start tracking changes to the added item
                        Created.Add(row);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // first, add all the deleted rows to the list
                    foreach (var old in e.OldItems)
                    {
                        Deleted.Add((DataRow)old);
                    }

                    // if any items were created and then deleted, remove them from the deleted list
                    (from c in Created
                     from d in Deleted
                     where c.Equals(d)
                     select d).ToList().ForEach(x => Deleted.Remove(x));


                    // now delete any deleted items from the Created and Updated lists
                    foreach (var old in e.OldItems)
                    {
                        DataRow row = (DataRow)old;
                        row.PropertyChanged -= PropertyChanged;
                        Created.Remove(row);
                        Updated.Remove(row);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    ClearTracking();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// clear all the tracking records.  This would be called after all the changes
        /// were committed to the backing store (database).
        /// </summary>
        public void ClearTracking()
        {
            Contract.Requires(Created != null);
            Contract.Requires(Updated != null);
            Contract.Requires(Deleted != null);

            // from now on, need to track changes in previously added rows
            foreach (DataRow r in Created)
            {
                r.PropertyChanged += PropertyChanged;
            }
            Created.Clear();
            Updated.Clear();
            Deleted.Clear();
        }

        /// <summary>
        /// clear all tracking, and unregister all events
        /// </summary>
        public void Dispose()
            {
                Contract.Requires(Items != null);

                Created.Clear();
                Updated.Clear();
                Deleted.Clear();

                Items.CollectionChanged -= CollectionChanged;
                foreach (DataRow row in Items)
                {
                    row.PropertyChanged -= PropertyChanged;
                }
            }
    }
}
