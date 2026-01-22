using Model;
using Model.Definitions;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using Utils.Disposables;

namespace UI.Windows.ShopWindow
{
    public class ManageShopWindow : AnimatedWindow
    {
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private ShopItemWidget _prefab;
        
        [SerializeField] private ItemWidget _sellPrice;
        [SerializeField] private Button _sellButton;
        
        [SerializeField] private ItemWidget _buyPrice;
        [SerializeField] private Button _buyButton;
        
        private DataGroup<ShopItemDef, ShopItemWidget> _dataGroup;
        
        private GameSession _session;
        
        private readonly CompositeDisposable _trash = new CompositeDisposable();
        
        protected override void Start()
        {
            base.Start();

            _dataGroup = new DataGroup<ShopItemDef, ShopItemWidget>(_prefab, _itemsContainer);
            
            _session = FindObjectOfType<GameSession>();
            
            _trash.Retain(_session.ShopModel.Subscribe(OnShopItemChanged));
            _trash.Retain(_session.QuickInventory.Subscribe(OnShopItemChanged));
            
            OnShopItemChanged();
        }

        private void OnShopItemChanged()
        {
            var shopData = DefsFacade.I.CrabbyShop.All;
            _dataGroup.SetData(shopData);
            
            var selected = _session.ShopModel.InterfaceSelection.Value;

            if (selected != null)
            {
                var shopItem = SelectedShopItemDef();
                
                _sellPrice.gameObject.SetActive(true);
                _sellPrice.SetData(shopItem.SellPrice);
                _sellButton.gameObject.SetActive(true);
                _sellButton.interactable = _session.ShopModel.CanSell(selected);
                
                _buyPrice.gameObject.SetActive(true);
                _buyPrice.SetData(shopItem.BuyPrice);
                _buyButton.gameObject.SetActive(true);
                _buyButton.interactable = _session.ShopModel.CanBuy(selected);
            }
            else
            {
                _sellPrice.gameObject.SetActive(false);
                _sellButton.gameObject.SetActive(false);
                
                _buyPrice.gameObject.SetActive(false);
                _buyButton.gameObject.SetActive(false);
            }
        }

        public void OnBuy()
        {
            var shopItem = SelectedShopItemDef();
            _session.Data.Inventory.Remove(shopItem.BuyPrice.ItemId, shopItem.BuyPrice.Count);
            _session.Data.Inventory.Add(shopItem.Id, 1);
        }

        public void OnSell()
        {
            var shopItem = SelectedShopItemDef();
            _session.Data.Inventory.Remove(shopItem.Id, 1);
            _session.Data.Inventory.Add(shopItem.SellPrice.ItemId, shopItem.SellPrice.Count);
        }

        private ShopItemDef SelectedShopItemDef()
        {
            var selected = _session.ShopModel.InterfaceSelection.Value;
            return DefsFacade.I.CrabbyShop.Get(selected);
        }
        
        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}