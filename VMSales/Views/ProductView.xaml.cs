using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VMSales.Views
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {

        private void NumericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public ProductView()
        {
            InitializeComponent();
        }
    }
}
