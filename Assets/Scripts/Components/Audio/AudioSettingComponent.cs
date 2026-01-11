using System;
using Model.Data;
using Model.Data.Properties;
using UnityEngine;

namespace Components.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSettingComponent : MonoBehaviour
    {
        [SerializeField] private SoundSetting _mode;

        private AudioSource _source;
        private FloatPersistentProperty _model;
        
        private void Start()
        {
            _source = GetComponent<AudioSource>();
            
            _model = FindProperty();
            _model.OnChanged += OnSoundSettingChanged;
            OnSoundSettingChanged(_model.Value, _model.Value);
        }

        private void OnSoundSettingChanged(float newValue, float oldValue)
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
                
            _source.volume = newValue;
        }

        private FloatPersistentProperty FindProperty()
        {
            switch (_mode)
            {
                case SoundSetting.Music:
                    return GameSettings.I.Music;
                case SoundSetting.Sfx:
                    return GameSettings.I.Sfx;
            }

            throw new AggregateException("Undefined mode");
        }
    }
}