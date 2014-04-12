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
                        var data = presenter.Model.Image;
                        if (data == null)
                        {
                            Console.WriteLine("No saved image found");
                            return;
                        }
                        NSData imageData = NSData.FromArray(data);
                        var image = UIImage.LoadFromData(imageData);
                        var imageView = new UIImageView(View.Bounds);
                        imageView.Image = image;
                        var vc = new UIViewController();
                        vc.Add(imageView);
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
            var imgData = image.AsJPEG();

            Byte[] myByteArray = new Byte[imgData.Length];
            System.Runtime.InteropServices.Marshal.Copy(imgData.Bytes, myByteArray, 0, Convert.ToInt32(imgData.Length));
           
            presenter.Model.Image = myByteArray;
            await presenter.SaveItem();
            DismissViewControllerAsync(true);
		}
    }

}

