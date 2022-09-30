using Rg.Plugins.Popup.Pages;
using ScanditSbx.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace ScanditSbx.Views
{
    public partial class PopupResultPage : PopupPage
    {
        public PopupResultPage()
        {
            InitializeComponent();
        }

        private void RemoveElement(object sender, System.EventArgs e)
        {
            int data = System.Convert.ToInt32(((Button)sender).BindingContext);
            var model = this.BindingContext as ScannerViewModel;
            model?.RemoveFromAdditiveScanResults(System.Convert.ToInt32(data));
        }

        private async void AddButtonClicked(object sender, System.EventArgs e)
        {
            //var navigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            //navigationService.NavigateBackAsync();
            await Navigation.PopPopupAsync(true);
        }

        private async void SearchButtonClicked(object sender, System.EventArgs e)
        {
            // Search Here
            await Navigation.PopPopupAsync(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
    }
}