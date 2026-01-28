using System;
using Model.Definitions.Repositories;
using UnityEngine;

namespace Model.Definitions.Player
{
    [Serializable]
    public struct StatDef
    {
        [SerializeField] private string _name;
        [SerializeField] private StatId _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private StatLevelDef[] _levels;
        [SerializeField] private bool _hidden;

        public StatId ID => _id;
        public string Name => _name;
        public Sprite Icon => _icon;
        public StatLevelDef[] Levels => _levels;
        public bool Hidden => _hidden;
    }

    [Serializable]
    public struct StatLevelDef
    {
        [SerializeField] private float _value;
        [SerializeField] private ItemWithCount _price;

        public float Value => _value;
        public ItemWithCount Price => _price;
    }

    public enum StatId
    {
        Hp,
        Speed,
        RangeDamage,
        CritChance,
        LightTime
    }
}