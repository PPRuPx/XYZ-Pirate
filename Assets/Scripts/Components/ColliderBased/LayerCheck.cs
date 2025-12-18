using UnityEngine;

namespace Components.ColliderBased
{
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private bool _isTouchingLayers;

        private Collider2D _collider;

        public bool IsTouchingLayers => _isTouchingLayers;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerStay2D(Collider2D other) =>
            _isTouchingLayers = _collider.IsTouchingLayers(_mask);

        private void OnTriggerExit2D(Collider2D other) =>
            _isTouchingLayers = _collider.IsTouchingLayers(_mask);
    }
}