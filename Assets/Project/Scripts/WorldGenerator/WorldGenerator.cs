using Game.ChangeScene;
using Game.Managers;
using NavMeshPlus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Game.WorldGenerator
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int worldSize = new Vector2Int(50, 50);
        [SerializeField] private int worldSeed = 0;

        [SerializeField] private BoxCollider2D CamCollider;
        [SerializeField] private GameObject Player;
        [SerializeField] private NavMeshSurface NavMesh;

        [Header("TileMap")]
        [SerializeField] private Tilemap groundMap;
        [SerializeField] private Tilemap mountainMap;
        [SerializeField] private Tilemap waterMap;
        [SerializeField] private RuleTile defaultGround;
        [SerializeField] private RuleTile mountain;
        [SerializeField] private RuleTile water;

        [SerializeField, Range(0f, 100f)] private float mountainChance;
        [SerializeField, Range(0f, 100f)] private float waterChance;

        [SerializeField] private int minMountainTailCluster = 11;
        [SerializeField] private int minWaterTailCluster = 6;

        [Header("Objects to Generate")]
        [SerializeField] private WorldObjectSettings objectSettings;
        [SerializeField] private GameObject worldObjectParent;

        private float perlinTime = 0.1f;
        private Vector2 offset_terrain;
        private Vector2 offset_object;

        
        private void Start()
        {
            if(worldSeed == 0)
            {
                worldSeed = Random.Range(1000,9999);
            }
            Random.InitState(worldSeed);
            offset_terrain = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000, 10000));
            offset_object = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000, 10000));

            StartWorldGenerate();
        }

        private void StartWorldGenerate()
        {
            // Generate Terrain
            MakeGround();
            WorldBorder();
            GenerateMountainAndWater();

            // Clean small custer, connect close area, ...
            RemoveSmallMountain();
            RemoveSmallWater();
            ConnectIslandToMainLand();

            // Generate Objects
            GenereteObjects();

            // Ending process
            SetCamCollider();
            SetPlayer();
            NavMesh.BuildNavMesh();
        }

        [ContextMenu("Regenerate Map")]
        private void RegenerateMap()
        {
            GameManager.Instance.SceneIndexValueToLoad.index = (int)SceneDictionary.Forest;
            SceneManager.LoadScene((int)SceneDictionary.LoadingScene);
        }

        #region Generate Tarrain

        private void MakeGround()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Generete Ground");
            for ( int x = 0; x < worldSize.x; x++ )
            {
                for( int y = 0; y < worldSize.y; y++)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), defaultGround);
                }
            }
        }

        private void WorldBorder()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Generete Borders");
            for (int x = 0; x < worldSize.x; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, 0, 0);
                Vector3Int tilePosition2 = new Vector3Int(x, worldSize.y - 1, 0);
                Vector3Int tilePosition3 = new Vector3Int(x, -1, 0);
                Vector3Int tilePosition4 = new Vector3Int(x, worldSize.y, 0);
                mountainMap.SetTile(tilePosition, mountain);
                mountainMap.SetTile(tilePosition2, mountain);
                mountainMap.SetTile(tilePosition3, mountain);
                mountainMap.SetTile(tilePosition4, mountain);
            }

            for (int y = 0; y < worldSize.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(0, y, 0);
                Vector3Int tilePosition2 = new Vector3Int(worldSize.x - 1, y, 0);
                Vector3Int tilePosition3 = new Vector3Int(-1, y, 0);
                Vector3Int tilePosition4 = new Vector3Int(worldSize.x, y, 0);
                mountainMap.SetTile(tilePosition, mountain);
                mountainMap.SetTile(tilePosition2, mountain);
                mountainMap.SetTile(tilePosition3, mountain);
                mountainMap.SetTile(tilePosition4, mountain);
            }

            //Set corner tile
            Vector3Int tileCornerPosition = new Vector3Int(-1, -1, 0);
            mountainMap.SetTile(tileCornerPosition, mountain);
            tileCornerPosition = new Vector3Int(worldSize.x, -1, 0);
            mountainMap.SetTile(tileCornerPosition, mountain);
            tileCornerPosition = new Vector3Int(-1, worldSize.y, 0);
            mountainMap.SetTile(tileCornerPosition, mountain);
            tileCornerPosition = new Vector3Int(worldSize.x, worldSize.y, 0);
            mountainMap.SetTile(tileCornerPosition, mountain);
        }

        private void GenerateMountainAndWater()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Generete Mountain and Water");
            for (int x = 1; x < worldSize.x - 1; x++)
            {
                for (int y = 1; y < worldSize.y - 1; y++)
                {
                    float perlin = Mathf.PerlinNoise((x + offset_terrain.x) * perlinTime, (y + offset_terrain.y) * perlinTime);

                    if(perlin > (mountainChance / 100))
                        mountainMap.SetTile(new Vector3Int(x, y), mountain);

                    if (perlin < (waterChance / 100))
                    {
                        waterMap.SetTile(new Vector3Int(x, y), water);
                        mountainMap.SetTile(new Vector3Int(x, y), null);
                    }
                }
            }
        }

        #endregion

        #region Clean small custer, connect close area, ...

        private void RemoveSmallMountain()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Remove Small Mountain");
            bool[,] visited = new bool[worldSize.x + 1, worldSize.y + 1];

            for (int x = 1; x < worldSize.x - 1; x++)
            {
                for (int y = 1; y < worldSize.y - 1; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (mountainMap.GetTile((Vector3Int)pos) != null && !visited[x, y])
                    {
                        List<Vector2Int> cluster = new List<Vector2Int>();
                        CollectCluster(pos, cluster, visited, mountainMap);

                        if (cluster.Count < minMountainTailCluster)
                        {
                            foreach (var tile in cluster)
                            {
                                mountainMap.SetTile(new Vector3Int(tile.x, tile.y, 0), null);
                            }
                        }
                    }
                }
            }
        }

        private void RemoveSmallWater()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Remove Small Water");
            bool[,] visited = new bool[worldSize.x + 1, worldSize.y + 1];

            for (int x = 1; x < worldSize.x - 1; x++)
            {
                for (int y = 1; y < worldSize.y - 1; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (waterMap.GetTile((Vector3Int)pos) != null && !visited[x, y])
                    {
                        List<Vector2Int> cluster = new List<Vector2Int>();
                        CollectCluster(pos, cluster, visited, waterMap);

                        if (cluster.Count < minWaterTailCluster)
                        {
                            foreach (var tile in cluster)
                            {
                                waterMap.SetTile(new Vector3Int(tile.x, tile.y), null);
                            }
                        }
                    }
                }
            }
        }

        // Removing closed areas by connecting them to the largest area 
        #region Removeing closed areas
        private void ConnectIslandToMainLand()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Remove Closed Area");
            bool[,] visited = new bool[worldSize.x + 1, worldSize.y + 1];
            List<List<Vector2Int>> allClusters = new List<List<Vector2Int>>();
            List<Vector2Int> mainCluster = null;

            for (int x = 1; x < worldSize.x - 1; x++)
            {
                for (int y = 1; y < worldSize.y - 1; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if(GetCollisionTile((Vector3Int)pos) == null && !visited[x, y])
                    {
                        List<Vector2Int> cluster = new List<Vector2Int>();
                        CollectAllArea(pos, cluster, visited);
                        allClusters.Add(cluster);

                        if(mainCluster == null)
                        { 
                            mainCluster = cluster;
                            continue;
                        }

                        if(cluster.Count > mainCluster.Count)
                            mainCluster = cluster;
                    }
                }
            }
            foreach (List<Vector2Int> cluster in allClusters)
            {
                if (cluster != mainCluster)
                {
                    ConnectCluster(mainCluster, cluster);
                }
            }
        }


        private void ConnectCluster(List<Vector2Int> mainCluster, List<Vector2Int> cluster)
        {
            Vector2Int closestMain = mainCluster[0];
            Vector2Int closestCluster = cluster[0];
            float minDistance = float.MaxValue;

            foreach (Vector2Int mainPos in mainCluster)
            {
                foreach (Vector2Int clusterPos in cluster)
                {
                    float distance = Vector2.Distance(mainPos, clusterPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestMain = mainPos;
                        closestCluster = clusterPos;
                    }
                }
            }

            CreatePath(closestMain, closestCluster);
        }

        private void CreatePath(Vector2Int start, Vector2Int end)
        {
            Vector2Int current = start;
            int maxAttempts = 100;
            int attempts = 0;

            while (current != end && attempts < maxAttempts)
            {
                mountainMap.SetTile((Vector3Int)current, null);
                waterMap.SetTile((Vector3Int)current, null);

                List<Vector2Int> possibleDirections = new List<Vector2Int>();

                if (current.x < end.x) possibleDirections.Add(Vector2Int.right);
                if (current.x > end.x) possibleDirections.Add(Vector2Int.left);
                if (current.y < end.y) possibleDirections.Add(Vector2Int.up);
                if (current.y > end.y) possibleDirections.Add(Vector2Int.down);

                if (Random.value < 0.3f)
                {
                    possibleDirections.Add(Vector2Int.right);
                    possibleDirections.Add(Vector2Int.left);
                    possibleDirections.Add(Vector2Int.up);
                    possibleDirections.Add(Vector2Int.down);
                }

                Vector2Int chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                current += chosenDirection;

                current.x = Mathf.Clamp(current.x, 1, worldSize.x - 2);
                current.y = Mathf.Clamp(current.y, 1, worldSize.y - 2);

                attempts++;
            }
        }
        #endregion

        #endregion

        #region Generate Objects

        private void GenereteObjects()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Generete Objects");
            GenerateExits();
            GenerateMandatory();
            GenerateEnemies();

            for (int x = 0; x < worldSize.x; x++)
            {
                for(int y = 0; y < worldSize.y; y++)
                {
                    foreach (WorldObject obj in objectSettings.worldObjects.Where(n => !n.IsMandatory).ToList())
                    {
                        int id_object = objectSettings.worldObjects.IndexOf(obj);
                        float perlin = Mathf.PerlinNoise((x + offset_object.x + (id_object * 10)) * perlinTime, (y + offset_object.y + (id_object * 10)) * perlinTime);

                        if (perlin > (obj.MinMaxPerlinValue.x / 100) && perlin < (obj.MinMaxPerlinValue.y / 100))
                        {
                            Vector3Int pos = new Vector3Int(x, y);
                            if (IsTiteFree(pos, obj.Size) && CanPlaceObject(pos, obj.Size))
                            {
                                Vector3 newPos = new Vector3(
                                    pos.x + Random.Range(-.5f, .5f),
                                    pos.y + Random.Range(-.5f, .5f)
                                    );
                                GameObject pref = obj.Prefab[Random.Range(0, obj.Prefab.Count - 1)];

                                PlaceObject(pref, newPos, Quaternion.identity, worldObjectParent.transform);
                            }
                        }
                    }
                }
            }
        }

        private void GenerateExits()
        {
            int placedObject = 0;
            int attempts = 0;
            int maxAttempts = 100;
            WorldObjectExit obj = objectSettings.Exit;

            while (placedObject < 4)
            {
                if (attempts >= maxAttempts)
                    break;

                Vector3Int pos;
                switch (placedObject)
                {
                    case 0:
                        pos = new Vector3Int
                        (
                            Random.Range(1, worldSize.x - 1),
                            Random.Range(1, 5)
                        );
                        break;
                    case 1:
                        pos = new Vector3Int
                        (
                            Random.Range(1, worldSize.x - 1),
                            Random.Range(worldSize.y - 5, worldSize.y - 1)
                        );
                        break;
                    case 2:
                        pos = new Vector3Int
                        (
                            Random.Range(1, 5),
                            Random.Range(1, worldSize.y - 1)
                        );
                        break;
                    case 3:
                        pos = new Vector3Int
                        (
                            Random.Range(worldSize.x - 5, worldSize.x - 1),
                            Random.Range(1, worldSize.y - 1)
                        );
                        break;
                    default:
                        pos = new Vector3Int
                        (
                            Random.Range(1, worldSize.x - 1),
                            Random.Range(1, worldSize.y - 1)
                        );
                        break;
                }

                if (CanPlaceObject(pos, obj.Size) && IsTiteFree(pos, obj.Size))
                {
                    Instantiate(obj.Prefab, pos, Quaternion.identity, worldObjectParent.transform);
                    placedObject++;
                }

                attempts++;
            }
        }

        private void GenerateMandatory()
        {
            foreach(WorldObject obj in objectSettings.worldObjects.Where(n => n.IsMandatory).ToList())
            {
                int placedObject = 0;
                int attempts = 0;
                int maxAttempts = 100;

                while (attempts < maxAttempts && placedObject < obj.MaxCount)
                {
                    Vector3 pos = new Vector3(
                        Random.Range(1, worldSize.x - 1),
                        Random.Range(1, worldSize.y - 1)
                        );

                    if(IsTiteFree(pos, obj.Size) && CanPlaceObject(pos, obj.Size))
                    {
                        if (obj.Prefab.Count == 0)
                            Debug.LogWarning($"<color=yellow>GenerateWorld</color> Object: {obj.Name} have empty prefab list");
                        else if (obj.Prefab.Count == 1)
                        {
                            PlaceObject(obj.Prefab[0], pos, quaternion.identity, worldObjectParent.transform);
                        }
                        else
                        {
                            PlaceObject(obj.Prefab[Random.Range(0, obj.Prefab.Count - 1)], pos, quaternion.identity, worldObjectParent.transform);
                        }

                        placedObject++;
                    }

                    attempts++;
                }
            }
        }

        private void GenerateEnemies()
        {
            foreach (WorldObjectEnemies enemy in objectSettings.Enemies)
            {
                int attempts = 0;
                int maxAttemps = 100;
                int placedObject = 0;

                while (attempts < maxAttemps && placedObject <= enemy.Count.y /*max count*/)
                {
                    Vector3 pos = new Vector3(
                        Random.Range(1, worldSize.x - 1),
                        Random.Range(1, worldSize.y - 1)
                        );

                    if (IsTiteFree(pos, enemy.Size) && CanPlaceObject(pos, enemy.Size))
                    {
                        placedObject++;
                        PlaceObject(enemy.Prefab, pos, Quaternion.identity, worldObjectParent.transform);
                    }

                    attempts++;
                }
            }
        }

        private void PlaceObject(GameObject prefab, Vector3 pos, Quaternion quaternion, Transform worldObjectParent)
        {
            GameObject obj = Instantiate(prefab, pos, quaternion, worldObjectParent);
            // if sprite renderer is exits set order settings else object must have component with this function
            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer sRenderer))
                sRenderer.sortingOrder = -(int)(pos.y * 100);
        }

        #endregion

        #region Ending Process

        private void SetCamCollider()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Set Cam Collider");
            CamCollider.size = worldSize;
            CamCollider.offset = new Vector2((worldSize.x / 2), (worldSize.y / 2));
        }

        private void SetPlayer()
        {
            Debug.Log($"<color=green>GenerateWorld</color> Set Player");
            Vector2Int pos = new Vector2Int((int)(worldSize.x / 2), (int)(worldSize.y / 2));
            bool[,] visited = new bool[worldSize.x + 1, worldSize.y + 1];

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(pos);
            visited[pos.x, pos.y] = true;
            while (queue.Count > 0)
            {
                Vector2Int currentPos = queue.Dequeue();
                if (IsTiteFree(new Vector3(currentPos.x, currentPos.y), Vector2.one))
                {
                    Player.transform.position = new Vector3(currentPos.x, currentPos.y);
                    Player.SetActive(true);
                    return;
                }

                foreach (var v in GetNeighbors(currentPos))
                {
                    if (v.x < 0 || v.x > worldSize.x || v.y < 0 || v.y > worldSize.y)
                        continue;

                    if (!visited[v.x, v.y])
                    {
                        visited[v.x, v.y] = true;
                        queue.Enqueue(v);
                    }
                }
            }
        }

        #endregion

        #region Other

        // Get Tilebase from mountain or water tilemap
        private TileBase GetCollisionTile(Vector3Int pos)
        {
            if(mountainMap.GetTile(pos) != null)
                return mountainMap.GetTile(pos);
            else if (waterMap.GetTile(pos) != null)
                return waterMap.GetTile(pos);
            else 
                return null;
        }

        // Check that the collision plate is in the correct position for size
        private bool IsTiteFree(Vector3 pos, Vector2 size)
        {
            Vector3 minPos = pos - (Vector3)(size / 2);
            Vector3 maxPos = pos + (Vector3)(size / 2);

            for (int x = Mathf.RoundToInt(minPos.x) - 1; x < maxPos.x + 1; x++)
            {
                for (int y = Mathf.RoundToInt(minPos.y) - 1; y < maxPos.y + 1; y++)
                {
                    if (GetCollisionTile(new Vector3Int(x, y)) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CanPlaceObject(Vector3 position, Vector2 size)
        {
            Collider2D coll = Physics2D.OverlapBox(position, size, 0);
            return coll == null;
        }

        // Get neighbors tile
        private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            yield return pos + Vector2Int.up;
            yield return pos + Vector2Int.right;
            yield return pos + Vector2Int.down;
            yield return pos + Vector2Int.left;
        }

        // Collect colision tile cluster
        private void CollectCluster(Vector2Int start, List<Vector2Int> cluster, bool[,] visited, Tilemap map)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                if (visited[current.x, current.y]) continue;

                visited[current.x, current.y] = true;
                cluster.Add(new Vector2Int(current.x, current.y));

                foreach (Vector2Int neighbor in GetNeighbors(new Vector2Int(current.x, current.y)))
                {
                    if (neighbor.x < 0 || neighbor.x > worldSize.x || neighbor.y < 0 || neighbor.y > worldSize.y)
                        continue;

                    if (map.GetTile((Vector3Int)neighbor) != null && !visited[neighbor.x, neighbor.y])
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        // Collect all areas
        private void CollectAllArea(Vector2Int start, List<Vector2Int> cluster, bool[,] visited)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                if (visited[current.x, current.y]) continue;

                visited[current.x, current.y] = true;
                cluster.Add(current);

                foreach (Vector2Int neighbor in GetNeighbors(current))
                {
                    if (neighbor.x < 0 || neighbor.x > worldSize.x || neighbor.y < 0 || neighbor.y > worldSize.y)
                        continue;

                    if (GetCollisionTile((Vector3Int)neighbor) == null && !visited[neighbor.x, neighbor.y])
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        #endregion
    }
}
