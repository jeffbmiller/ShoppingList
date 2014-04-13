using System;
using System.Collections.Generic;

namespace ShoppingList
{
	public interface IShoppingListsView
	{
		void Refresh(IEnumerable<ShoppingItem> items);
		void ShowItemView(ShoppingItem item);
	}
}

