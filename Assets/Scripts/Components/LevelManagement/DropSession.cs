using System;
using Model;
using UnityEngine;

namespace Components.LevelManagement
{
    public class DropSession : MonoBehaviour
    {
        public void Start()
        {
            Destroy(FindObjectOfType<GameSession>());
        }
    }
}