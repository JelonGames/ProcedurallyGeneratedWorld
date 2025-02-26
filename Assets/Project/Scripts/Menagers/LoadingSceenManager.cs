using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Managers
{
    public class LoadingSceenManager : MonoBehaviour
    {
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TextMeshProUGUI loadingText;

        private void Start()
        {
            StartCoroutine(LoadLevel());
        }

        public IEnumerator LoadLevel()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameManager.Instance.SceneIndexValueToLoad.index);
            asyncLoad.allowSceneActivation = false;

            int loadingStep = 0;

            string[] loadingSteps =
            {
                "Initializing World...",
                "Generating Tarrain...",
                "Placing Object...",
                "Setting up Lighting...",
                "Finalizing..."
            };

            loadingText.text = loadingSteps[loadingStep];

            while (!asyncLoad.isDone)
            {
                float process = Mathf.Clamp01(asyncLoad.progress/ .9f);

                // Debug log
                Debug.Log($"<color=green>Loading scene {SceneManager.GetSceneByBuildIndex(2).name}</color> {process}");

                loadingBar.value = Mathf.Clamp01(process);

                if(loadingStep < loadingSteps.Length && process >= (loadingStep + 1) * 0.2f)
                {
                    loadingStep++;
                    loadingText.text = loadingSteps[loadingStep];
                }

                if(process >= 1f)
                {
                    //loadingText.text = "Pres any key to continue...";
                    loadingBar.value = 1f;

                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
