using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.ChangeScene
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExitLevel : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameManager.Instance.SceneIndexValueToLoad.index = (int)SceneDictionary.Hub;
                SceneManager.LoadScene((int)SceneDictionary.LoadingScene);
            }
        }
    }
}