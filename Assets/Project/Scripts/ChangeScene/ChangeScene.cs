using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.ChangeScene
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ChangeScene : MonoBehaviour
    {
        public SceneDictionary LoadScene;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameManager.Instance.SceneIndexValueToLoad.index = (int)LoadScene;
                SceneManager.LoadScene((int)SceneDictionary.LoadingScene);
            }
        }
    }
}