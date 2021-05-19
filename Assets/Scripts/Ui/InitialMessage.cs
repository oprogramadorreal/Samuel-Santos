using System.Collections;
using TMPro;
using UnityEngine;

namespace ss
{
    public sealed class InitialMessage : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text howFarText;

        private float fadeSpeed = 1.0f;

        private void Start()
        {
            Invoke(nameof(FadeOut), 4.0f);
        }

        private void FadeOut()
        {
            StartCoroutine(FadeOutImpl());
        }

        private IEnumerator FadeOutImpl()
        {
            while (howFarText.color.a > 0.0f)
            {
                var fadeAmount = howFarText.color.a - (fadeSpeed * Time.deltaTime);
                howFarText.color = new Color(howFarText.color.r, howFarText.color.g, howFarText.color.b, fadeAmount);
                yield return null;
            }
        }
    }
}
