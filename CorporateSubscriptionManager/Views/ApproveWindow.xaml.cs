using System;
using System.Windows;

namespace CorporateSubscriptionManager.Views
{
    public partial class ApproveWindow : Window
    {
        public DateTime? SelectedStartDate { get; set; }

        public ApproveWindow()
        {
            InitializeComponent();
            SelectedStartDate = DateTime.Now.Date;
            StartDatePicker.SelectedDate = SelectedStartDate;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedStartDate = StartDatePicker.SelectedDate ?? DateTime.Now.Date;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}