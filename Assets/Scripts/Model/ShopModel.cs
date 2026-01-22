using System;
using Model.Data;
using Model.Data.Properties;
using Model.Definitions;
using Utils.Disposables;

namespace Model
{
    public class ShopModel : IDisposable
    {
        private readonly PlayerData _data;
        
        public event Action OnChanged;
        
        public readonly StringProperty InterfaceSelection = new StringProperty();
        
        private readonly CompositeDisposable _trash = new CompositeDisposable();
        
        public ShopModel(PlayerData data)
        {
            _data = data;
            _trash.Retain(InterfaceSelection.Subscribe((x, y) => OnChanged?.Invoke()));
        }
        
        public IDisposable Subscribe(Action call)
        {
            OnChanged += call;
            return new ActionDisposable(() => OnChanged -= call);
        }
        
        public bool CanBuy(string itemId)
        {
            var def = DefsFacade.I.CrabbyShop.Get(itemId);
            return _data.Inventory.IsEnough(def.BuyPrice);
        }
        
        public bool CanSell(string itemId)
        {

            return _data.Inventory.hasItem(itemId);
        }
        
        public void Dispose()
        {
            _trash.Dispose();
        }
    }
}