using Model;
using Model.Definitions.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Disposables;

namespace Components.Effects.CameraRelated
{
    public class DamageSplashOverlay : MonoBehaviour
    {
        [SerializeField] private Image[] _overlays;
        [SerializeField] private float damageThreshold = 0.3f;
        
        private GameSession _session;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _trash.Retain(_session.Data.Hp.SubscribeAndInvoke(OnHpChanged));
        }

        private void OnHpChanged(int newValue, int oldValue)
        {
            float maxHp = _session.StatsModel.GetValue(StatId.Hp);
            float currentHp = _session.Data.Hp.Value;
            float thresholdHp = maxHp * damageThreshold;
            
            bool lowHp = currentHp < thresholdHp;
            float overlayAlpha = lowHp ? 1 - currentHp / thresholdHp : 0;
            
            foreach (var overlay in _overlays)
            {
                Color tmpColor = overlay.color;
                tmpColor.a = overlayAlpha;
                overlay.color = tmpColor;
            }
        }

        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}