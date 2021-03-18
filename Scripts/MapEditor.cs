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

    }



    [Serializable]
    public class MapArea
    {
        public int x;
        public int y;
        public List<ChunkData> chunks;
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