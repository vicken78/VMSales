using VMSales.Models;
using VMSales.Logic;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Threading.Tasks;

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
            String cust_pk;
            if (Select_Request == null)
            {
                MessageBox.Show("No Changes Were Made.");
                return;
            }

            // update or insert, attempt to get primary key.
            dataBaseProvider = getprovider();
            DataBaseLayer.CustomerRepository CustomerRepo = new DataBaseLayer.CustomerRepository(dataBaseProvider);
            try
            {
                cust_pk = CustomerRepo.Get(select_request.customer_pk).Result.ToString();
            }
            catch (AggregateException e) // primary key does not exist
            {
                // insert
                Task<bool> insertCustomer = CustomerRepo.Insert(Select_Request);
                if (insertCustomer.Result == true)
                {
                    CustomerRepo.Commit();
                    CustomerRepo.Dispose();
                }
                else
                {
                    MessageBox.Show("An Error has occured with inserting.  Insertion Rejected");
                    CustomerRepo.Revert();
                    CustomerRepo.Dispose();
                }
                return;
            }
            catch (Exception e)
            {  // any other error
                MessageBox.Show(e.ToString());
                CustomerRepo.Revert();
                CustomerRepo.Dispose();
                return;
            }
            // if id match UPDATE
            if (cust_pk == select_request.customer_pk.ToString())
            {
                Task<bool> updateCategory = CustomerRepo.Update(Select_Request);
                if (updateCategory.Result == true)
                {
                    CustomerRepo.Commit();
                    CustomerRepo.Dispose();
                    MessageBox.Show("Saved");
                }
                else
                {
                    MessageBox.Show("An Error has occured with Updating.  Updating Rejected");
                    CustomerRepo.Revert();
                    CustomerRepo.Dispose();
                }
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
            String cust_pk;
            dataBaseProvider = getprovider();
            DataBaseLayer.CustomerRepository CustomerRepo = new DataBaseLayer.CustomerRepository(dataBaseProvider);

            // get primary key.
            try
            {
                cust_pk = CustomerRepo.Get(select_request.customer_pk).Result.customer_pk.ToString();
                CustomerRepo.Revert();
                CustomerRepo.Dispose();
            }
            catch (Exception e)
            {  // catch all errors
                MessageBox.Show(e.ToString());
                CustomerRepo.Revert();
                CustomerRepo.Dispose();
                return;
            }
            // if id match DELETE
            if (cust_pk == select_request.customer_pk.ToString())
            {
            Task<bool> deleteCustomer = CustomerRepo.Delete(Select_Request);
            if (deleteCustomer.Result == true)
            {
                CustomerRepo.Commit();
                CustomerRepo.Dispose();
                MessageBox.Show("Row Deleted");
                initial_load();
            }
            else
            {
            MessageBox.Show("An Error has occured with Deleting.  Rejected");
            CustomerRepo.Revert();
            CustomerRepo.Dispose();
            }
            return;
            }
        }
        public void initial_load()
        {
            ObservableCollectionCustomerModel = new ObservableCollection<CustomerModel>();
            try
            {
                dataBaseProvider = getprovider();
                DataBaseLayer.CustomerRepository CustomerRepo = new DataBaseLayer.CustomerRepository(dataBaseProvider);
                ObservableCollectionCustomerModel = CustomerRepo.GetAll().Result.ToObservable();
                CustomerRepo.Commit();
                CustomerRepo.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("An Expected error has occured. " + e);
            }
        }
        public CustomerViewModel()
        {
            initial_load();
        }
    }
}