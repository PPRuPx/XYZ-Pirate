using System;
using Model.Definitions.Repositories.Item;
using UnityEngine;

namespace Model.Definitions.Repositories
{
    [CreateAssetMenu(menuName = "Defs/ThrowableItemsDef", fileName = "ThrowableItemsDef")]
    public class ThrowableRepository : ScriptableObject
    {
        [SerializeField] private ThrowableDef[] _items;

        public ThrowableDef Get(string id)
        {
            foreach (var itemDef in _items)
            {
                if (itemDef.Id == id)
                    return itemDef;
            }

            return default;
        }
    
#if UNITY_EDITOR
    public ThrowableDef[] ItemsForEditor => _items;
#endif
    }

    [Serializable]
    public struct ThrowableDef
    {
        [InventoryId]
        [SerializeField] private string _id;
        [SerializeField] private GameObject _projectile;
            
        public string Id => _id;
        public GameObject Projectile => _projectile;
    }
}