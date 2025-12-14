using System.Collections.Generic;
using DefaultNamespace.Utils;
using UnityEditor;
using UnityEngine;

public class CheckCircleOverlap : MonoBehaviour
{
    [SerializeField] private float _radius;

    private Collider2D[] _overlapResult = new Collider2D[20];

    public GameObject[] getObjectsInRange()
    {
        var size = Physics2D.OverlapCircleNonAlloc(
            transform.position, _radius, _overlapResult);

        var overlaps = new List<GameObject>();
        for (int i = 0; i < size; i++)
            overlaps.Add(_overlapResult[i].gameObject);
            
        return overlaps.ToArray();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = HandlesUtils.TransparentRed;
        Handles.DrawSolidDisc(transform.position, Vector3.forward, _radius);
    }
}