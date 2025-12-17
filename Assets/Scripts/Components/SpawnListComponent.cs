using System;
using System.Linq;
using UnityEngine;

namespace Components
{
    public class SpawnListComponent : MonoBehaviour
    {
        [SerializeField] private SpawnData[] _spawners;

        public void SpawnAll() {
            foreach (var spawnData in _spawners)
                spawnData.Component.Spawn();
        }


        public void Spawn(string id)
        {
            _spawners.FirstOrDefault(s => s.id == id)?.Component.Spawn();
        }
        
        [Serializable]
        public class SpawnData
        {
            public string id;
            public SpawnComponent Component;
        }
    }
}