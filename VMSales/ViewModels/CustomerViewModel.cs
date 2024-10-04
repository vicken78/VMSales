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
                //RaisePropertyChanged("ObservableCollectionCustomerModel");
            }
        }
      
        private CustomerModel select_request;
        public CustomerModel Select_Request
        {
            get { return select_request; }
            set
            {
                select_request = value;
                //RaisePropertyChanged("Select_Request");
            }
        }

        IDatabaseProvider dataBaseProvider;
        public async void SaveCommand()
        {
            if (Select_Request == null)
            {
                MessageBox.Show("No changes were made.");
                return;
            }

            try
            {
                var dataBaseProvider = getprovider();
                var CustomerRepo = new CustomerRepository(dataBaseProvider);

                // Create a new instance of CustomerRepository using the database provider

                // Try to get the primary key

                int cust_pk = await CustomerRepo.Get_cust_pk(select_request.customer_pk);
                // cust_pk will be 0 if no matching rows are found
                if (cust_pk == 0)
                {
                    // Insert
                    int insertResult = await CustomerRepo.Insert(Select_Request);
                    if (insertResult > 0)
                    {
                        MessageBox.Show("Saved.");
                        CustomerRepo.Commit();
                        CustomerRepo.Dispose();

                    }
                    else
                    {
                        MessageBox.Show("An error has occurred with inserting. Insertion rejected.");
                        CustomerRepo.Revert();
                        CustomerRepo.Dispose();
                    }
                }
                else if (cust_pk == select_request.customer_pk)
                {
                    // Update
                    bool updateResult = await CustomerRepo.Update(Select_Request);
                    if (updateResult)
                    {
                        MessageBox.Show("Saved.");
                        CustomerRepo.Commit();
                        CustomerRepo.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("An error has occurred with updating. Updating rejected.");
                        CustomerRepo.Revert();
                        CustomerRepo.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("An error has occurred: primary key mismatch.");
                }
                initial_load();
            }
            catch (Exception) { }
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
                shipping_city = null,
                shipping_state = null,
                shipping_zip = null,
                shipping_country = null,
                same_shipping_address = 0
            };
            ObservableCollectionCustomerModel.Add(obj);
            //RaisePropertyChanged("ObservableCollectionCustomerModel");
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
            CustomerRepository CustomerRepo = new CustomerRepository(dataBaseProvider);

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
                Select_Request = new CustomerModel();
                dataBaseProvider = getprovider();
                CustomerRepository CustomerRepo = new CustomerRepository(dataBaseProvider);
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