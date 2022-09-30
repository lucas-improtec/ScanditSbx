using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Rg.Plugins.Popup.Extensions;
using Scandit.DataCapture.Barcode.Capture.Unified;
using Scandit.DataCapture.Barcode.Data.Unified;
using Scandit.DataCapture.Barcode.Tracking.Capture.Unified;
using Scandit.DataCapture.Barcode.Tracking.Data.Unified;
using Scandit.DataCapture.Barcode.Tracking.UI.Unified;
using Scandit.DataCapture.Core.Capture.Unified;
using Scandit.DataCapture.Core.Data.Unified;
using Scandit.DataCapture.Core.Source.Unified;
using ScanditSbx.Models;
using ScanditSbx.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Brush = Scandit.DataCapture.Core.UI.Style.Unified.Brush;

namespace ScanditSbx.ViewModels
{
    public class ScannerViewModel : BaseViewModel, IBarcodeTrackingListener, IBarcodeTrackingBasicOverlayListener, IBarcodeCaptureListener
    {
        private readonly Scandit.DataCapture.Core.UI.Style.Unified.Brush highlightedBrush;
        private readonly HashSet<ScanResult> scanResults = new HashSet<ScanResult>();

        public Camera Camera { get; private set; } = ScannerModel.Instance.CurrentCamera;
        public DataCaptureContext DataCaptureContext { get; private set; } = ScannerModel.Instance.DataCaptureContext;
        public BarcodeTracking BarcodeTracking { get; private set; } = ScannerModel.Instance.BarcodeTracking;
        public BarcodeCapture BarcodeCapture { get; private set; } = ScannerModel.Instance.BarcodeCapture;

        private bool showBarcodeCaptureLayout;
        public bool ShowBarcodeCaptureLayout
        {
            get => showBarcodeCaptureLayout;
            set
            {
                showBarcodeCaptureLayout = value;
                OnPropertyChanged();
            }
        }

        private bool showTrackingBarcodeLayout;
        public bool ShowTrackingBarcodeLayout
        {
            get => showBarcodeCaptureLayout;
            set
            {
                showBarcodeCaptureLayout = value;
                OnPropertyChanged();
            }
        }

        private bool _showScanner;

        public bool ShowScanner 
        {
            get => _showScanner;
            private set
            {
                _showScanner = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand ScanSimpleClicked { private set; get; }
        public ICommand ScanMultipleClicked { private set; get; }
        public ICommand CloseScanClicked { private set; get; }

        #endregion

        public ScannerViewModel()
        {
            this.InitializeScanner();
            this.SubscribeToAppMessages();
            ScannerModel.OnSettingMode += AdjustUIVariablesOnSettingMode;
            ScanSimpleClicked = new AsyncRelayCommand(execute: OnScanSimpleClicked);
            ScanMultipleClicked = new AsyncRelayCommand(execute: OnScanMultipleClicked);
            CloseScanClicked = new AsyncRelayCommand(execute: OnCloseScanClicked);
        }

        private void AdjustUIVariablesOnSettingMode()
        {
            ShowScanner = ScannerModel.Instance?.Mode != Mode.Off;
            switch (ScannerModel.Instance?.Mode)
            {
                case Mode.Barcode:
                    ShowBarcodeCaptureLayout = true;
                    ShowTrackingBarcodeLayout = false;
                    break;
                case Mode.Tracking:
                    ShowTrackingBarcodeLayout = true;
                    ShowBarcodeCaptureLayout = false;
                    break;
                default:
                    ShowBarcodeCaptureLayout = false;
                    ShowTrackingBarcodeLayout = false;
                    break;
            }
        }

        private void SubscribeToAppMessages()
        {
            MessagingCenter.Subscribe(this, App.MessageKeys.OnResume, callback: async (App app) => await this.OnResumeAsync());
            MessagingCenter.Subscribe(this, App.MessageKeys.OnSleep, callback: async (App app) => await this.OnSleep());
        }

        private void InitializeScanner()
        {
            this.BarcodeTracking.AddListener(this);
            this.BarcodeCapture.AddListener(this);
        }

        public Task OnSleep()
        {
            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            // Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            
            ScannerModel.Instance.SetScannerMode(Mode.Off);
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public async Task OnResumeAsync()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    await this.ResumeFrameSource();
                }
            }
            else
            {
                await this.ResumeFrameSource();
            }
        }

        private Task ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        #region Barcode Tracking Listeners

        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        { }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        { }

        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session,
            IFrameData frameData)
        {
            // This method is called whenever objects are updated and it's the right place to react to the tracking results.
            lock (this.scanResults)
            {
                foreach (var trackedBarcode in session.AddedTrackedBarcodes)
                {
                    SymbologyDescription description = new SymbologyDescription(trackedBarcode.Barcode.Symbology);
                    this.scanResults.Add(new ScanResult(trackedBarcode.Identifier)
                    {
                        Data = trackedBarcode.Barcode.Data,
                        Symbology = description.ReadableName
                    });
                }
            }
        }

        public Brush BrushForTrackedBarcode(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {
            return this.highlightedBrush;
        }

        public void OnTrackedBarcodeTapped(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        { }

        #endregion

        #region Barcode Capture Listeners

        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        { }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        { }

        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        { }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            // This method is called whenever objects are updated and it's the right place to react to the tracking results.
            lock (this.scanResults)
            {
                foreach (var trackedBarcode in session.NewlyRecognizedBarcodes)
                {
                    SymbologyDescription description = new SymbologyDescription(trackedBarcode.Symbology);
                    this.scanResults.Add(new ScanResult(trackedBarcode.SymbolCount)
                    {
                        Data = trackedBarcode.Data,
                        Symbology = description.ReadableName
                    });
                }
            }
        }

        #endregion

        public void RemoveFromAdditiveScanResults(int id)
        {
            this.scanResults.Remove(scanResults.FirstOrDefault(t => t.Id == id));
            OnPropertyChanged("scanResults");
        }

        #region UI Listeners

        private Task OnScanSimpleClicked()
        {
            ScannerModel.Instance.SetScannerMode(Mode.Barcode);
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private Task OnScanMultipleClicked()
        {
            ScannerModel.Instance.SetScannerMode(Mode.Tracking);
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private Task OnCloseScanClicked()
        {
            ScannerModel.Instance.SetScannerMode(Mode.Off);
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }
        #endregion
    }
}
