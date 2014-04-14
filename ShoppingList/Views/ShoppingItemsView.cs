using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Threading;

namespace ShoppingList
{
	public class ShoppingItemsView : DialogViewController, IShoppingListsView
	{
		private ShoppingItemsPresenter presenter;

		public ShoppingItemsView ():base(new RootElement("Shopping List"))
		{
			this.presenter = new ShoppingItemsPresenter(this);
			Style = UITableViewStyle.Plain;
			TableSource = new ShoppingItemsSource (this,presenter);

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, delegate
				{
					presenter.ShowItemView(new ShoppingItem());
				});

			presenter.IsBusyChanged = (busy) =>
			{
				if(busy)
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				else
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			};

			RefreshRequested += delegate {
				Thread.Sleep(1000);
				presenter.Refresh();
				ReloadComplete();
			};
				
		}

		public Source TableSource { get; private set; }

		public override void ViewDidLoad()
		{
//			presenter.Refresh ();
			base.ViewDidLoad();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			presenter.Refresh ();
		}
			
		public override Source CreateSizingSource (bool unevenRows)
		{
			return TableSource;
		}

		#region IShoppingItemsView Implementation

		public void Refresh(IEnumerable<ShoppingItem> items)
		{
			InvokeOnMainThread (delegate {
				if (!Root.Any ())
					Root.Add (new Section (""));
				var section = Root[0];
				section.Clear();
				section.AddAll (
					items.Select (x => new StyledStringElement (x.Item, delegate {
						presenter.ShowItemView (x);
					})));
				Root.Reload(section,UITableViewRowAnimation.None);
			});

		}

		public void ShowItemView(ShoppingItem item)
		{
			NavigationController.PushViewController(new ShoppingItemView(item),true);
		}

		#endregion
	}

	public class LongPressElement :StyledStringElement
	{
		private UILongPressGestureRecognizer gester;
		private MonoTouch.Foundation.NSAction longPressAction;
		private ShoppingItem item;

		public LongPressElement (ShoppingItem item, MonoTouch.Foundation.NSAction tappedAction,MonoTouch.Foundation.NSAction longPressAction):base(item.Item,tappedAction)
		{
			this.item = item;
			this.longPressAction = longPressAction;
			gester = new UILongPressGestureRecognizer ();
			gester.Delegate = new LongPressGestureDelegate ();
			gester.MinimumPressDuration = .5;
			gester.AddTarget (x => {
				if ((x as UILongPressGestureRecognizer).State == UIGestureRecognizerState.Began)
				{
					longPressAction();
				}
			});

		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			if (cell.GestureRecognizers == null || cell.GestureRecognizers.Count() ==0)
				cell.AddGestureRecognizer (gester);
			cell.Accessory = item.Completed ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}
	}

	public class LongPressGestureDelegate : UIGestureRecognizerDelegate
	{
		public LongPressGestureDelegate ()
		{

		}



	}

	public class ShoppingItemsSource : MonoTouch.Dialog.DialogViewController.Source
	{
		private ShoppingItemsPresenter presenter;

		public ShoppingItemsSource (DialogViewController dvc,ShoppingItemsPresenter presenter):base(dvc)
		{
			this.presenter = presenter;

		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}
			
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			presenter.DeleteItem (presenter.ShoppingItems.ToList()[indexPath.Item]);
		}
			
	}
}

