using UnityEngine;

public class LayerCheck : MonoBehaviour
{
    [SerializeField] private LayerMask[] _layers;
    
    private Collider2D _collider;

    public bool IsTouchingLayer;
    
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        foreach (var layer in _layers)
        {
            if (_collider.IsTouchingLayers(layer))
            {
                IsTouchingLayer = true;
                return;
            }
        }
    
        IsTouchingLayer = false;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (var layer in _layers)
        {
            if (_collider.IsTouchingLayers(layer))
            {
                IsTouchingLayer = true;
                return;
            }
        }
    
        IsTouchingLayer = false;
    }
}