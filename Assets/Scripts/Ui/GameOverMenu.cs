using TMPro;
using UnityEngine;

namespace ss
{
    public sealed class GameOverMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject inputField;

        [SerializeField]
        private TMP_Text finalScoreText;

        [SerializeField]
        private TMP_Text thanksText;

        [SerializeField]
        private Highscores highscore;

        private void Start()
        {
            highscore.NewHighscoreUploaded += Highscore_NewHighscoreUploaded;
        }

        private void Highscore_NewHighscoreUploaded(object sender, Highscores.NewHighscoreUploadedEventArgs e)
        {
            inputField.SetActive(false);

            thanksText.text = string.Format("Thanks, {0}", e.UserName);
            thanksText.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            finalScoreText.text = GameManager.Instance.CurrentScore.ToString();
        }

        public void AddNewHighscore(string playerName)
        {
            highscore.AddNewHighscore(HighscoresMenu.FormatUserNameInput(playerName), GameManager.Instance.CurrentScore);
        }
    }
}
