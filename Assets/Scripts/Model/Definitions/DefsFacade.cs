using System;
using Model.Definitions.Potions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private PlayerDef player;
        [SerializeField] private InventoryItemsDef _items;
        [SerializeField] private ThrowableItemsDef _throwables;

        [Space] [Header("Potions Specs")] 
        [SerializeField] private HealPotionDef _healPotion;
        [SerializeField] private JumpPotionDef _jumpPotion;

        public PlayerDef Player => player;
        public InventoryItemsDef Items => _items;
        public ThrowableItemsDef Throwables => _throwables;
        
        public HealPotionDef HealPotion => _healPotion;
        public JumpPotionDef JumpPotion => _jumpPotion;
        

        private static DefsFacade _instance;

        public static DefsFacade I => _instance == null ? LoadDefs() : _instance;

        private static DefsFacade LoadDefs() =>
            _instance = Resources.Load<DefsFacade>("DefsFacade");
    }
}