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
        private EntryElement item, location;
		private StepperElement quantity;
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
			Root = new RootElement(""){
                new Section(){
					(item = new EntryElement("Item","Enter Item name",presenter.Item)),
					(quantity = new StepperElement("Quantity", presenter.Quantity, new StepperView())),
					(location = new EntryElement("Location","Enter Location", presenter.Location)),
					new StyledStringElement("Take Picture", delegate {
						TakePicture();
					}){Accessory = UITableViewCellAccessory.DisclosureIndicator},
					new StringElement("View Picture",delegate {
						ShowPicture();
					}),
				}};


            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, async delegate
            {
                try
                {
                    presenter.Item = item.Value;
					presenter.Quantity = (int)quantity.Value;
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
			await DismissViewControllerAsync(true);
		}

		private async void TakePicture()
		{
			var picker = new UIImagePickerController();
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
			picker.FinishedPickingMedia += HandleFinishedPickingImage;
			picker.Canceled += delegate {
				DismissViewControllerAsync (true);
			};
			await NavigationController.PresentViewControllerAsync(picker,true);
		}

		private void ShowPicture()
		{
			var data = presenter.Model.Image;
			if (data == null)
			{
				new UIAlertView("Error","No saved imaged for this item",null,"OK",null).Show();
				return;
			}
			NSData imageData = NSData.FromArray(data);
			var image = UIImage.LoadFromData(imageData);
			var imageView = new UIImageView(View.Bounds);
			imageView.Image = image;
			var vc = new UIViewController();
			vc.Add(imageView);
			NavigationController.PushViewController(vc,true);
		}
    }

	public class StepperElement : UIViewElement
	{
		private StepperView stepperView;
		public StepperElement (string caption,int value, StepperView stepperView):base(null,stepperView,false)
		{
			this.stepperView = stepperView;
			stepperView.Caption = caption;
			stepperView.Value = value;

		}
		public int Value { get { return stepperView.Value; } }
	}
}

