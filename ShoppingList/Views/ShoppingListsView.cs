using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ShoppingList
{
    public class ShoppingListsView : UITableViewController
    {
        private ShoppingListsViewPresenter presenter;

        public ShoppingListsView()
        {
            this.presenter = new ShoppingListsViewPresenter();
            Title = "Shopping List";
            TableView.Source = new ShoppingItemsSource(presenter, this);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, delegate
            {
                NavigationController.PushViewController(new ShoppingItemView(new ShoppingItem()),true);
            });

            presenter.IsBusyChanged = (busy) =>
            {
                if(busy)
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
                else
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            };
        }

        public async override void ViewDidLoad()
        {
            await presenter.ExecuteLoadExpensesCommand();
            TableView.ReloadData();
            base.ViewDidLoad();
        }

        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            await presenter.ExecuteLoadExpensesCommand();
            TableView.ReloadData();
            base.ViewDidLoad();
        }
    }

    public class ShoppingItemsSource : UITableViewSource
    {
        private ShoppingListsViewPresenter presenter;
        private string cellIdentifier = "ExpenseCell";
        private ShoppingListsView controller;
        public ShoppingItemsSource(ShoppingListsViewPresenter presenter, ShoppingListsView controller)
        {
            this.presenter = presenter;
            this.controller = controller;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return presenter.ShoppingItems.Count;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }

//        public async override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
//        {
//            var item = presenter.ShoppingItems[indexPath.Row];
//            await presenter.DeleteItem(item);
//            tableView.ReloadData();
//        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier) as SlidingCell;
				var item = presenter.ShoppingItems [indexPath.Row];
            if (cell == null)
            {
				cell = new SlidingCell (cellIdentifier);
				cell.DeleteButton.TouchDown += async delegate {
					await presenter.DeleteItem(item);
					tableView.ReloadData();
				};
            }

            ;
            cell.TextLabel.Text = item.Item;

//			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
//            if (viewModel.IsBusy)
//                return;

            var item = presenter.ShoppingItems[indexPath.Row];
            controller.NavigationController.PushViewController(new ShoppingItemView(item), true);
        }

    }
}

