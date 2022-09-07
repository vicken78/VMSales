using Caliburn.Micro;
using System.Windows;
using VMSales.ViewModels;

namespace VMSales
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }

    }
}
