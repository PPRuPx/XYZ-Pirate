using System;
using Model.Definitions.Repositories;
using Model.Definitions.Repositories.Item;
using UnityEngine;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/CrabbyShopRepository", fileName = "CrabbyShopRepository")]
    public class CrabbyShopRepository : DefRepository<ShopItemDef>
    {
    }
    
    [Serializable]
    public struct ShopItemDef : IHaveId
    {
        [InventoryId]
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private ItemWithCount _sellPrice;
        [SerializeField] private ItemWithCount _buyPrice;

        public string Id => _id;
        public Sprite Icon => _icon;
        public ItemWithCount SellPrice => _sellPrice;
        public ItemWithCount BuyPrice => _buyPrice;
    }
}