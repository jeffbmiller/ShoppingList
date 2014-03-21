using System;

namespace ShoppingList
{
    public class BasePresenter
    {
        public Action<bool> IsBusyChanged { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                if (IsBusyChanged != null)
                    IsBusyChanged(isBusy);
            }
        }
    }
}

