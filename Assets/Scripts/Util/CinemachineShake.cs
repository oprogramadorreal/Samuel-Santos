using Cinemachine;
using UnityEngine;

namespace ss
{
    /// <summary>
    /// Based on https://youtu.be/ACf1I27I6Tk
    /// </summary>
    public sealed class CinemachineShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerlin cinemachineCameraNoise;
        private float shakeTimer = 0.0f;
        private float shakeTimerTotal;
        private float startingIntensity;

        private void Awake()
        {
            var cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
            cinemachineCameraNoise = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update()
        {
            if (shakeTimer > 0.0f)
            {
                shakeTimer -= Time.deltaTime;
                cinemachineCameraNoise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0.0f, 1.0f - shakeTimer / shakeTimerTotal);
            }
        }

        public void ShakeCamera(float intensity, float time)
        {
            cinemachineCameraNoise.m_AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakeTimerTotal = time;
            shakeTimer = time;
        }
    }
}
