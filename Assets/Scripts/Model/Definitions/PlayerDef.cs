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
        
        public int InventorySize => _inventorySize;
        public int MaxHealth => _maxHealth;
    }
}