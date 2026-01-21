using System;
using Creatures.Hero;
using UnityEngine;
using UnityEngine.Events;

namespace Components.Interactables
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] private InteractEvent _action;

        public void Interact()
        {
            var hero = FindObjectOfType<Hero>();
            if (hero)
                _action?.Invoke(hero.gameObject);   
        }
        
        [Serializable]
        public class InteractEvent : UnityEvent<GameObject>
        {
        }
    }
}