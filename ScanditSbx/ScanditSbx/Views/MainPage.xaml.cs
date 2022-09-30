using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using ScanditSbx.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScanditSbx.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ScannerViewModel viewModel;
        private bool Scanning;
        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel = this.BindingContext as ScannerViewModel;
            Scanning = true;
            this.Resources.Add("Scanning", Scanning);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = this.viewModel.OnResumeAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.viewModel.OnSleep();
        }

        private async void ButtonClicked(object sender, System.EventArgs e)
        {
            Scanning = false;

            // Page implementation
            var context = this.BindingContext;
            //var resultPage = new ResultsPage();
            //resultPage.BindingContext = context;

            //var navigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            //await navigationService.NavigateToAsync(resultPage);
            await Navigation.PushPopupAsync(new PopupResultPage()
            {
                BindingContext = context
            });

            //((NavigationPage)Application.Current.MainPage).PushAsync(resultPage);
        }
    }
}