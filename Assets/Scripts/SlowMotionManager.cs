using Cinemachine;
using System;
using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/0VGosgaoTsw
    /// </summary>
    public sealed class SlowMotionManager : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 0.5f)]
        private float targetSlowMotionTimeScale = 0.25f;

        [SerializeField]
        private float slowdownFadeInLength = 1.0f;

        [SerializeField]
        private float slowdownFadeOutLength = 2.0f;

        [SerializeField]
        private float slowdownLength = 2.0f;

        [SerializeField]
        private Vector2 targetCameraPlayerOffset = Vector2.zero;

        [SerializeField]
        private float targetCameraOrthographicSize = 5.0f;

        [SerializeField]
        private CinemachineVirtualCamera gameCamera;

        private CinemachineFramingTransposer gameCameraTransposer;

        private Vector3 defaultCameraTrackedObjectOffset;
        private float defaultCameraOrthographicSize;
        private float defaultFixedDeltaTime;

        private Action updateFadeAction;

        private void Start()
        {
            defaultFixedDeltaTime = Time.fixedDeltaTime;

            gameCameraTransposer = gameCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            defaultCameraTrackedObjectOffset = gameCameraTransposer.m_TrackedObjectOffset;

            defaultCameraOrthographicSize = gameCamera.m_Lens.OrthographicSize;
        }

        private void Update()
        {
            updateFadeAction?.Invoke();
        }

        private void UpdateSlowMotionFadeIn()
        {
            var timeScaleStep = Time.unscaledDeltaTime / slowdownFadeInLength;
            var currentTimeScale = Time.timeScale - timeScaleStep;

            if (currentTimeScale <= targetSlowMotionTimeScale)
            {
                currentTimeScale = targetSlowMotionTimeScale;

                updateFadeAction = null;
                Invoke(nameof(StartFadeOut), currentTimeScale * slowdownLength);
            }

            UpdateTimeScale(currentTimeScale);
        }

        private void UpdateSlowMotionFadeOut()
        {
            var timeScaleStep = Time.unscaledDeltaTime / slowdownFadeOutLength;
            var currentTimeScale = Time.timeScale + timeScaleStep;

            if (currentTimeScale >= 1.0f)
            {
                currentTimeScale = 1.0f;
                updateFadeAction = null;
                gameCameraTransposer.m_TrackedObjectOffset = defaultCameraTrackedObjectOffset;
                gameCamera.m_Lens.OrthographicSize = defaultCameraOrthographicSize;
            }

            UpdateTimeScale(currentTimeScale);
        }

        private void StartFadeOut()
        {
            updateFadeAction = UpdateSlowMotionFadeOut;
        }

        private void UpdateTimeScale(float newTimeScale)
        {
            Time.timeScale = newTimeScale;
            Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;

            GameManager.Instance.Music.pitch = newTimeScale;

            UpdateCamera(newTimeScale);
        }

        private void UpdateCamera(float newTimeScale)
        {
            var t = Mathf.Clamp((newTimeScale - targetSlowMotionTimeScale) / (1.0f - targetSlowMotionTimeScale), 0.0f, 1.0f);

            gameCameraTransposer.m_TrackedObjectOffset = Vector3.Lerp(targetCameraPlayerOffset, defaultCameraTrackedObjectOffset, t);
            gameCamera.m_Lens.OrthographicSize = Mathf.Lerp(targetCameraOrthographicSize, defaultCameraOrthographicSize, t);
        }

        public void DoSlowMotion()
        {
            if (!IsDoingSlowMotion())
            {
                updateFadeAction = UpdateSlowMotionFadeIn;
            }
        }

        public bool IsDoingSlowMotion()
        {
            return Time.timeScale != 1.0f;
        }
    }
}
