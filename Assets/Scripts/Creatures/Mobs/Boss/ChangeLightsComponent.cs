using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Creatures.Mobs.Boss
{
    public class ChangeLightsComponent : MonoBehaviour
    {
        [SerializeField] private LightsSetup[] _lightsSetups;

        private int currentSetupIndex = 0;
        
        [ContextMenu("ApplyNextSetup")]
        public void ApplyNextSetup()
        {
            if (currentSetupIndex >= _lightsSetups.Length) return;
            
            foreach (var light2D in _lightsSetups[currentSetupIndex].Lights)
            {
                light2D.color = _lightsSetups[currentSetupIndex].Color;
                light2D.intensity = _lightsSetups[currentSetupIndex].Intensity;
            }

            currentSetupIndex++;
        }
    }

    [Serializable]
    public class LightsSetup
    {
        [SerializeField] private Light2D[] _lights;
        
        [ColorUsage(true, true)] 
        [SerializeField] private Color _color;
        [SerializeField] private float _intensity;
        
        public Light2D[] Lights => _lights;
        public Color Color => _color;
        public float Intensity => _intensity;
    }
}