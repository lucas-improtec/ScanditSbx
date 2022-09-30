# ScanditSbx
Attempt to make BarcodeCapture and BarcodeTracking modes to work in the same view but one at time.

## Projects needs
Our project needs to implement both BarcodeCapture and Barcode tracking in several views during it's life cycle. But some views would need both modes and a way to switch between them.

## Quickly introduction to the project solution
The SetScannerMode method @ScannerModel is supposed to be the only way throughout any viewmodels that may utilize the ScannerModel to initialize any of the supported modes in the app. It attempt's to archieve that by using events that are called in the setter of the Mode @ScannerModel.

## Problem
@ScannerModel constructor we add both modes to the DataCaptureContext and since both have the same requirements they cannot be added that way. Refer to Error

## Solution
@ScannerModel we have SetScanner method, the commentaries in it were one of my first guesses on how to solve the problem. The other ideas fly around having more than one DataCaptureContext and toggle the Layout for each of them or just change the binding if they were supposed to live at the same layout.
