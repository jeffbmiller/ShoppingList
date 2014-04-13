using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using System.Threading;

namespace ShoppingList
{
    public class ShoppingService
    {
        string folder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
        SQLiteConnection conn;

        public ShoppingService()
        {
            conn = new SQLiteConnection (System.IO.Path.Combine (folder, "shoppingItems.db"));
            conn.CreateTable<ShoppingItem>();
//            Console.WriteLine ("Table created!");
//            if (!GetItems().Any())
//            {
//                var shoppingItems = new List<ShoppingItem>();
//                shoppingItems.Add(new ShoppingItem() { Item = "Milk", Quantity = 1, Location = "Sobeys" });
//                shoppingItems.Add(new ShoppingItem() { Item = "Bread", Quantity = 1, Location = "Sobeys" });
//
//                foreach (var item in shoppingItems)
//                    SaveItem(item);
//            }
        }
            
        private IEnumerable<ShoppingItem> GetItems()
        {
            return conn.Table<ShoppingItem>();
        }

        private ShoppingItem SaveItem(ShoppingItem item)
        {
            if (item.Id == 0)
            {
                conn.Insert(item);
                Console.WriteLine ("New item ID: {0} inserted", item.Id);

            }
            else
            {
                conn.Update(item);
                Console.WriteLine ("Item ID: {0} updated", item.Id);

            }
            return item;
        }

        private int DeleteItem(ShoppingItem item)
        {
            var id = conn.Delete(item);
            Console.WriteLine ("Item ID: {0} deleted", item.Id);
			return id;
        }

        public Task<int> DeleteShoppingItem(ShoppingItem item)
        {
            return Task.Factory.StartNew(() => { 
				var id = DeleteItem(item);
				return id;
			});
        }

        public Task<IEnumerable<ShoppingItem>> QueryItemsAsync()
        {
            return Task.Factory.StartNew(() => GetItems());
        }

        public Task<ShoppingItem> SaveShoppingItem(ShoppingItem item)
        {
            return Task.Factory.StartNew(() =>
                                         {
                SaveItem(item);
                return item;
            });
        }
    }
}

