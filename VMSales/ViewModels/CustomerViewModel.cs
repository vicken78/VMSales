using VMSales.Models;
using VMSales.Logic;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace VMSales.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private ObservableCollection<CustomerModel> _ObservableCollectionCustomerModel { get; set; }
        public ObservableCollection<CustomerModel> ObservableCollectionCustomerModel
        {
            get { return _ObservableCollectionCustomerModel; }
            set
            {
                if (_ObservableCollectionCustomerModel == value) return;
                _ObservableCollectionCustomerModel = value;
                RaisePropertyChanged("ObservableCollectionCustomerModel");
            }
        }
        private int _use_same_address { get; set; }
        public int use_same_address
        {
            get { return _use_same_address; }
            set
            {
                if (_use_same_address == value) return;
                _use_same_address = value;
                RaisePropertyChanged("use_same_address");
                }
            }

        private CustomerModel select_request;
        public CustomerModel Select_Request
        {
            get { return select_request; }
            set
            {
                select_request = value;
                RaisePropertyChanged("Select_Request");
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


            // all values good, now we must update or insert, get primary key.
            dataBaseProvider = getprovider();
            //DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            try
            {
                //db_supplier_pk = SupplierRepo.Get(select_request.customer_pk).Result.supplier_pk.ToString();
            }
            catch (AggregateException e) // primary key does not exist
            {
                // insert
                try
                {
                    //Task<bool> insertSupplier = SupplierRepo.Insert(Select_Request);
                    //if (insertSupplier.Result == true)
                    //{
                    //SupplierRepo.Commit();
                    //SupplierRepo.Dispose();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error has occured with inserting.  Insertion Rejected" + ex);
                    //SupplierRepo.Revert();
                    //SupplierRepo.Dispose();
                }
                return;
            }
            // if id match UPDATE
            if (db_supplier_pk == select_request.customer_pk.ToString())
            {
                //Task<bool> updateCategory = SupplierRepo.Update(Select_Request);
                //if (updateCategory.Result == true)
                //{
                //SupplierRepo.Commit();
                //SupplierRepo.Dispose();
                //MessageBox.Show("Saved");
                //}
                //else
                //{
                //MessageBox.Show("An Error has occured with Updating.  Updating Rejected");
                //SupplierRepo.Revert();
                //SupplierRepo.Dispose();
                //}
                return;
            }
        }

        public void ResetCommand()
        {
            initial_load();
        }
        public void AddCommand()
        {
            var obj = new CustomerModel()
            {
                customer_pk = 0,
                user_name = null,
                first_name = null,
                last_name = null,
                address = null,
                state = null,
                city = null,
                zip = null,
                country = null,
                phone = null,
                shipping_address = null,
                shipping_state = null,
                shipping_zip = null,
                shipping_country = null,
                use_same_address = 0
            };
            ObservableCollectionCustomerModel.Add(obj);
            RaisePropertyChanged("ObservableCollectionCustomerModel");
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
            if (Select_Request.customer_pk.ToString() == "0")
            {
                //remove code to be here
                return;
            }
            // set db value to null until checked
            String db_supplier_pk = null;

            // get primary key.
            dataBaseProvider = getprovider();
            //DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            try
            {
                //db_supplier_pk = SupplierRepo.Get(select_request.customer_pk).Result.supplier_pk.ToString();
            }
            catch (Exception e)
            {  // catch all errors
                MessageBox.Show(e.ToString());
                //SupplierRepo.Revert();
                //SupplierRepo.Dispose();
                return;
            }
            // if id match DELETE
            //if (db_supplier_pk == select_request.customer_pk.ToString())
            //{
            //Task<bool> deleteCategory = SupplierRepo.Delete(Select_Request);
            //if (deleteCategory.Result == true)
            //{
            //SupplierRepo.Commit();
            //SupplierRepo.Dispose();
            //MessageBox.Show("Row Deleted");
            // we are not refreshing for delete. we need to remove it from observable and refresh.
            //}
            //else
            //{
            //MessageBox.Show("An Error has occured with Deleting.  Rejected");
            //SupplierRepo.Revert();
            //SupplierRepo.Dispose();
            //}
            //return;
            //}
        }
        public void initial_load()
        {
            ObservableCollectionCustomerModel = new ObservableCollection<CustomerModel>();
        }
        public CustomerViewModel()
        {
            initial_load();
        }
    }
}
