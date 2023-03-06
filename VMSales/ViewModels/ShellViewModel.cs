using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;

namespace VMSales.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public async Task LoadCategory()
        {
            await ActivateItemAsync(new CategoryViewModel());
        }

        public async Task LoadPurchaseOrder()
        {
            await ActivateItemAsync(new PurchaseOrderViewModel());
        }
     
        public async Task LoadSupplier()
        {
            await ActivateItemAsync (new SupplierViewModel());
        }

        public async Task LoadProduct()
        {
            await ActivateItemAsync(new ProductViewModel());   
        }
        public async Task LoadCustomer()
        {
            await ActivateItemAsync(new CustomerViewModel());
        }

        public async Task LoadCustomerOrder()
        {
            await ActivateItemAsync(new CustomerOrderViewModel());
        }



        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }


    }
}