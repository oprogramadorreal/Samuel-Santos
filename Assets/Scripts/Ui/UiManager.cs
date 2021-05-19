using UnityEngine;

namespace ss
{
    public sealed class UiManager : MonoBehaviour
    {
        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private CinemachineShake cameraShake;

        private void Start()
        {
            Cursor.visible = false;
        }

        public void FirePlayerWeapon()
        {
            GameManager.Instance.Player.FireWeapon();
        }

        public void UnequipPlayerWeapon()
        {
            GameManager.Instance.Player.UnequipWeapon();
        }

        public void ButtonPressed()
        {
            PlaySound();
            ShakeCamera();
        }

        private void PlaySound()
        {
            audioManager.CreateTemporaryAudioSource("MenuButton");
        }

        private void ShakeCamera()
        {
            cameraShake.ShakeCamera(10.0f, 0.2f);
        }
    }
}
