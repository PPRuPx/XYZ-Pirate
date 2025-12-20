using System;
using Model.Definitions.Potions;
using UnityEngine;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private InventoryDef _inventory;
        [SerializeField] private InventoryItemsDef _items;

        [Space] [Header("Potions Specs")] 
        [SerializeField] private HealPotionDef _healPotion;
        [SerializeField] private JumpPotionDef _jumpPotion;

        public InventoryDef Inventory => _inventory;
        public InventoryItemsDef Items => _items;
        
        public HealPotionDef HealPotion => _healPotion;
        public JumpPotionDef JumpPotion => _jumpPotion;
        

        private static DefsFacade _instance;

        public static DefsFacade I => _instance == null ? LoadDefs() : _instance;

        private static DefsFacade LoadDefs() =>
            _instance = Resources.Load<DefsFacade>("DefsFacade");
    }
}