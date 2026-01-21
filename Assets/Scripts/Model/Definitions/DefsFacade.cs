using Model.Definitions.Repositories;
using Model.Definitions.Repositories.Item;
using Model.Definitions.Repositories.Potions;
using UnityEngine;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private PlayerDef player;
        
        [SerializeField] private ItemsReposiory _items;
        [SerializeField] private ThrowableRepository _throwables;
        [SerializeField] private HealPotionRepository _healPotion;
        [SerializeField] private JumpPotionRepository _jumpPotion;
        [SerializeField] private PerkRepository _perks;

        public PlayerDef Player => player;
        
        public ItemsReposiory Items => _items;
        public ThrowableRepository Throwables => _throwables;
        public HealPotionRepository HealPotion => _healPotion;
        public JumpPotionRepository JumpPotion => _jumpPotion;
        public PerkRepository Perks => _perks;

        private static DefsFacade _instance;

        public static DefsFacade I => _instance == null ? LoadDefs() : _instance;

        private static DefsFacade LoadDefs() =>
            _instance = Resources.Load<DefsFacade>("DefsFacade");
    }
}