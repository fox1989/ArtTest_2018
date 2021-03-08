using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    GridLayout grid;

    public GameObject cubePrefab;
    public GameObject quad;


    // Start is called before the first frame update
    void Start()
    {

        grid = GetComponent<GridLayout>();

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
                GameObject cube = Instantiate(cubePrefab, transform);
                cube.transform.position = WorldToCenter(hit.point);
                cube.SetActive(true);
            }
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



    Vector3 CellToCenter(Vector3Int cell)
    {
        return grid.CellToWorld(cell) + grid.cellSize * 0.5f;
    }


    Vector3 WorldToCenter(Vector3 world)
    {
        return CellToCenter(grid.WorldToCell(world));
    }

}
