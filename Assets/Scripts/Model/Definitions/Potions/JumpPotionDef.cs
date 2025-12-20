using UnityEngine;

namespace Model.Definitions.Potions
{
    [CreateAssetMenu(menuName = "Defs/Potions/JumpPotion", fileName = "JumpPotion")]
    public class JumpPotionDef : ScriptableObject
    {
        [SerializeField] private int _multiplier;
        [SerializeField] private int _duration;

        public int Multiplier => _multiplier;
        public int Duration => _duration;
    }
}