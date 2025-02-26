using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.WorldGenerator
{
    [Serializable]
    public class WorldObject
    {
        public string Name;
        public List<GameObject> Prefab = new List<GameObject>();
        public Vector2Int Size;
        public bool IsMandatory;
        public int MaxCount;
        [MinMaxRangeSlider(0, 100)] public Vector2 MinMaxPerlinValue;
    }

    [Serializable]
    public class WorldObjectExit
    {
        public string Name;
        public GameObject Prefab;
        public Vector2Int Size;
    }

    [Serializable]
    public class WorldObjectEnemies
    {
        public string Name;
        public GameObject Prefab;
        [MinMaxRangeSlider(0,50)] public Vector2 Count;
        public Vector2Int Size;
    }
}