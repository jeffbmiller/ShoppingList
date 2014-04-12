using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShoppingList
{
    public class ShoppingItemViewPresenter : BasePresenter
    {
        private ShoppingItem item;
        private ShoppingService service;

        public ShoppingItemViewPresenter(ShoppingItem item)
        {
            this.item = item;
            this.service = new ShoppingService();

        }

        #region Properties

        public ShoppingItem Model { get { return item; } }

        public string Item 
        {
            get {return item.Item;}
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("You must enter a item name");
                if (item.Item == value)
                    return;
                item.Item = value;
            }
        }
        public string Quantity  
        {
            get {return item.Quantity.ToString();}
            set
            {
                if (item.Quantity == Convert.ToDecimal(value))
                    return;
                item.Quantity = Convert.ToDecimal(value);
            }
        }
        public string Location  
        {
            get {return item.Location;}
            set
            {
                if (item.Location == value)
                    return;
                item.Location = value;
            }
        }
        public bool Completed
        {
            get {return item.Completed;}
            set
            {
                if (item.Completed == value)
                    return;
                item.Completed = value;
            }
        }

        public string ImagePath
        {
            get {return item.ImagePath;}
            set
            {
                if (item.ImagePath == value)
                    return;
                item.ImagePath = value;
            }
        }

        #endregion

        public bool CanSave()
        {
            if (string.IsNullOrEmpty(item.Item))
                return false;
            if (item.Quantity == 0)
                return false;
            return true;
        }

        public async Task SaveItem()
        {
            try
            {
                IsBusy = true;
                await service.SaveShoppingItem(item);
                IsBusy = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unable to Save Item.");
            }
            finally
            {
                IsBusy = false;
            }
        }

		

    }
}

