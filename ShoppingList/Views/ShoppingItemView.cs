using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace ShoppingList
{
    public class ShoppingItemView : DialogViewController
    {
        private ShoppingItemViewPresenter presenter;
        private EntryElement item, quantity, location;

        public ShoppingItemView(ShoppingItem item):base(null,true)
        {
            this.presenter = new ShoppingItemViewPresenter(item);
            var tap = new UITapGestureRecognizer ();
            tap.AddTarget (() =>{
               View.EndEditing (true);
            });
            View.AddGestureRecognizer (tap);

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
                    (location = new EntryElement("Location","Enter Location", presenter.Location))

                }
            };

            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save, async delegate
            {
                presenter.Item = item.Value;
                presenter.Quantity = Convert.ToDecimal(quantity.Value);
                presenter.Location = location.Value;
                await presenter.SaveItem();
                NavigationController.PopToRootViewController(true);
            });

        }
    }
}

