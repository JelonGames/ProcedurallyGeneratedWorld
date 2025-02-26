using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SceneIndexValueToLoad", menuName = "Scriptable Objects/SceneIndexValueToLoad")]
    public class SceneIndexValueToLoad : ScriptableObject
    {
        public int index = 0;
    }
}