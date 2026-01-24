using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Components
{
    public class LightControllerComponent : MonoBehaviour
    {
        [SerializeField] private Light2D light;

        private float _maxIntensity;

        private void Start()
        {
            _maxIntensity = light.intensity;
        }

        public void SetIntensityRatio(float intensityRatio) =>
            light.intensity = _maxIntensity * intensityRatio;
    }
}