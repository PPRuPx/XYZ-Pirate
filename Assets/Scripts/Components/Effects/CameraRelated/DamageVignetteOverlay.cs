using Model;
using Model.Definitions.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utils.Disposables;

namespace Components.Effects.CameraRelated
{
    public class DamageVignetteOverlay: MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Volume globalVolume; // Перетащите ваш GlobalPostFX сюда в инспекторе
        [SerializeField] private float maxIntensity = 0.7f;
    
        private Vignette _vignette;
        private GameSession _session;
        
        private readonly CompositeDisposable _trash = new CompositeDisposable();

        void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _trash.Retain(_session.Data.Hp.SubscribeAndInvoke(OnHpChanged));
            
            if (globalVolume.profile.TryGet(out _vignette))
            {
                _vignette.intensity.value = 0f;
                _vignette.color.value = Color.red;
            }
        }

        public void OnHpChanged(int newValue, int oldValue)
        {
            if (_vignette == null) 
                return;

            float maxHp = _session.StatsModel.GetValue(StatId.Hp);
            float currentHp = _session.Data.Hp.Value;
            float healthPercent = currentHp / maxHp;
            
            float targetIntensity = 1f - healthPercent;
            targetIntensity = Mathf.Clamp(targetIntensity, 0f, maxIntensity);

            _vignette.intensity.value = targetIntensity;
        }
        
        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}