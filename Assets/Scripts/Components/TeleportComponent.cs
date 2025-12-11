using UnityEngine;

namespace Components
{
    public class TeleportComponent : MonoBehaviour
    {
        [SerializeField] private Transform _distPosition;

        public void Teleport(GameObject target)
        {
            target.transform.position = _distPosition.position;
        }
    }
}