using System;
using System.Threading.Tasks;

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

        public string Item 
        {
            get {return item.Item;}
            set
            {
                if (item.Item == value)
                    return;
                item.Item = value;
            }
        }
        public decimal Quantity  
        {
            get {return item.Quantity;}
            set
            {
                if (item.Quantity == value)
                    return;
                item.Quantity = value;
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
        #endregion

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

