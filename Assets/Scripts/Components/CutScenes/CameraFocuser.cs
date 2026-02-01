using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Components.CutScenes
{
    public class CameraFocuser : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _mainCamera;
        [SerializeField] private CinemachineVirtualCamera _targetCamera;
        [SerializeField] private float _defaultDuration = 3f;

        private int originalMainCameraPriority = 0;
        private int originalTargetCameraPriority = 0;
        private Coroutine _currentFocusRoutine;

        private void Start()
        {
            originalMainCameraPriority = _mainCamera.Priority;
            originalTargetCameraPriority = _targetCamera.Priority;
        }

        public void Focus()
        {
            if (_currentFocusRoutine != null) StopCoroutine(_currentFocusRoutine);
            
            _mainCamera.Priority = originalMainCameraPriority;
            _targetCamera.Priority = originalMainCameraPriority + 10;
        }

        public void Unfocus()
        {
            if (_currentFocusRoutine != null) StopCoroutine(_currentFocusRoutine);
            
            _mainCamera.Priority = originalMainCameraPriority;
            _targetCamera.Priority = originalTargetCameraPriority;
        }

        public void FocusWithAutoReturn(float duration = -1f)
        {
            float time = duration > 0 ? duration : _defaultDuration;
            
            if (_currentFocusRoutine != null) 
                StopCoroutine(_currentFocusRoutine);
            
            _currentFocusRoutine = StartCoroutine(FocusRoutine(time));
        }

        private IEnumerator FocusRoutine(float delay)
        {
            Focus();
            yield return new WaitForSeconds(delay);
            Unfocus();
            _currentFocusRoutine = null;
        }

    }
}