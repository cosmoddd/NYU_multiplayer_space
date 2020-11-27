using FirstGearGames.FlexSceneManager.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FirstGearGames.FlexSceneManager.Demos
{

    public class LoadingCanvas : MonoBehaviour
    {

        [SerializeField]
        private Image _loadingBar;

        private static LoadingCanvas _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            FlexSceneManager.OnSceneQueueStart += FlexSceneManager_OnSceneQueueStart;
            FlexSceneManager.OnSceneQueueEnd += FlexSceneManager_OnSceneQueueEnd;
            FlexSceneManager.OnLoadScenePercentChange += FlexSceneManager_OnLoadScenePercentChange;
            gameObject.SetActive(false);

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void FlexSceneManager_OnLoadScenePercentChange(LoadScenePercentEventArgs obj)
        {
            _loadingBar.fillAmount = obj.Percent;
        }

        private void FlexSceneManager_OnSceneQueueEnd()
        {
            gameObject.SetActive(false);
        }

        private void FlexSceneManager_OnSceneQueueStart()
        {
            _loadingBar.fillAmount = 0f;
            gameObject.SetActive(true);
        }


    }


}