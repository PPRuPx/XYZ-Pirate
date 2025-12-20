using System;
using Animations;
using Components.ColliderBased;
using UnityEngine;
using Utils;

namespace Creatures.Mobs
{
    public class ShootingTrapAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private Cooldown _cooldown;
        [SerializeField] private SpriteMultiAnimation _animation;

        private void Update()
        {
            if (_vision.IsTouchingLayers && _cooldown.IsReady)
                Shoot();
        }

        private void Shoot()
        {
            _cooldown.Reset();
            _animation.SetClip("attack-start");
        }
    }
}