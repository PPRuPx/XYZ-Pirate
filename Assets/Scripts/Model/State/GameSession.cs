using System;
using System.Collections;
using DefaultNamespace.Model.State;
using UnityEngine;

namespace Model.State
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        
        public PlayerData Data => _data;

        private void Awake()
        {
            if (IsSessionExist())
                DestroyImmediate(gameObject);
            else
                DontDestroyOnLoad(this);
        }

        private bool IsSessionExist()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var session in sessions)
            {
                if (session != this) 
                    return true;
            }

            return false;
        }
    }
}