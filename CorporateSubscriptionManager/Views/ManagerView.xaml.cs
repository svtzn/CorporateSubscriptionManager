using System.Windows.Controls;

namespace CorporateSubscriptionManager.Views
{
    public partial class ManagerView : UserControl
    {
        public ManagerView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ManagerViewModel();
        }
    }
}