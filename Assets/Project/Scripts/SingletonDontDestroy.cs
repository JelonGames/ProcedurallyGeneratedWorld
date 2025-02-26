using Game.Managers;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    public class SingletonDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
