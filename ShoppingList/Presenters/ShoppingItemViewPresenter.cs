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

		public UIImage GetImage()
		{
			UIImage image;
			using (var ms = new MemoryStream(item.Image))
			{
				var formater = new BinaryFormatter ();

				image = formater.Deserialize (ms) as UIImage;
			}

			return image;
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

		public void SerializeImage(UIImage image)
		{
			byte[] bytes;
			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, image);
				bytes = stream.ToArray();
			}
			item.Image = bytes;

		}
    }
}

