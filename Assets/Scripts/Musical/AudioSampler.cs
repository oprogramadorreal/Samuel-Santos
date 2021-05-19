using UnityEngine;

namespace ss
{
    /// <summary>
    /// This code was adapted from https://assetstore.unity.com/packages/tools/audio/simplespectrum-free-audio-spectrum-generator-webgl-85294
    /// </summary>
    public sealed class AudioSampler : MonoBehaviour
    {
        // Should be equal or less than numberOfSpectrumSamples.
        [SerializeField]
        private int numberOfUserSamples = 100;

        // The number of samples to use when sampling. Must be a power of two.
        [SerializeField]
        private int numberOfInternalSamples = 256;

        [SerializeField]
        private float minValue = 0.0f;

        // The lower bound of the freuqnecy range to sample from. Leave at 0 when unused.
        [SerializeField]
        private float frequencyLimitLow = 0;

        // The upper bound of the freuqnecy range to sample from. Leave at 22050 (44100/2) when unused.
        [SerializeField]
        private float frequencyLimitHigh = 22050;

        // The audio channel to use when sampling.
        [SerializeField]
        private int sampleChannel = 0;

        // The FFTWindow to use when sampling.
        [SerializeField]
        private FFTWindow windowUsed = FFTWindow.BlackmanHarris;

        // If true, audio data is scaled logarithmically.
        [SerializeField]
        private bool useLogarithmicFrequency = true;

        // If true, the values of the spectrum are multiplied based on their frequency, to keep the values proportionate.
        [SerializeField]
        private bool multiplyByFrequency = true;

        // The amount of dampening used when the new scale is higher than the bar's existing scale.
        [Range(0, 1)]
        [SerializeField]
        private float attackDamp = 0.3f;

        // The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
        [Range(0, 1)]
        [Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
        [SerializeField]
        private float decayDamp = 0.15f;

        private float highestLogFreq, frequencyScaleFactor; // multiplier to ensure that the frequencies stretch to the highest record in the array.

        private float[] internalSamples;
        private float[] userSamples;

        public int NumberOfUserSamples { get => numberOfUserSamples; }

        public float[] UserSamples { get => userSamples; }

        private void Awake()
        {
            highestLogFreq = Mathf.Log(numberOfUserSamples + 1, 2); // gets the highest possible logged frequency, used to calculate which sample of the spectrum to use for a bar
            frequencyScaleFactor = 1.0f / (AudioSettings.outputSampleRate / 2) * numberOfInternalSamples;

            numberOfInternalSamples = Mathf.ClosestPowerOfTwo(numberOfInternalSamples);

            internalSamples = new float[numberOfInternalSamples];
            userSamples = new float[numberOfUserSamples];
        }

        private void Update()
        {
            UpdateSamples();
        }

        private void UpdateSamples()
        {
            AudioListener.GetSpectrumData(internalSamples, sampleChannel, windowUsed); // get the spectrum data

#if WEB_MODE
            var freqLim = frequencyLimitHigh * 0.76f; // AnalyserNode.getFloatFrequencyData doesn't fill the array, for some reason
#else          
            var freqLim = frequencyLimitHigh;
#endif

            for (var i = 0; i < numberOfUserSamples; ++i)
            {
                var prevValue = userSamples[i];
                var newValue = SampleSpectrumAt(i, freqLim);
                
                if (newValue > prevValue)
                {
                    newValue = Mathf.Lerp(prevValue, Mathf.Max(newValue, minValue), attackDamp);
                }
                else
                {
                    newValue = Mathf.Lerp(prevValue, Mathf.Max(newValue, minValue), decayDamp);
                }

                userSamples[i] = newValue;
            }
        }

        private float SampleSpectrumAt(int i, float freqLim)
        {
            float trueSampleIndex;

            if (useLogarithmicFrequency)
            {
                trueSampleIndex = Mathf.Lerp(frequencyLimitLow, freqLim, (highestLogFreq - Mathf.Log(numberOfUserSamples + 1 - i, 2)) / highestLogFreq) * frequencyScaleFactor;
            }
            else
            {
                trueSampleIndex = Mathf.Lerp(frequencyLimitLow, freqLim, ((float)i) / numberOfUserSamples) * frequencyScaleFactor;
            }

            var sampleIndexFloor = Mathf.FloorToInt(trueSampleIndex);
            sampleIndexFloor = Mathf.Clamp(sampleIndexFloor, 0, internalSamples.Length - 2); // just keeping it within the spectrum array's range

            var value = Mathf.SmoothStep(internalSamples[sampleIndexFloor], internalSamples[sampleIndexFloor + 1], trueSampleIndex - sampleIndexFloor); // smoothly interpolate between the two samples using the true index's decimal.

            if (multiplyByFrequency) // multiplies the amplitude by the true sample index
            {
#if WEB_MODE
                value = value * (Mathf.Log(trueSampleIndex + 1) + 1); // different due to how the WebAudioAPI outputs spectrum data.
#else
                value = value * (trueSampleIndex + 1);
#endif
            }

#if !WEB_MODE
            value = Mathf.Sqrt(value); // compress the amplitude values by sqrt(x)
#endif

            return value;
        }
    }
}
