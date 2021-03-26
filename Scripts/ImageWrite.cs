using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWrite : MonoBehaviour
{

    public Texture2D texture2D;
    public Color brushColor;
    public float brushSize = 2f;
    // Start is called before the first frame update
    void Start()
    {

        texture2D = new Texture2D(500, 500, TextureFormat.ARGB32, false, false);
        GetComponent<Renderer>().material.mainTexture = texture2D;

        Clear();
        
    }

    public void Clear()
    {
        for (int x = 0; x < texture2D.width; x++)
        {
            for (int y = 0; y < texture2D.height; y++)
            {
                texture2D.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }
        texture2D.Apply();

    }

    Vector3Int prevPos = Vector3Int.one * -1;
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 999) && Input.GetMouseButton(0))
        {
            Vector3 localPos = transform.InverseTransformPoint(hit.point);

            localPos = Vector3.one * 0.5f + localPos;
            localPos.x = texture2D.width * localPos.x;
            localPos.y = texture2D.height * localPos.y;
            Vector3Int pos = new Vector3Int((int)localPos.x, (int)localPos.y, 0);
            DrawLine(prevPos, pos);
            prevPos = pos;
            texture2D.Apply();
        }
        if (Input.GetMouseButtonUp(0))
        {
            prevPos = Vector3Int.one * -1;
        }
    }




    public void DrawLine(Vector3Int pos1, Vector3Int pos2)
    {
        if (pos1 == Vector3Int.one * -1)
            return;

        float dis = Vector3Int.Distance(pos1, pos2);

        for (int x = 0; x < dis; x++)
        {
            Vector3 p = Vector3.Lerp(pos1, pos2, x / dis);

            // float tempBrushSize = Mathf.Abs(x - dis) / dis + 0.5f;

            //tempBrushSize = brushSize * tempBrushSize;
            DrawPos(new Vector3Int((int)p.x, (int)p.y, 0), brushSize);
        }
    }


    public void DrawPos(Vector3Int center, float r)
    {

        DrawPoint(center);

        //int intR = (int)(r * 0.5f);
        //for (int x = -intR; x < intR; x++)
        //{
        //    for (int y = -intR; y < intR; y++)
        //    {
        //        Vector3Int tPos = center + new Vector3Int(x, y, 0);
        //        if (Vector3Int.Distance(center, tPos) < r)
        //        {
        //            DrawPoint(tPos);
        //        }

        //    }
        //}

    }

    public void DrawPoint(Vector3Int point)
    {
        if (point.x > 0 && point.x < texture2D.width && point.y > 0 && point.y < texture2D.height)
            texture2D.SetPixel(point.x, point.y, brushColor);
    }


}

