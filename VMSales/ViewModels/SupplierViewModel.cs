using Caliburn.Micro;
using VMSales.Models;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using VMSales.ChangeTrack;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Data;
using VMSales.BaseType;
using VMSales.Database;
namespace VMSales.ViewModels
{
    public class SupplierViewModel {
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }
        public ChangeTracker<SupplierModel> changetracker { get; set; }
        //ObservableCollection<Data> Rows;
        //ChangeTrack<Data> changeTrack = new ChangeTrack<Data>();

        //Commands
        public void SaveCommand()
        {
            var changetracker = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            changetracker.StartTracking(ObservableCollectionSupplierModel);

            List<SupplierModel> Saved;
           Saved = changetracker.RowsUpdated;
           int count = Saved.Count;
            MessageBox.Show("Saved");
            MessageBox.Show(count.ToString());

        }
        public void DeleteCommand()
        {
            List<SupplierModel> Deleted;
            //var ChangeTrack = new ChangeTracker<SupplierModel>();
            //changeTrack = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            //changeTrack.StartTracking(ObservableCollectionSupplierModel);
            //Deleted = changeTrack.RowsDeleted;

            //for (var i = 0; i < Deleted.Count; i++)
            //{
            //    Debug.WriteLine(Deleted[i]);
           // }
            //int count = Deleted.Count;

            //MessageBox.Show("Delete");
            //MessageBox.Show(count.ToString());

        }

        public void AddCommand()
        {
            MessageBox.Show("Add");
        }

        public void ResetCommand()
        {
            MessageBox.Show("Reset");
        }


        public SupplierViewModel()
        {
            string sql = "SELECT * FROM supplier;";
            DataTable dt = Database.DataBaseOps.SQLiteDataTableWithQuery(sql);

            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();


            foreach (DataRow row in dt.Rows)
            {
                var obj = new SupplierModel()
                {
                    Supplier_pk = (string)row["supplier_pk"].ToString(),
                    Sname = (string)row["sname"],
                    Address = (string)row["address"],
                    City = (string)row["city"],
                    State = (string)row["state"],
                    Country = (string)row["country"],
                    Zip = (string)row["zip"],
                    Phone = (string)row["phone"],
                    Email = (string)row["email"]
                };
                ObservableCollectionSupplierModel.Add(obj);
            }



            // commands
            //    SaveCommand = new Commands();
            //    ResetCommand = new Commands();

            //  var test = new ObservableCollection<Test>();
            //  foreach (DataRow row in TestTable.Rows)
            // {
            //      test.Add(new Test()
            //      {
            //          id_test = (int)row["id_test"],
            //          name = (string)row["name"],
            //      });
            // }

            /*
            foreach (DataRow dataRow in dt.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    var obj = new SupplierModel()
                    {
                        supplier_pk = (string)dataRow["supplier_pk"].ToString(),
                        sname = (string)dataRow["sname"],
                        address = (string)dataRow["address"],
                        city = (string)dataRow["city"],
                        state = (string)dataRow["state"],
                        country = (string)dataRow["country"],
                        zip = (string)dataRow["zip"],
                        phone = (string)dataRow["phone"],
                        email = (string)dataRow["email"]

                    }



                    };

                    ObservableCollectionSupplierModel.Add(obj);
            */




            //}
            //  }


            var changetracker = new ChangeTracker<SupplierModel>(ObservableCollectionSupplierModel);
            changetracker.StartTracking(ObservableCollectionSupplierModel);
        
           



            //  var SupplierView = CollectionViewSource.GetDefaultView(Suppliers) as ListCollectionView;

            // Suppliers = this.listsupplier; 
            //as ObservableCollection<SupplierModel>;



            //Debug.WriteLine(listsupplier.sname);


            //  SupplierView.CurrentChanged += (s, e) =>
            //  {
            //       RaisePropertyChanged(() => SupplierModel);
            //  };
        }
    }
}
            //SuppliersView.CurrentChanged += (s, e) =>
            //{
            //RaisePropertyChanged(() => SupplierModel);
            //};
            //SuppliersView.SortDescriptions.Clear();
            //SuppliersView.SortDescriptions.Add(new SortDescription(nameof(PersonModel.Firstname), ListSortDirection.Ascending));
            //foreach (var item in Persons)
            //{
            //item.PropertyChanged += PersonsOnPropertyChanged;
            //}
//Persons.CollectionChanged += (s, e) =>
//{
  //  if (e.NewItems != null)
   // {
     //   foreach (INotifyPropertyChanged added in e.NewItems)
      //  {
       //     added.PropertyChanged += PersonsOnPropertyChanged;
    //    }
   // }
   // if (e.OldItems != null)
    //{
     //   foreach (INotifyPropertyChanged removed in e.OldItems)
      //  {
       //     removed.PropertyChanged -= PersonsOnPropertyChanged;
    //    }
   // }
//};
//OpenChildCommand = new RelayCommand(() => MessengerInstance.Send(new OpenChildWindowMessage("Hello Child!")));
//SetSomeDateCommand = new RelayCommand<PersonModel>(person => person.Birthday = DateTime.Now.AddYears(-20));
//AddPersonCommand = new RelayCommand(
  //  () =>
   // {
     //   var newPerson = new PersonModel
      //  {
       //     Firstname = "Z(Firstname)"
       // };
        //Persons.Add(newPerson);
        //PersonModel = newPerson;
   // });
    //        }
     //   }

      //  #endregion

       // #region methods

      //  /// <summary>
      //  /// Event handler for property changes on elements of <see cref="Persons"/>.
        /// </summary>
        /// <param name="sender">The person model.</param>
        /// <param name="e">The event arguments.</param>
    //    private void SuppliersOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //if (e.PropertyName == nameof(SupplierModel.HasErrors) || e.PropertyName == nameof(PersonModel.IsOk))
        //{
        //return;
        //}
   // if (SuppliersView.IsEditingItem || PersonsView.IsAddingNew)
   // {
   //     return;
   // }
   // PersonsView.Refresh();
