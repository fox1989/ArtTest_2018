using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{

    GridLayout grid;

    public GameObject cubePrefab;
    public GameObject solpePrefab;
    public GameObject quad;

    public GameObject currSelectGo = null;

    public Material selectMat;
    public Material normalMat;


    private GameObject currPrefab;


    public GameObject cubeTypes;

    public GameObject brushTypes;


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

            Vector3 angle = currSelectGo.transform.localEulerAngles;
            angle.y += 90 * dir;
            currSelectGo.transform.localEulerAngles = angle;
        }

    }

    void Add(Vector3 hitPoint)
    {
        GameObject cube = Instantiate(currPrefab, transform);
        cube.transform.position = WorldToCenter(hitPoint);
        cube.SetActive(true);
        SelectGo(cube);
    }


    void Dle(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "cube")
        {
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

        go.GetComponent<Renderer>().material = selectMat;

        if (currSelectGo != null)
        {
            currSelectGo.GetComponent<Renderer>().material = normalMat;

        }
        currSelectGo = go;

    }


    Vector3 CellToCenter(Vector3Int cell)
    {
        return grid.CellToWorld(cell) + grid.cellSize * 0.5f;
    }


    Vector3 WorldToCenter(Vector3 world)
    {
        return CellToCenter(grid.WorldToCell(world));
    }


}
