using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;

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
		}

		public Source TableSource { get; private set; }

		public override void ViewDidLoad()
		{
			presenter.Refresh ();
			base.ViewDidLoad();
		}
			
		public override Source CreateSizingSource (bool unevenRows)
		{
			return TableSource;
		}

		#region IShoppingItemsView Implementation

		public void Refresh(IEnumerable<ShoppingItem> items)
		{
			InvokeOnMainThread (delegate {
				Root.Clear ();
				Root.Add (new Section () {
					items.Select (x => new LongPressElement (x, delegate {
						presenter.ShowItemView (x);
					}, delegate {
						presenter.MarkAsComplete (x);
					}))
				});
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
			gester.AddTarget (longPressAction);

		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			if (cell.GestureRecognizers == null || cell.GestureRecognizers.Count() ==0)
				cell.AddGestureRecognizer (gester);
			cell.Accessory = item.Completed ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		
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

