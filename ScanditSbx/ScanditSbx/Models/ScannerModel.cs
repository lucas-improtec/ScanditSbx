using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scandit.DataCapture.Barcode.Data.Unified;
using Scandit.DataCapture.Barcode.Tracking.Capture.Unified;
using Scandit.DataCapture.Core.Capture.Unified;
using Scandit.DataCapture.Core.Source.Unified;
using Scandit.DataCapture.Barcode.Capture.Unified;
using ScanditSbx.Configuration;

namespace ScanditSbx.Models
{
    public class ScannerModel
    {
        #region Instance
        private static readonly Lazy<ScannerModel> instance = new Lazy<ScannerModel>(() => new ScannerModel(), LazyThreadSafetyMode.PublicationOnly);
        private Mode _mode;

        public static ScannerModel Instance => instance.Value;
        #endregion

        #region Mode
        public Mode Mode
        {
            get => _mode;
            private set
            {
                _mode = value;
                OnSettingMode?.Invoke();
            }
        }

        public delegate void SetMode();

        public static event SetMode OnSettingMode;
        #endregion

        #region DataCaptureContext
        public DataCaptureContext DataCaptureContext { get; private set; }
        #endregion

        #region CameraSettings
        public Camera CurrentCamera { get; private set; } = Camera.GetCamera(CameraPosition.WorldFacing);
        public CameraSettings TrackingCameraSettings { get; } = BarcodeTracking.RecommendedCameraSettings;
        public CameraSettings BarcodeCameraSettings { get; } = BarcodeTracking.RecommendedCameraSettings;
        #endregion

        #region BarcodeTracking
        public BarcodeTracking BarcodeTracking { get; private set; }
        public BarcodeTrackingSettings BarcodeTrackingSettings { get; private set; }
        #endregion

        #region BarcodeCapture
        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }
        #endregion

        private ScannerModel()
        {
            OnSettingMode += SetScanner;
            // Adjust camera settings - set Full HD resolution.
            this.BarcodeCameraSettings.PreferredResolution = VideoResolution.FullHd;

            if(Mode == Mode.Barcode) this.CurrentCamera?.ApplySettingsAsync(this.BarcodeCameraSettings);
            else if(Mode == Mode.Tracking) this.CurrentCamera?.ApplySettingsAsync(this.TrackingCameraSettings);

            // Create data capture context using your license key and set the camera as the frame source.
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(ScanditConfig.SCANDIT_LICENSE_KEY);
            this.DataCaptureContext.SetFrameSourceAsync(this.CurrentCamera);

            // The barcode tracking process is configured through barcode tracking settings
            // and are then applied to the barcode tracking instance that manages barcode tracking.
            this.BarcodeTrackingSettings = BarcodeTrackingSettings.Create(BarcodeTrackingScenario.A);
            this.BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.Qr,
                Symbology.Code128
            };

            this.BarcodeTrackingSettings.EnableSymbologies(symbologies);
            this.BarcodeCaptureSettings.EnableSymbologies(symbologies);
            this.BarcodeCapture = BarcodeCapture.Create(this.DataCaptureContext, this.BarcodeCaptureSettings);
            this.BarcodeTracking = BarcodeTracking.Create(this.DataCaptureContext, this.BarcodeTrackingSettings);
            this.DataCaptureContext.RemoveAllModes();
            Mode = Mode.Off;
        }

        private void SetScanner()
        {
            this.BarcodeTracking.Enabled = false;
            this.BarcodeCapture.Enabled = false;
            //this.DataCaptureContext.RemoveAllModes();
            // Adjust Camera Settings
            if (Mode == Mode.Barcode)
            {
                //this.DataCaptureContext.AddMode(BarcodeCapture);
                this.DataCaptureContext.AddMode(BarcodeCapture);
                this.CurrentCamera?.ApplySettingsAsync(this.BarcodeCameraSettings);
                this.BarcodeCapture.Enabled = true;
            }
            else if (Mode == Mode.Tracking)
            {
                //this.DataCaptureContext.AddMode(BarcodeTracking);
                this.DataCaptureContext.AddMode(BarcodeTracking);
                this.CurrentCamera?.ApplySettingsAsync(this.TrackingCameraSettings);
                this.BarcodeTracking.Enabled = true;
            }

        }

        public void SetScannerMode(Mode mode)
        {
            if (this.Mode == mode) return;
            this.Mode = mode;
        }
    }
}
