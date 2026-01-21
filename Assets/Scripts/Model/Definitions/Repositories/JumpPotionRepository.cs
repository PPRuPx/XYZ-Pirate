using UnityEngine;

namespace Model.Definitions.Repositories.Potions
{
    [CreateAssetMenu(menuName = "Defs/JumpPotion", fileName = "JumpPotion")]
    public class JumpPotionRepository : ScriptableObject
    {
        [SerializeField] private int _multiplier;
        [SerializeField] private int _duration;

        public int Multiplier => _multiplier;
        public int Duration => _duration;
    }
}