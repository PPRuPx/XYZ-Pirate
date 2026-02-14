using System.Linq;
using Model.Definitions.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/PlayerDef", fileName = "PlayerDef")]
    public class PlayerDef : ScriptableObject
    {
        [FormerlySerializedAs("_capacity")] 
        [SerializeField] private int _inventorySize;
        [SerializeField] private int _maxHealth;
        [SerializeField] private StatDef[] _stats;
        
        public int InventorySize => _inventorySize;
        public int MaxHealth => _maxHealth;
        public StatDef[] Stats => _stats;

        public StatDef GetStat(StatId id)
        {
            foreach (var x in _stats)
                if (x.ID == id) return x;

            return new StatDef();
        } 
    }
}