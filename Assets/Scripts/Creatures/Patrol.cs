using System.Collections;
using UnityEngine;

public abstract class Patrol : MonoBehaviour
{
    public abstract IEnumerator DoPatrol();
}
