using System.Linq;
using Creatures.Hero;
using Model;
using Model.Definitions;
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
            
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        
        public void SoftReload()
        {
            Destroy(FindObjectOfType<Hero>().gameObject);
            
            var session = FindObjectOfType<GameSession>();
            session.Data.Hp.Value = DefsFacade.I.Player.MaxHealth;
            
            FindObjectsOfType<CheckPointComponent>()
                .First(cp => cp.Id == session.LastCheckpointId)
                .SpawnHero();

            FindObjectOfType<SetFollowComponent>().Start();
        }
    }
}