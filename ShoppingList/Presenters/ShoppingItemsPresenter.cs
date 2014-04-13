using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;

namespace ShoppingList
{
    public class ShoppingItemsPresenter : BasePresenter
    {
        private ShoppingService shoppingService;
		private IShoppingListsView view;

		public ShoppingItemsPresenter(IShoppingListsView view)
        {
            shoppingService = new ShoppingService();
			this.view = view;
        }

		public IEnumerable<ShoppingItem> ShoppingItems { get; set; }
			
		public async void Refresh()
        {
            try
            {
                IsBusy = true;
				var items = await shoppingService.QueryItemsAsync();
				ShoppingItems = items;
				view.Refresh (items);
                IsBusy = false;
            }
            catch (Exception exception)
            {
				Console.WriteLine("Unable to query items");
            }
            finally
            {
                IsBusy = false;
            }
        }

		public void ShowItemView(ShoppingItem item)
		{
			view.ShowItemView (item);
		}

		public async void MarkAsComplete(ShoppingItem item)
        {
            try
            {
                IsBusy = true;
                item.Completed = !item.Completed; // Toggle Complete for now.
                var result = await shoppingService.SaveShoppingItem(item);
				Console.WriteLine("{0} marked as {1}",item.Item,item.Completed ? "Completed" : "Not Complete");
				Refresh();
                IsBusy = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unable to mark as complete");
            }
            finally
            {
                IsBusy = false;
            }
        }

		public async void DeleteItem(ShoppingItem item)
        {
           
            try
            {
                IsBusy = true;
				var id = await shoppingService.DeleteShoppingItem(item);
				Refresh();
                IsBusy = false;
            }
            catch (Exception exception)
            {
				Console.WriteLine("Unable to delete item");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
	   
}

