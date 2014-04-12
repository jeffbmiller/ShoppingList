using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.IO;

namespace ShoppingList
{
    public class ShoppingItemView : DialogViewController
    {
        private ShoppingItemViewPresenter presenter;
        private EntryElement item, quantity, location;

        public ShoppingItemView(ShoppingItem item):base(null,true)
        {
            this.presenter = new ShoppingItemViewPresenter(item);
//            var tap = new UITapGestureRecognizer ();
//            tap.AddTarget (() =>{
//               View.EndEditing (true);
//            });
//            View.AddGestureRecognizer (tap);

            presenter.IsBusyChanged = (busy) =>
            {
                if(busy)
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
                else
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Root = new RootElement(presenter.Item){
                new Section(){
                    (item = new EntryElement("Item","Enter Item name",presenter.Item)),
                    (quantity = new EntryElement("Quantity","Enter quantity", presenter.Quantity){KeyboardType = UIKeyboardType.NumberPad}),
					(location = new EntryElement("Location","Enter Location", presenter.Location)),
					new StringElement("Show Picture",delegate {
                        var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
                        var filepath = Path.Combine (documents, presenter.Item + ".jpg");
//                        var data = NSData.FromFile(filepath); 

                        var webview = new UIWebView(View.Bounds);
                        webview.LoadRequest(new NSUrlRequest(new NSUrl(filepath,false)));
                        webview.ScalesPageToFit = true;
                        webview.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                        var vc = new UIViewController();
                        vc.Add(webview);
						NavigationController.PushViewController(vc,true);
					}),
					new StyledStringElement("Take Picture", async delegate {
						var picker = new UIImagePickerController();
                        picker.SourceType = UIImagePickerControllerSourceType.Camera;
                        picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
                        picker.FinishedPickingMedia += HandleFinishedPickingImage;
						picker.Canceled += delegate {
							DismissViewControllerAsync(true);
					};
						await NavigationController.PresentViewControllerAsync(picker,true);
					}){Accessory = UITableViewCellAccessory.DisclosureIndicator}
				}};


            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, async delegate
            {
                try
                {
                    presenter.Item = item.Value;
                    presenter.Quantity = quantity.Value;
                    presenter.Location = location.Value;
                    await presenter.SaveItem();
                    NavigationController.PopToRootViewController(true);
                }
                catch (Exception e)
                {
                    new UIAlertView("Error Saving",e.Message,null,"OK",null).Show();
                }
            });

        }
        async void HandleFinishedPickingImage (object sender, UIImagePickerMediaPickedEventArgs e)
		{
            var image = e.OriginalImage;

            var documentsDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
            string jpgFilename = System.IO.Path.Combine (documentsDirectory, presenter.Item +".jpg"); // hardcoded filename, overwritten each time
            NSData imgData = image.AsJPEG();
            NSError err = null;
            if (imgData.Save(jpgFilename, false, out err)) {
                Console.WriteLine("saved as " + jpgFilename);
            } else {
                Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
            }
            presenter.ImagePath = jpgFilename;
            await presenter.SaveItem();
            DismissViewControllerAsync(true);
		}
    }

}

