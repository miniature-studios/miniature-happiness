using System;

namespace Level.Shop.View
{
    internal interface IShopContent
    {
        public void Hide();

        public event Action OnSwitchedTo;
    }
}
