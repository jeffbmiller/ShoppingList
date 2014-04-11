using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using Xamarin.Media;

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
                    (quantity = new EntryElement("Quantity","Enter quantity", presenter.Quantity.ToString()){KeyboardType = UIKeyboardType.NumberPad}),
                    (location = new EntryElement("Location","Enter Location", presenter.Location)),
					new StringElement("Show Picture",delegate {
						var view = new UIImageView(presenter.GetImage());
						var vc = new UIViewController();
						vc.Add(view);
						NavigationController.PushViewController(vc,true);

					}),
				new Section(){new StyledStringElement("Take Picture", async delegate {
					var picker = new UIImagePickerController();
					picker.FinishedPickingImage += HandleFinishedPickingImage;
					await NavigationController.PresentViewControllerAsync(picker,true);
				}){Accessory = UITableViewCellAccessory.DisclosureIndicator}}
				}};

//			var textView = new UITextView (new RectangleF (5, 200, View.Bounds.Width, 100));
//			textView.Editable = true;
//			textView.Text = "Type Comments here";
//			View.AddSubview (textView);

            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, async delegate
            {
                presenter.Item = item.Value;
                presenter.Quantity = Convert.ToDecimal(quantity.Value);
                presenter.Location = location.Value;
                await presenter.SaveItem();
                NavigationController.PopToRootViewController(true);
            });

        }

		void HandleFinishedPickingImage (object sender, UIImagePickerImagePickedEventArgs e)
        {
			var image = e.Image;
			presenter.SerializeImage (image);
        }
    }

}

