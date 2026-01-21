using Model;
using Model.Definitions;
using UI.Widgets;
using UnityEngine;
using Utils;
using Utils.Disposables;

namespace UI.Hud
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private ProgressBarWidget _healthBar;
        [SerializeField] private CurrentPerkWidget _currentPerk;

        private GameSession _session;
        private readonly CompositeDisposable _trash = new CompositeDisposable();
        
        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            _trash.Retain(_session.Data.Hp.SubscribeAndInvoke(OnHealthChanged));
            _trash.Retain(_session.PerksModel.Subscribe(OnPerkChanged));
            
            OnPerkChanged();
        }

        private void OnPerkChanged()
        {
            var usedPerkId = _session.PerksModel.Used;
            var hasPerk = !string.IsNullOrEmpty(usedPerkId);
            if (hasPerk)
            {
                var perkDef = DefsFacade.I.Perks.Get(usedPerkId);
                _currentPerk.Set(perkDef);
            }
            
            _currentPerk.gameObject.SetActive(hasPerk);
        }

        private void OnHealthChanged(int newValue, int oldValue)
        {
            var maxHealth = DefsFacade.I.Player.MaxHealth;
            var value = (float)newValue / maxHealth;
            _healthBar.SetProgress(value);
        }

        public void OnSettings()
        {
            WindowUtils.CreateWindow("UI/InGameMenuWindow");
        }

        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}