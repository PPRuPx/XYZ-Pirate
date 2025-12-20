using UnityEngine;

namespace Components.GameObjectBased
{
    public class GOContainerComponent : MonoBehaviour
    {
        [SerializeField] private GameObject[] _gos;
        [SerializeField] private DropEvent _onDrop;
        
    }
}