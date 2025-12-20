using UnityEngine;

namespace Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/Inventory", fileName = "Inventory")]
    public class InventoryDef : ScriptableObject
    {
        [SerializeField] private int _capacity;

        public int Capacity => _capacity;
    }
}