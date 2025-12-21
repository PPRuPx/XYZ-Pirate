using UnityEngine;
using UnityEngine.Audio;

namespace Components.Audio
{
    public class EchoChamberComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private AudioMixerSnapshot _defaultSnapshot;
        [SerializeField] private AudioMixerSnapshot _caveSnapshot;
        [SerializeField] private float _transitionTime = 1.0f;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered Cave: Switch to Cave Snapshot");
            if (other.CompareTag(_tag))
                _caveSnapshot.TransitionTo(_transitionTime);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Exited Cave: Switch to Normal Snapshot");
            if (other.CompareTag(_tag))
                _defaultSnapshot.TransitionTo(_transitionTime);
        }
    }
}