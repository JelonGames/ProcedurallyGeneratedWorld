using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldGenerator
{
    [CreateAssetMenu(fileName = "WorldObjectSettings", menuName = "Scriptable Objects/WorldObjectSettings")]
    public class WorldObjectSettings : ScriptableObject
    {
        public WorldObjectExit Exit;
        public List<WorldObject> worldObjects;
        public List<WorldObjectEnemies> Enemies;
    }
}
