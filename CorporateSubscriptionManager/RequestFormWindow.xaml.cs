using System.Windows;

namespace CorporateSubscriptionManager
{
    public partial class RequestFormWindow : Window
    {
        public RequestFormWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.RequestViewModel(this); // Новый VM для формы
        }
    }
}