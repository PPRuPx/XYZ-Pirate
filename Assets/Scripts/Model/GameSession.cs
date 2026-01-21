using System.Collections.Generic;
using System.Linq;
using Components.LevelManagement;
using Model.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Disposables;

namespace Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private string _defaultCheckPoint;

        public PlayerData Data => _data;
        private PlayerData _save;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        public QuickInventoryModel QuickInventory { get; private set; }
        public PerksModel PerksModel { get; private set; }

        private readonly List<string> _checkpoints = new List<string>();
        public string LastCheckpointId => _checkpoints.Last();
        
        private void Awake()
        {
            var existsSession = GetExistsSession();
            if (existsSession != null)
            {
                existsSession.StartSession(_defaultCheckPoint);
                Destroy(gameObject);
            }
            else
            {
                Save();
                InitModels();
                DontDestroyOnLoad(this);
                StartSession(_defaultCheckPoint);
            }
        }

        private void StartSession(string defaultCheckPoint)
        {
            SetChecked(defaultCheckPoint);

            LoadHud();
            SpawnHero();
        }
        
        private void SpawnHero()
        {
            var checkpoints = FindObjectsOfType<CheckPointComponent>();
            foreach (var checkPoint in checkpoints)
            {
                if (checkPoint.Id == LastCheckpointId)
                {
                    checkPoint.SpawnHero();
                    break;
                }
            }
        }
        
        private void InitModels()
        {
            QuickInventory = new QuickInventoryModel(_data);
            _trash.Retain(QuickInventory);
            
            PerksModel = new PerksModel(_data);
            _trash.Retain(PerksModel);
        }

        private void LoadHud()
        {
            SceneManager.LoadScene("Hud", LoadSceneMode.Additive);
        }
        
        private GameSession GetExistsSession()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var gameSession in sessions)
            {
                if (gameSession != this)
                    return gameSession;
            }

            return null;
        }

        public void Save()
        {
            _save = _data.Clone();
        }

        public void LoadLastSave()
        {
            _data = _save.Clone();
            
            _trash.Dispose();
            InitModels();
        }
        
        public bool IsChecked(string id)
        {
            return _checkpoints.Contains(id);
        }

        public void SetChecked(string id)
        {
            if (!_checkpoints.Contains(id))
            {
                Save();
                _checkpoints.Add(id);
            }
        }
        
        private void OnDestroy()
        {
            _trash.Dispose();
        }
    }
}