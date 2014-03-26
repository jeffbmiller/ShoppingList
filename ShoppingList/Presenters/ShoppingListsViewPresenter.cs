using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;

namespace ShoppingList
{
    public class ShoppingListsViewPresenter : BasePresenter
    {
        private ShoppingService shoppingService;

        public ShoppingListsViewPresenter()
        {
            shoppingService = new ShoppingService();

        }

        public List<ShoppingItem> ShoppingItems {get;set;}

        public async Task ExecuteLoadExpensesCommand()
        {
            ShoppingItems = new List<ShoppingItem>();

            try
            {
                IsBusy = true;
                var shoppingItems = await shoppingService.GetShoppingItems();
                IsBusy = false;
                foreach (var expense in shoppingItems)
                {
                    ShoppingItems.Add(expense);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine("Unable to query and gather expenses");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task MarkAsComplete(ShoppingItem item)
        {
            try
            {
                IsBusy = true;
                item.Completed = !item.Completed; // Toggle Complete for now.
                var result = await shoppingService.SaveShoppingItem(item);
                ShoppingItems.First(x=>x.Id == item.Id).Completed = item.Completed;
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

        public async Task DeleteItem(ShoppingItem item)
        {
           
            try
            {
                IsBusy = true;
                var id = await shoppingService.DeleteShoppingItem(item);
                IsBusy = false;
                ShoppingItems.Remove(ShoppingItems.First(x=>x.Id == item.Id));

            }
            catch (Exception exception)
            {
                Console.WriteLine("Unable to query and gather expenses");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

   
}

