using VMSales.Models;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace VMSales.ViewModels
{
    public class SupplierViewModel : BaseViewModel
    {
        private SupplierModel select_request;
        public SupplierModel Select_Request
        {
            get { return select_request; }
            set
            {
                select_request = value;
                RaisePropertyChanged("Select_Request");
            }
        }
        private ObservableCollection<SupplierModel> ObservableCollectionSupplierModelclean { get; set; }
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel
        {
            get { return ObservableCollectionSupplierModelclean; }
            set
            {
                if (ObservableCollectionSupplierModelclean == value) return;
                ObservableCollectionSupplierModelclean = value;
                RaisePropertyChanged("ObservableCollectionSupplierModel");
            }
        }

        IDatabaseProvider dataBaseProvider;

        //Commands
        public void SaveCommand()
        {
            // set db value to null until checked
            String db_supplier_pk = null;
            // nothing selected
            if (Select_Request == null)
            {
                MessageBox.Show("No Changes Were Made.");
                return;
            }
            if (Select_Request.supplier_name.ToString() == "" 
                || Select_Request.address.ToString() == "" 
                || Select_Request.city.ToString() == "" 
                || Select_Request.state.ToString() == "" 
                || Select_Request.country.ToString() == "" 
                || Select_Request.zip.ToString() == "" 
                || Select_Request.phone.ToString() == "")
            {
                MessageBox.Show("Only Email can be blank.");
                return;
            }
            // all values good, now we must update or insert, get primary key.
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            try
            {
                db_supplier_pk = SupplierRepo.Get(select_request.supplier_pk).Result.supplier_pk.ToString();
            }
            catch (AggregateException e) // primary key does not exist
            {
                // insert
                Task<bool> insertSupplier = SupplierRepo.Insert(Select_Request);
                if (insertSupplier.Result == true)
                {
                    SupplierRepo.Commit();
                    SupplierRepo.Dispose();
                }
                else
                {
                    MessageBox.Show("An Error has occured with inserting.  Insertion Rejected");
                    SupplierRepo.Revert();
                    SupplierRepo.Dispose();
                }
                return;
            }
            catch (Exception e)
            {  // any other error
                MessageBox.Show(e.ToString());
                SupplierRepo.Revert();
                SupplierRepo.Dispose();
                return;
            }
            // if id match UPDATE
            if (db_supplier_pk == select_request.supplier_pk.ToString())
            {
                Task<bool> updateCategory = SupplierRepo.Update(Select_Request);
                if (updateCategory.Result == true)
                {
                    SupplierRepo.Commit();
                    SupplierRepo.Dispose();
                    MessageBox.Show("Saved");
                }
                else
                {
                    MessageBox.Show("An Error has occured with Updating.  Updating Rejected");
                    SupplierRepo.Revert();
                    SupplierRepo.Dispose();
                }
                return;
            }
        }

        public void ResetCommand()
        {
        }
        public void AddCommand()
        {
            var obj = new SupplierModel()
            {
                supplier_pk = 0,
                supplier_name = "",
                address = "",
                city = "",
                state = "",
                country = "",
                zip = "",
                phone = "",
                email = ""

            };
            ObservableCollectionSupplierModel.Add(obj);
            RaisePropertyChanged("ObservableCollectionSupplierModel");
        }
        public void DeleteCommand()
        {
            // nothing selected
            if (Select_Request == null)
            {
                MessageBox.Show("No Changes Were Made.");
                return;
            }
            // remove from observable only if selected 0
            if (Select_Request.supplier_pk.ToString() == "0")
            {
                //remove code to be here
                return;
            }
            // set db value to null until checked
            String db_supplier_pk = null;

            // get primary key.
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            try
            {
                db_supplier_pk = SupplierRepo.Get(select_request.supplier_pk).Result.supplier_pk.ToString();
            }
            catch (Exception e)
            {  // catch all errors
                MessageBox.Show(e.ToString());
                SupplierRepo.Revert();
                SupplierRepo.Dispose();
                return;
            }
            // if id match DELETE
            if (db_supplier_pk == select_request.supplier_pk.ToString())
            {
                Task<bool> deleteCategory = SupplierRepo.Delete(Select_Request);
                if (deleteCategory.Result == true)
                {
                    SupplierRepo.Commit();
                    SupplierRepo.Dispose();
                    MessageBox.Show("Row Deleted");
                    // we are not refreshing for delete. we need to remove it from observable and refresh.
                }
                else
                {
                    MessageBox.Show("An Error has occured with Deleting.  Rejected");
                    SupplierRepo.Revert();
                    SupplierRepo.Dispose();
                }
                return;
            }
        }
        public SupplierViewModel()
        {
            ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
            dataBaseProvider = BaseViewModel.getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
            ObservableCollectionSupplierModelclean = ObservableCollectionSupplierModel;
            SupplierRepo.Commit();
            SupplierRepo.Dispose();
        }
    }
}