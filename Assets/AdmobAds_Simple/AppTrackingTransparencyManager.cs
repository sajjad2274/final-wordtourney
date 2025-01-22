
    public class AppTrackingTransparencyManager : UnityEngine.MonoBehaviour
    {

        public void ShowATT(System.Action onComplete)
        {
#if UNITY_IOS && !UNITY_EDITOR
            StartCoroutine(ShowATTDelay(onComplete));
#else
            onComplete?.Invoke();
#endif
        }

        private System.Collections.IEnumerator ShowATTDelay(System.Action onComplete)
        {
#if UNITY_IOS
            Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
            yield return new UnityEngine.WaitUntil(() => Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus()!= 
                                                         Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED);
                
            onComplete?.Invoke();
#endif
            yield break;
        }

            
    }
        

