using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMultiAnimation : MonoBehaviour
{
    
    [SerializeField] [Range(0, 60)] private int _frameRate = 10;
    [SerializeField] private UnityEvent<String> _onComplete;
    [SerializeField] private AnimationClip[] _clips;

    private SpriteRenderer _renderer;
    
    private float _secPerFrame;
    private float _nextFrameTime;
    private int _currentClip;
    private int _currentFrame;

    private bool _isPlaying = true;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _secPerFrame = 1f / _frameRate;

        StartAnimation();
    }

    private void OnBecameVisible()
    {
        enabled = _isPlaying;
    }
    
    private void OnBecameInvisible()
    {
        enabled = false;
    }

    public void SetClip(string clipName)
    {
        for (int i = 0; i < _clips.Length; i++)
        {
            if (_clips[i].Name == clipName)
            {
                _currentClip = i;
                StartAnimation();
                return;
            }
        }

        enabled = _isPlaying = false;
    }
    
    private void StartAnimation()
    {
        _nextFrameTime = Time.time + _secPerFrame;
        enabled = _isPlaying = true;
        _currentFrame = 0;
    }

    private void OnEnable()
    {
        _nextFrameTime = Time.time + _secPerFrame;
    }

    private void Update()
    {
        if (_nextFrameTime > Time.time)
            return;

        var clip = _clips[_currentClip];
        if (_currentFrame >= clip.Sprites.Length)
        {
            if (clip.Loop)
            {
                _currentFrame = 0;
            }
            else
            {
                enabled = _isPlaying = clip.AllowNextClip;
                clip.OnComplete?.Invoke();
                _onComplete?.Invoke(clip.Name);
                if (clip.AllowNextClip)
                {
                    _currentFrame = 0;
                    _currentClip = (int) Mathf.Repeat(_currentClip + 1, _clips.Length);
                }
            }
            
            return;
        }

        _renderer.sprite = clip.Sprites[_currentFrame];
        _nextFrameTime += _secPerFrame;
        _currentFrame++;
    }

    [Serializable]
    public class AnimationClip
    {
        public string Name;
        public Sprite[] Sprites;
        public bool Loop;
        public bool AllowNextClip;
        public UnityEvent OnComplete;
    }
}
