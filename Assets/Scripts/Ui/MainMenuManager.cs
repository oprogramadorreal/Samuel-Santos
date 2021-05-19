using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ss
{
    public sealed class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private CinemachineShake cameraShake;

        [SerializeField]
        private TMP_Text musicButtonText;

        [SerializeField]
        private GameObject bestScoreUI;

        [SerializeField]
        private TMP_Text bestScoreText;

        private AudioSource menuMusic;

        private void Start()
        {
            PlaySelectedMusic();

            var bestScore = GameManager.GetBestScore();

            if (bestScore > 0)
            {
                bestScoreText.SetText(bestScore.ToString());
                bestScoreUI.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (menuMusic != null)
            {
                menuMusic.Stop();
            }
        }

        public void Play()
        {
            ButtonPressed();
            Invoke(nameof(PlayImpl), 0.3f);
        }

        private void PlayImpl()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void Quit()
        {
            ButtonPressed();
            Invoke(nameof(QuitImpl), 0.3f);
        }

        private void QuitImpl()
        {
            Application.Quit();
        }

        public void ChangeMusic()
        {
            ButtonPressed();
            MusicSelector.SelectNextMusic();
            PlaySelectedMusic();
        }

        private void PlaySelectedMusic()
        {
            StopCurrentMusic();

            menuMusic = audioManager.CreateAudioSource(MusicSelector.SelectedMusicName);
            musicButtonText.SetText(MusicSelector.SelectedMusicText);
        }

        private void StopCurrentMusic()
        {
            if (menuMusic != null)
            {
                menuMusic.Stop();

                Destroy(menuMusic);
                menuMusic = null;
            }
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
