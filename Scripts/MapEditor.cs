using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Map
{
    public class MapEditor : MonoBehaviour
    {

        GridLayout grid;

        public GameObject cubePrefab;
        public GameObject solpePrefab;
        public GameObject quad;

        public Chunk currSelectGo = null;

        public Material selectMat;
        public Material normalMat;


        private GameObject currPrefab;

        #region UI

        public GameObject cubeTypes;

        public GameObject brushTypes;

        public InputField saveName;


        #endregion




        public enum BrushType
        {
            Add,//添加
            Dle,//删掉
            Select,//选择
        }


        public enum CubeType
        {
            Cube,
            Slope,
        }


        public BrushType brushType;


        public CubeType cubeType;

        private Dictionary<Chunk, Vector3Int> chunks = new Dictionary<Chunk, Vector3Int>();
        private HashSet<Vector3Int> chunkCoords = new HashSet<Vector3Int>();

        // Start is called before the first frame update
        void Start()
        {
            grid = GetComponent<GridLayout>();


            InitCubeType();
            InitBrushType();
        }


        void InitCubeType()
        {

            Toggle[] cubeToggles = cubeTypes.GetComponentsInChildren<Toggle>();

            foreach (var item in cubeToggles)
            {
                item.onValueChanged.AddListener((bool b) =>
                {
                    if (b)
                    {
                        cubeType = (CubeType)System.Enum.Parse(typeof(CubeType), item.name);
                        SetCubeType();
                    }
                });
            }

        }

        void InitBrushType()
        {
            Toggle[] Toggles = brushTypes.GetComponentsInChildren<Toggle>();
            foreach (var item in Toggles)
            {
                item.onValueChanged.AddListener((bool b) =>
                {
                    if (b)
                    {
                        brushType = (BrushType)System.Enum.Parse(typeof(BrushType), item.name);
                    }
                });
            }
        }


        public void SetCubeType()
        {
            switch (cubeType)
            {
                case CubeType.Cube:
                    currPrefab = cubePrefab;
                    break;
                case CubeType.Slope:
                    currPrefab = solpePrefab;
                    break;
                default:
                    break;
            }

        }

        // Update is called once per frame
        void Update()
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool raycast = Physics.Raycast(ray, out hit, float.MaxValue);


            if (raycast)
            {
                Vector3 setPos = FindMinQuad(hit.point);
                quad.SetActive(true);
                quad.transform.position = setPos;
                Vector3 center = WorldToCenter(hit.point);
                quad.transform.LookAt(center);


            }
            else
            {
                quad.SetActive(false);
            }


            if (Input.GetMouseButtonDown(0))
            {
                if (raycast)
                {

                    switch (brushType)
                    {
                        case BrushType.Add:
                            Add(hit.point);
                            break;
                        case BrushType.Dle:
                            Dle(hit);
                            break;
                        case BrushType.Select:
                            Select(hit);
                            break;
                        default:
                            break;
                    }

                }
            }


            if (Input.GetKeyDown(KeyCode.Q))
            {
                Rotate90(-1);
            }


            if (Input.GetKeyDown(KeyCode.E))
            {
                Rotate90(1);

            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(CreateMap());
            }



            if (Input.GetKeyDown(KeyCode.F))
            {
                LoadMapData();
            }


            if (Input.GetKeyDown(KeyCode.D))
            {
                CreatMapByCoord(5, 6);
            }



        }



        void Rotate90(int dir)
        {
            if (currSelectGo != null)
            {
                currSelectGo.SetRot(dir);
                //Vector3 angle = currSelectGo.transform.localEulerAngles;
                //angle.y += 90 * dir;
                //currSelectGo.transform.localEulerAngles = angle;
            }
        }

        void Add(Vector3 hitPoint)
        {
            GameObject cube = Instantiate(currPrefab, transform);
            cube.transform.position = WorldToCenter(hitPoint);
            cube.SetActive(true);
            SelectGo(cube);
            cube.name = cubeType.ToString();
            Vector3Int key = grid.WorldToCell(hitPoint);

            Chunk chunk = cube.GetComponent<Chunk>();

            chunks.Add(chunk, key);
            chunkCoords.Add(key);
        }


        void Dle(RaycastHit hit)
        {
            if (hit.collider.gameObject.tag == "cube")
            {
                Chunk chunk = hit.collider.gameObject.GetComponent<Chunk>();
                chunkCoords.Remove(chunks[chunk]);

                chunks.Remove(chunk);
                Destroy(hit.collider.gameObject);
            }
        }

        private void Select(RaycastHit hit)
        {
            if (hit.collider.gameObject.tag == "cube")
            {
                SelectGo(hit.collider.gameObject);
            }
        }

        Vector3[] dirs = new Vector3[]
         {
         new Vector3(0,0,1),
         new Vector3(0,0,-1),
         new Vector3(0,1,0),
         new Vector3(0,-1,0),
         new Vector3(1,0,0),
         new Vector3(-1,0,0),
         };

        Vector3 FindMinQuad(Vector3 hitPoint)
        {
            Vector3 center = WorldToCenter(hitPoint);

            Vector3 minPos = Vector3.zero;
            float dis = float.MaxValue;

            foreach (var dir in dirs)
            {
                Vector3 pos = center + dir * grid.cellSize.x * 0.5f;
                float tdis = Vector3.Distance(pos, hitPoint);
                if (tdis < dis)
                {
                    minPos = pos;
                    dis = tdis;
                }
            }
            return minPos;
        }


        void SelectGo(GameObject go)
        {

            go.GetComponentInChildren<Renderer>().material = selectMat;

            if (currSelectGo != null)
            {
                currSelectGo.GetComponentInChildren<Renderer>().material = normalMat;

            }
            currSelectGo = go.GetComponent<Chunk>(); ;

        }


        Vector3 CellToCenter(Vector3Int cell)
        {
            return grid.CellToWorld(cell) + grid.cellSize * 0.5f;
        }


        Vector3 WorldToCenter(Vector3 world)
        {
            return CellToCenter(grid.WorldToCell(world));
        }


        public void Save()
        {
            MapArea mapArea = new MapArea();

            mapArea.chunks = new List<ChunkData>();
            foreach (var item in chunks)
            {
                if (IsSimplify(item.Value))
                    continue;

                mapArea.chunks.Add(item.Key.GetData());
            }

            string json = JsonUtility.ToJson(mapArea);
            string path = Application.streamingAssetsPath + "/" + saveName.text + ".json";
            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Debug.LogError("json:" + json);

            File.WriteAllText(path, json);
        }




        Vector3Int[] simplifyDir = new Vector3Int[] {
    new Vector3Int(1,0,0),
    new Vector3Int(-1,0,0),
    new Vector3Int(0,1,0),
    new Vector3Int(0,-1,0),
    new Vector3Int(0,0,1),
};

        /// <summary>
        /// 简化 
        /// </summary>
        private bool IsSimplify(Vector3Int key)
        {
            foreach (var item in simplifyDir)
            {
                if (!chunkCoords.Contains(key + item))
                    return false;
            }
            return true;
        }


        public void Load()
        {
            string path = Application.streamingAssetsPath + "/" + saveName.text + ".json";
            string json = File.ReadAllText(path);

            Debug.LogError("load json:" + json);

            MapArea mapArea = JsonUtility.FromJson<MapArea>(json);

            foreach (var item in mapArea.chunks)
            {
                GameObject prefab = Resources.Load(item.type) as GameObject;
                GameObject go = Instantiate(prefab, transform);

                go.SetActive(true);
                Chunk chunk = go.GetComponent<Chunk>();
                chunk.Init(item);


                Vector3Int key = grid.WorldToCell(go.transform.position);
                chunks.Add(chunk, key);
                chunkCoords.Add(key);
            }
        }



        /// <summary>
        /// 已经创建了的坐标
        /// </summary>
        Dictionary<Vector2Int, int> hasCoord = new Dictionary<Vector2Int, int>();


        public IEnumerator CreateMap()
        {
            Debug.LogError("createMap");

            mapData = new MapData();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    Vector3Int cell = new Vector3Int(x, y, 0);
                    Vector3 pos = grid.CellToWorld(cell);
                    pos.y = 9999;


                    Ray r = new Ray(pos, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Water")))
                    {

                        ChunkData chunk = new ChunkData();
                        chunk.type = "Cube";
                        chunk.coord = grid.WorldToCell(hit.point);
                        AddChunk(chunk);

                    }
                }
                yield return new WaitForEndOfFrame();
            }

            mapData.Dic2List();

            Replenish();


            string json = JsonUtility.ToJson(mapData);
            string path = Application.streamingAssetsPath + "/MapData.json";
            string dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Debug.LogError("save");
            File.WriteAllText(path, json);
        }

        private void AddChunk(ChunkData chunk, bool addHasCoord = true)
        {
            Vector2Int key = new Vector2Int(chunk.coord.x / 10, chunk.coord.y / 10);

            if (!mapData.areaData.ContainsKey(key))
            {
                MapArea mapArea = new MapArea();
                mapArea.x = key.x;
                mapArea.y = key.y;
                mapArea.chunks = new List<ChunkData>();
                mapData.areaData.Add(key, mapArea);

            }


            if (chunkCoords.Contains(chunk.coord))
                return;

            chunkCoords.Add(chunk.coord);

            mapData.areaData[key].chunks.Add(chunk);

            if (addHasCoord)
                AddHasCoord(chunk);

        }

        void AddHasCoord(ChunkData chunk)
        {
            Vector2Int hasKey = new Vector2Int(chunk.coord.x, chunk.coord.y);

            if (!hasCoord.ContainsKey(hasKey))
                hasCoord.Add(hasKey, chunk.coord.z);
            else
            {
                if (hasCoord[hasKey] > chunk.coord.z)
                    hasCoord[hasKey] = chunk.coord.z;
            }
        }



        Vector2Int[] replenishDir = new Vector2Int[] {
             new Vector2Int(1,0),
             new Vector2Int(0,-1),
             new Vector2Int(-1,0),
             new Vector2Int(0,1),

        };

        /// <summary>
        /// 补全
        /// </summary>
        void Replenish()
        {
            Debug.LogError("Replenish" + mapData.data.Count);
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    Vector2Int hasKey = new Vector2Int(x, y);
                    foreach (var dir in replenishDir)
                    {
                        Vector2Int tempHasKey = hasKey + dir;

                        if (hasCoord.ContainsKey(tempHasKey) && hasCoord[tempHasKey] > hasCoord[hasKey])
                        {
                            ///Debug.LogError("Replenish " + "tempHasKey:" + tempHasKey + ":" + hasCoord[tempHasKey] + "  hasKey:" + hasKey + ":" + hasCoord[hasKey]);
                            int tempz = hasCoord[tempHasKey];
                            int hasz = hasCoord[hasKey];
                            for (int z = tempz; z > hasz; z--)
                            {
                                ChunkData chunk = new ChunkData();
                                chunk.type = "Cube";
                                chunk.coord = new Vector3Int(tempHasKey.x, tempHasKey.y, z);
                                //Debug.LogError("Replenish" + chunk.coord);
                                AddChunk(chunk, false);
                            }

                        }
                    }
                }
            }
        }





        MapData mapData;

        public void LoadMapData()
        {
            string path = Application.streamingAssetsPath + "/MapData.json";

            string json = File.ReadAllText(path);

            mapData = JsonUtility.FromJson<MapData>(json);
            mapData.List2Dic();


            CreatMapByCoord(4, 5);

        }



        Dictionary<Vector3Int, Chunk> currMapData = new Dictionary<Vector3Int, Chunk>();
        List<MapArea> currMapArea = new List<MapArea>();
        Vector2Int[] mapAreaDir = new Vector2Int[] {
             new Vector2Int(1,0),
             new Vector2Int(0,-1),
             new Vector2Int(-1,0),
             new Vector2Int(0,1),
             new Vector2Int(1,1),
             new Vector2Int(-1,-1),
             new Vector2Int(1,-1),
             new Vector2Int(-1,1),
        };

        public void CreatMapByCoord(int x, int y)
        {
            Vector2Int key = new Vector2Int(x, y);

            List<MapArea> addMapArea = new List<MapArea>();
            List<MapArea> removeMapArea = new List<MapArea>();

            FindAddAndRemove(key, ref addMapArea, ref removeMapArea);

            RemoveMapArea(removeMapArea);
            AddMapArea(addMapArea);

        }



        public void FindAddAndRemove(Vector2Int key, ref List<MapArea> addMapArea, ref List<MapArea> removeMapArea)
        {

            if (mapData.areaData.ContainsKey(key))
            {
                List<MapArea> tempMapArea = new List<MapArea>();

                tempMapArea.Add(mapData.areaData[key]);

                foreach (var dir in mapAreaDir)
                {
                    Vector2Int tempKey = key + dir;
                    if (mapData.areaData.ContainsKey(tempKey))
                    {
                        tempMapArea.Add(mapData.areaData[tempKey]);

                    }
                }

                foreach (var item in tempMapArea)
                {
                    MapArea area = currMapArea.Find(a => a.x == item.x && a.y == item.y);
                    if (area == null)
                        addMapArea.Add(item);
                }

                foreach (var item in currMapArea)
                {
                    MapArea area = tempMapArea.Find(a => a.x == item.x && a.y == item.y);
                    if (area == null)
                        removeMapArea.Add(item);
                }


            }

        }



        void AddMapArea(List<MapArea> addList)
        {
            foreach (var area in addList)
            {
                foreach (var chunk in area.chunks)
                {
                    Vector3 center = CellToCenter(chunk.coord);
                    GameObject go = Instantiate(cubePrefab, transform);
                    go.transform.position = center;
                    go.SetActive(true);
                    if (!currMapData.ContainsKey(chunk.coord))
                        currMapData.Add(chunk.coord, go.GetComponent<Chunk>());
                }
                currMapArea.Add(area);
            }
        }

        void RemoveMapArea(List<MapArea> removeList)
        {
            foreach (var area in removeList)
            {
                foreach (var chunk in area.chunks)
                {
                    if (currMapData.ContainsKey(chunk.coord))
                    {
                        Chunk c = currMapData[chunk.coord];
                        currMapData.Remove(chunk.coord);
                        GameObject.Destroy(c.gameObject);
                    }
                }
            }



        }

    }







    [Serializable]
    public class MapData
    {
        public Dictionary<Vector2Int, MapArea> areaData = new Dictionary<Vector2Int, MapArea>();
        public List<MapArea> data = new List<MapArea>();


        public void Dic2List()
        {
            foreach (var item in areaData)
            {
                data.Add(item.Value);
            }

        }



        public void List2Dic()
        {
            foreach (var item in data)
            {
                Vector2Int key = new Vector2Int(item.x, item.y);
                if (!areaData.ContainsKey(key))
                {
                    areaData.Add(key, item);
                }
            }
        }
    }

    [Serializable]
    public class MapArea
    {
        public int x;
        public int y;
        public List<ChunkData> chunks = new List<ChunkData>();
    }
    [Serializable]
    public class ChunkData
    {
        public string type;
        public Vector3Int coord;
        public Vector3 pos;
        public Quaternion quaternion;
        /// <summary>
        /// 损耗值 ： 上 下 左 右
        /// </summary>
        public Vector4 dirLoss;
    }


}