//}

//#endregion

//#region properties

/// <summary>
/// Adds a new person to the <see cref="Persons"/>.
/// </summary>
//public RelayCommand AddSupplierCommand { get; }

/// <summary>
/// Opens a new child window.
/// </summary>
//public RelayCommand OpenChildCommand { get; }

/// <summary>
/// A person to edit.
/// </summary>
//public SupplierModel SupplierModel
//{
   // get => SuppliersView.CurrentItem as SupplierModel;
   // set
   // {
   //     SuppliersView.MoveCurrentTo(value);
  //      RaisePropertyChanged();
 //   }
//}

/// <summary>
/// The view for binding the UI against the <see cref="Persons"/>.
/// </summary>
//public ListCollectionView PersonsView { get; }


/// <summary>
/// Adds a birthday to a person.
/// </summary>
//public RelayCommand<PersonModel> SetSomeDateCommand { get; }

/// <summary>
/// The list of persons.
/// </summary>
//private ObservableCollection<PersonModel> Persons { get; }

  //  #endregion




 
            

       /*     foreach (var item in listsupplier)
             {
                 Debug.WriteLine(item);
             }
            SupplierModel sm = new SupplierModel();
            //sm.Datasource = listsupplier;

            // this is supplier view model
              foreach (SupplierModel per in listsupplier)
             {
                string state = per.state;
                sm.sname = per.sname;
                sm.address = per.address;
                sm.city = per.city;
                sm.state = per.state;
                sm.email = per.email;
                sm.phone = per.phone;
                sm.country = per.country;
               Debug.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7}", per.sname, per.address, per.city, per.state, per.zip, per.country, per.phone, per.email);
             }
       
*/

/*

namespace codingfreaks.blogsamples.MvvmSample.Logic.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

    //using BaseTypes;

    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Threading;

    using Messages;

    //using Models;

    /// <summary>
    /// Contains logic for the main view of the UI.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
                var personList = new List<PersonModel>();
                for (var i = 0; i < 10; i++)
                {
                    personList.Add(
                        new PersonModel
                        {
                            Firstname = Guid.NewGuid().ToString("N").Substring(0, 10),
                            Lastname = Guid.NewGuid().ToString("N").Substring(0, 10)
                        });
                }
                Persons = new ObservableCollection<PersonModel>(personList);
                PersonsView = CollectionViewSource.GetDefaultView(Persons) as ListCollectionView;
                PersonsView.CurrentChanged += (s, e) =>
                {
                    RaisePropertyChanged(() => PersonModel);
                };
                PersonsView.SortDescriptions.Clear();
                PersonsView.SortDescriptions.Add(new SortDescription(nameof(PersonModel.Firstname), ListSortDirection.Ascending));
                foreach (var item in Persons)
                {
                    item.PropertyChanged += PersonsOnPropertyChanged;
                }
                Persons.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (INotifyPropertyChanged added in e.NewItems)
                        {
                            added.PropertyChanged += PersonsOnPropertyChanged;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (INotifyPropertyChanged removed in e.OldItems)
                        {
                            removed.PropertyChanged -= PersonsOnPropertyChanged;
                        }
                    }
                };
                OpenChildCommand = new RelayCommand(() => MessengerInstance.Send(new OpenChildWindowMessage("Hello Child!")));
                SetSomeDateCommand = new RelayCommand<PersonModel>(person => person.Birthday = DateTime.Now.AddYears(-20));
                AddPersonCommand = new RelayCommand(
                    () =>
                    {
                        var newPerson = new PersonModel
                        {
                            Firstname = "Z(Firstname)"
                        };
                        Persons.Add(newPerson);
                        PersonModel = newPerson;
                    });
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Event handler for property changes on elements of <see cref="Persons"/>.
        /// </summary>
        /// <param name="sender">The person model.</param>
        /// <param name="e">The event arguments.</param>
        private void PersonsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PersonModel.HasErrors) || e.PropertyName == nameof(PersonModel.IsOk))
            {
                return;
            }
            if (PersonsView.IsEditingItem || PersonsView.IsAddingNew)
            {
                return;
            }
            PersonsView.Refresh();
        }

        #endregion

        #region properties

        /// <summary>
        /// Adds a new person to the <see cref="Persons"/>.
        /// </summary>
        public RelayCommand AddPersonCommand { get; }

        /// <summary>
        /// Opens a new child window.
        /// </summary>
        public RelayCommand OpenChildCommand { get; }

        /// <summary>
        /// A person to edit.
        /// </summary>
        public PersonModel PersonModel
        {
            get => PersonsView.CurrentItem as PersonModel;
            set
            {
                PersonsView.MoveCurrentTo(value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The view for binding the UI against the <see cref="Persons"/>.
        /// </summary>
        public ListCollectionView PersonsView { get; }

        /// <summary>
        /// Indicates the progress.
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Adds a birthday to a person.
        /// </summary>
        public RelayCommand<PersonModel> SetSomeDateCommand { get; }

        /// <summary>
        /// The list of persons.
        /// </summary>
        private ObservableCollection<PersonModel> Persons { get; }

        #endregion
    }
}*/