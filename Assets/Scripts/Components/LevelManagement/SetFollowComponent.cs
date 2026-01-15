using Cinemachine;
using Creatures.Hero;
using UnityEngine;

namespace Components.LevelManagement
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class SetFollowComponent : MonoBehaviour
    {
        public void Start()
        {
            var vCamera = GetComponent<CinemachineVirtualCamera>();
            vCamera.Follow = FindObjectOfType<Hero>().transform;
        }
    }
}