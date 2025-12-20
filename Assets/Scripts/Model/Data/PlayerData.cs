using System;
using Model.Data;
using UnityEngine;

namespace DefaultNamespace.Model.State
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;
        
        public int Hp;

        public InventoryData Inventory => _inventory;
    }
}