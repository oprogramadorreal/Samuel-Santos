using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace ss
{
    public sealed class GameManager : SingletonMonoBehaviour<GameManager>, IAdsManagerCallback
    {
        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private GameObject musicNotesParticles;

        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private Transform musicTransform;

        [SerializeField]
        private Light2D musicLight;

        [SerializeField]
        private Transform playerBodyPosition;

        [SerializeField]
        private GameObject gameUI;

        [SerializeField]
        private GameObject gameOverUI;

        [SerializeField]
        private GameObject adsMenuUI;

        [SerializeField]
        private AdsManager adsManager;

        [SerializeField]
        private SlowMotionManager slowMotion;

        [SerializeField]
        private CinemachineShake cameraShake;

        [SerializeField]
        private GameObject playerPrefab;

        private PlayerController player;

        private const string BestScorePref = "UserBestScore";

        private AudioSource music;

        private int deathsCount = 0;

        private int currentScore = 0;

        private float maxMusicLightIntensity = 0;

        private const int scoreInitialPosition = 21;

        private int playerLayer;
        private int weaponLayer;

        private bool gameIsOver = false;

        public SlowMotionManager SlowMotion { get => slowMotion; }

        public CinemachineShake CameraShake { get => cameraShake; }

        public PlayerController Player { get => player; }

        public AudioSource Music { get => music; }

        public int CurrentScore { get => currentScore; }

        protected override void Awake()
        {
            base.Awake();

            playerLayer = LayerMask.NameToLayer("Player");
            weaponLayer = LayerMask.NameToLayer("Weapon");
        }

        private void Start()
        {
            player = TryToFindPlayer();

            if (player == null)
            {
                player = Instantiate(playerPrefab, playerBodyPosition.position, playerBodyPosition.rotation)
                    .GetComponent<PlayerController>();
            }

            maxMusicLightIntensity = musicLight.intensity;

            music = audioManager.CreateAudioSourceWithin(MusicSelector.SelectedMusicName, musicTransform);

            Physics2D.IgnoreLayerCollision(playerLayer, weaponLayer, false);
        }

        private void Update()
        {
            player = TryToFindPlayer();

            if (player == null)
            {
                return;
            }

            var newPlayerBodyPosition = player.BodyPosition;
            playerBodyPosition.position = newPlayerBodyPosition;

            var currentPosition = Mathf.FloorToInt(newPlayerBodyPosition.x);

            currentScore = Mathf.Max(currentScore, currentPosition - scoreInitialPosition);
            scoreText.SetText(currentScore.ToString());

            UpdateMusicNotesPosition(newPlayerBodyPosition);

            UpdateMusicLightIntensity(newPlayerBodyPosition);

            if (playerBodyPosition.position.y < -15.0f)
            {
                Destroy(player.gameObject);
                player = null;

                OnPlayerDied();
            }
        }

        private PlayerController TryToFindPlayer()
        {
            var currentPlayer = player;

            if (currentPlayer == null)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");

                if (playerObject != null)
                {
                    currentPlayer = playerObject.GetComponent<PlayerController>();
                }
            }

            return currentPlayer;
        }

        private void UpdateMusicNotesPosition(Vector2 playerBodyPosition)
        {
            var currentMusicPosition = musicNotesParticles.transform.position;

            if (playerBodyPosition.x > currentMusicPosition.x)
            {
                var newMusicPosition = new Vector3(playerBodyPosition.x, playerBodyPosition.y, currentMusicPosition.z) + Vector3.down * 3.0f;
                musicNotesParticles.transform.position = Vector3.Lerp(musicNotesParticles.transform.position, newMusicPosition, 0.1f);
                musicTransform.position = Vector3.Lerp(musicTransform.position, newMusicPosition, 0.5f);
            }
        }

        private void UpdateMusicLightIntensity(Vector2 playerBodyPosition)
        {
            var distanceInX = Mathf.Abs(playerBodyPosition.x - musicTransform.position.x);

            const float minLightDistance = 2.0f;
            const float maxLightDistance = 5.0f;

            const float minLightIntensity = 0.0f;

            var intensity = Scale(distanceInX, minLightDistance, maxLightDistance, minLightIntensity, maxMusicLightIntensity);
            intensity = Mathf.Clamp(intensity, minLightIntensity, maxMusicLightIntensity);

            musicLight.intensity = intensity;
        }

        private static float Scale(float xInAb, float a, float b, float u, float v)
        {
            return (xInAb - a) / (b - a) * (v - u) + u;
        }

        public void OnPlayerGiveUp()
        {
            if (!gameIsOver)
            {
                OnGameOver();
                gameOverUI.SetActive(true);
            }
        }

        public void OnPlayerDied()
        {
            if (!gameIsOver)
            {
                OnGameOver();

                const int lives = 2;

                if (++deathsCount < lives)
                {
                    adsMenuUI.SetActive(true);
                }
                else
                {
                    gameOverUI.SetActive(true);
                }
            }
        }

        private void OnGameOver()
        {
            music.Stop();
            audioManager.CreateTemporaryAudioSource("GameOver");
            UpdateBestScore();
            gameUI.SetActive(false);
            gameIsOver = true;
        }

        public void AdsMenuYes()
        {
            adsMenuUI.SetActive(false);

            if (!adsManager.ShowRewardedVideoAd(this))
            {
                gameOverUI.SetActive(true);
            }
        }

        public void AdsMenuNo()
        {
            adsMenuUI.SetActive(false);
            gameOverUI.SetActive(true);
        }

        private void UpdateBestScore()
        {
            var bestScore = GetBestScore();

            if (currentScore > bestScore)
            {
                PlayerPrefs.SetInt(BestScorePref, currentScore);
            }
        }

        public static int GetBestScore()
        {
            return PlayerPrefs.GetInt(BestScorePref, 0);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }

        void IAdsManagerCallback.AdWatched()
        {
            music.Play();
            RespawnPlayer();
            gameUI.SetActive(true);
            gameIsOver = false;
        }

        void IAdsManagerCallback.AdSkipped()
        {
            gameOverUI.SetActive(true);
        }

        private void RespawnPlayer()
        {
            // make sure there is no player active
            player = TryToFindPlayer();

            if (player != null)
            {
                Destroy(player.gameObject);
                player = null;
            }

            var respawnPos = new Vector3(playerBodyPosition.position.x - 20.0f, 30.0f, playerBodyPosition.position.z);
            Instantiate(playerPrefab, respawnPos, playerBodyPosition.rotation);

            player = TryToFindPlayer();
        }
    }
}
