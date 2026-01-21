using Model;
using Model.Definitions;
using Model.Definitions.Player;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using Utils.Disposables;

namespace UI.Windows.PlayerStats
{
    public class ManageStatsWindow : AnimatedWindow
    {
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private StatWidget _prefab;

        [SerializeField] private ItemWidget _price;
        [SerializeField] private Button _upgradeButton;

        private DataGroup<StatDef, StatWidget> _dataGroup;
        
        private GameSession _session;
        
        private readonly CompositeDisposable _trash = new CompositeDisposable();

        protected override void Start()
        {
            base.Start();

            _dataGroup = new DataGroup<StatDef, StatWidget>(_prefab, _statsContainer);
            
            _session = FindObjectOfType<GameSession>();
            _session.StatsModel.InterfaceSelectedStat.Value = DefsFacade.I.Player.Stats[0].ID;
            
            _trash.Retain(_session.StatsModel.Subscribe(OnStatsChanged));
            _trash.Retain(_upgradeButton.onClick.Subscribe(OnUpgrade));

            OnStatsChanged();
        }

        private void OnUpgrade()
        {
            var selected = _session.StatsModel.InterfaceSelectedStat.Value;
            _session.StatsModel.LevelUp(selected);
        }

        private void OnStatsChanged()
        {
            var stats = DefsFacade.I.Player.Stats;
            _dataGroup.SetData(stats);

            var selected = _session.StatsModel.InterfaceSelectedStat.Value;
            var nextLevel = _session.StatsModel.GetCurrentLevel(selected) + 1;
            var def = _session.StatsModel.GetStatLevelDef(selected, nextLevel);
            _price.SetData(def.Price);

            _price.gameObject.SetActive(def.Price.Count != 0);
            _upgradeButton.gameObject.SetActive(def.Price.Count != 0);
            
            if (_upgradeButton.gameObject.activeSelf)
                _upgradeButton.interactable = _session.StatsModel.CanUpgrade(selected, nextLevel);
        }

        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}