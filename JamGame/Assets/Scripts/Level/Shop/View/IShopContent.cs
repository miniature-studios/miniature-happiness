using System;

namespace Level.Shop.View
{
    internal interface IShopContent
    {
        public void Show();
        public void Hide();

        public event Action OnSwitchedTo;
    }
}
