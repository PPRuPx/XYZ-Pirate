using UnityEngine;

namespace DefaultNamespace.Utils
{
    public static class GameObjectExtensions
    {
        public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask.value & (1 << gameObject.layer)) != 0;
        }
    }
}