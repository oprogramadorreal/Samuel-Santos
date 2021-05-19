using UnityEngine;
using UnityEngine.Advertisements;

namespace ss
{
    public sealed class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        // This placement ID is created in the project's config at dashboard.unity3d.com
        private const string placementId = "rewardedVideo";

        private IAdsManagerCallback currentCallback = null;

        private void Start()
        {
            //Advertisement.AddListener(this);
            //Advertisement.Initialize("", true);
        }

        public bool ShowRewardedVideoAd(IAdsManagerCallback callback)
        {
            if (!Advertisement.IsReady(placementId) || currentCallback != null)
            {
                return false;
            }

            currentCallback = callback;
            Advertisement.Show(placementId);

            return true;
        }

        void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (currentCallback != null)
            {
                if (showResult == ShowResult.Finished)
                {
                    currentCallback.AdWatched();
                }
                else
                {
                    currentCallback.AdSkipped();
                }
            }

            currentCallback = null;
        }

        void IUnityAdsListener.OnUnityAdsDidError(string message) { }

        void IUnityAdsListener.OnUnityAdsDidStart(string placementId) { }

        void IUnityAdsListener.OnUnityAdsReady(string placementId) { }
    }
}
