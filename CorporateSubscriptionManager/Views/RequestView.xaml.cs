using System;
using System.Windows.Controls;

namespace CorporateSubscriptionManager.Views
{
    public partial class RequestView : UserControl
    {
        public RequestView(Action onSubmitted = null, Action onCancelled = null)
        {
            InitializeComponent();
            var vm = new ViewModels.RequestViewModel();
            vm.OnSubmitted = onSubmitted;
            vm.OnCancelled = onCancelled;
            DataContext = vm;
        }
    }
}