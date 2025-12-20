using UnityEngine;

namespace Model.Definitions.Potions
{
    [CreateAssetMenu(menuName = "Defs/Potions/HealPotion", fileName = "HealPotion")]
    public class HealPotionDef : ScriptableObject
    {
        [SerializeField] private int _healAmount;

        public int HealAmount => _healAmount;
    }
}