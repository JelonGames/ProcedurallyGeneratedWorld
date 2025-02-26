using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnterWorld : MonoBehaviour
    {
        [SerializeField] private Scenes sceneToLoad = Scenes.Hub;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene((int)sceneToLoad);
            }
        }
    }

    public enum Scenes
    {
        Hub = 0,
        LoadingScene = 1
    }
}
