using UnityEngine;

namespace Model.Definitions.Repositories.Potions
{
    [CreateAssetMenu(menuName = "Defs/HealPotion", fileName = "HealPotion")]
    public class HealPotionRepository : ScriptableObject
    {
        [SerializeField] private int _healAmount;

        public int HealAmount => _healAmount;
    }
}