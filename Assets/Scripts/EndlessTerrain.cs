using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EndlessTerrain : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDst;
    public Transform viewer;
    public Material mapMaterial;

    public static EndlessTerrain instance;
    private static MapGenerator mapGenerator;

    public static Vector2 viewerPosition;
    private Vector2 viewerPositionOld;
    private int chunkSize;
    private int chunksVisibleInViewDst;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = mapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformScale;
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        foreach (var chunk in terrainChunksVisibleLastUpdate)
        {
            chunk.SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        private GameObject meshObject;
        private Vector2 position;
        private Bounds bounds;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private NavMeshSurface navMeshSurface;
        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;
        private LODMesh collisionLODMesh;
        private MapData mapData;
        private bool mapDataReceived;
        private int previousLODIndex = -1;
        private GameObject objectParent;
        private List<ObjectGenerator.ObjectSpawnData> pendingObjects;
        private List<GameObject> spawnedObjects = new List<GameObject>();
        private bool hasGeneratedObjects = false;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            navMeshSurface = meshObject.AddComponent<NavMeshSurface>();
            navMeshSurface.collectObjects = CollectObjects.Children;
            meshRenderer.material = material;
            meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
            objectParent = new GameObject("Object Parent");
            objectParent.transform.parent = meshObject.transform;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }
            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;
            mapGenerator.RequestObjectData(mapData, position, OnObjectDataReceived);
        }

        void OnObjectDataReceived(List<ObjectGenerator.ObjectSpawnData> spawnDataList)
        {
            pendingObjects = spawnDataList;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (!mapDataReceived) return;

            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= maxViewDst;

            if (isVisible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold) lodIndex = i + 1;
                    else break;
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                        // THIS IS THE PERMANENT FIX FOR FLOATING OBJECTS
                        meshCollider.sharedMesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData);
                    }
                }

                if (collisionLODMesh.hasMesh && pendingObjects != null && !hasGeneratedObjects)
                {
                    hasGeneratedObjects = true;
                    EndlessTerrain.instance.StartCoroutine(SpawnObjectsAndBakeNavMesh());
                }

                terrainChunksVisibleLastUpdate.Add(this);
            }
            SetVisible(isVisible);
        }

        IEnumerator SpawnObjectsAndBakeNavMesh()
        {
            var objectsToSpawn = new List<ObjectGenerator.ObjectSpawnData>(pendingObjects);
            pendingObjects = null;

            for (int i = 0; i < objectsToSpawn.Count; i++)
            {
                var data = objectsToSpawn[i];
                Vector3 spawnPos = new Vector3(data.position.x, mapGenerator.terrainData.maxHeight * 2, data.position.y);

                if (Physics.Raycast(spawnPos, Vector3.down, out var hit, mapGenerator.terrainData.maxHeight * 4))
                {
                    GameObject spawnedObject = Instantiate(data.prefab, hit.point, data.rotation);
                    spawnedObject.transform.localScale = data.scale;
                    spawnedObject.transform.parent = objectParent.transform;
                    spawnedObjects.Add(spawnedObject);
                }
            }

            yield return new WaitForEndOfFrame();
            navMeshSurface.BuildNavMesh();
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
            // This logic is now simplified. When a chunk is hidden, its child objects
            // are hidden with it. When it becomes visible, they reappear.
            // When a chunk is truly destroyed, the objects will be destroyed with it.
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;
        private System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
        public bool useForCollider;
    }
}