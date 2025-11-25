using System.Windows.Controls;
using System.Windows;

namespace CorporateSubscriptionManager.Views
{
    public partial class AdminView : UserControl
    {
        private ViewModels.AdminViewModel _vm;
        public AdminView()
        {
            InitializeComponent();
            _vm = new ViewModels.AdminViewModel();
            DataContext = _vm;
        }

        private void RefreshReport_Click(object sender, RoutedEventArgs e)
        {
            _vm.LoadAll();
        }
    }
}