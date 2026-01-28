using System.Linq;
using Creatures.Hero;
using Model;
using Model.Definitions.Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Components.LevelManagement
{
    public class ReloadLevelComponent : MonoBehaviour
    {
        public void Reload()
        {
            var session = FindObjectOfType<GameSession>();
            session.LoadLastSave();
            
            var loader = FindObjectOfType<LevelLoader>();
            var scene = SceneManager.GetActiveScene();
            loader.LoadLevel(scene.name);
        }
        
        public void SoftReload()
        {
            Destroy(FindObjectOfType<Hero>().gameObject);
            
            var session = FindObjectOfType<GameSession>();
            session.Data.Hp.Value = (int) session.StatsModel.GetValue(StatId.Hp);
            session.Data.Light.Value = (float) session.StatsModel.GetValue(StatId.LightTime);
            
            FindObjectsOfType<CheckPointComponent>()
                .First(cp => cp.Id == session.LastCheckpointId)
                .SpawnHero();

            FindObjectOfType<SetFollowComponent>().Start();
        }
    }
}