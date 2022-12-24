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

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
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


    }
}