using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Components
{
    public class LightControllerComponent : MonoBehaviour
    {
        [SerializeField] private Light2D light;

        public void SetIntensityRatio(float intensityRatio) =>
            light.intensity = 1 * intensityRatio;
    }
}