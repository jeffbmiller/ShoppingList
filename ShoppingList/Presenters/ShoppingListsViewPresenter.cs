using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MonoTouch.UIKit;

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

        public async Task DeleteItem(ShoppingItem item)
        {
           
            try
            {
                IsBusy = true;
                await shoppingService.DeleteShoppingItem(item);
                IsBusy = false;
                ShoppingItems.Remove(item);

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

