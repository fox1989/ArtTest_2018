using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnime : MonoBehaviour
{

    public WaterItem waterItem;
    public float waveFrequency = 0.53f;
    public float waveHeight = 0.48f;
    public float waveLength = 0.71f;

    public Material waterMaterial;
    
    List<WaterItem> waterItems = new List<WaterItem>();

    Vector3[] verts;
    public Mesh mesh;
    public MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {

        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        mesh = new Mesh();

        Dictionary<Vector3, int> v3index = new Dictionary<Vector3, int>();
        List<Vector3> vers = new List<Vector3>();
        List<Vector3> noramls = new List<Vector3>();
        List<Vector4> tangets = new List<Vector4>();
        Vector3[] dir = new Vector3[] {
          new Vector3(1,0,1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),};
        int index = 0;
        List<int> triangles = new List<int>();



        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Vector3 center = new Vector3(x, 0, y) * 2;
                vers.Add(center + new Vector3(1, 0, 1));
                vers.Add(center + new Vector3(1, 0, -1));
                vers.Add(center + new Vector3(-1, 0, -1));
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + 2);
                index += 3;
                vers.Add(center + new Vector3(-1, 0,  -1));
                vers.Add(center + new Vector3(-1, 0, 1));
                vers.Add(center + new Vector3(1, 0, 1));

                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + 2);
                index += 3;

                //waterItems.Add(go.GetComponent<WaterItem>());
            }

        }

        mesh.vertices = vers.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        verts = mesh.vertices;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshFilter.GetComponent<Renderer>().material = waterMaterial;

    }

    // Update is called once per frame
    void Update()
    {
        CalcWave();

    }


    void CalcWave()
    {


        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            v.y = 0.0f;
            float dist = Vector3.Distance(v, Vector3.zero);
            dist = (dist % waveLength) / waveLength;
            Vector3 tempV3 = transform.position + v;
            float dis = Vector3.Distance(Vector3.zero, tempV3);
            v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f + dis);
            verts[i] = v;
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;



        //for (int i = 0; i < waterItems.Count; i++)
        //{
        //    for (int j = 0; j < waterItems[i].verts.Length; j++)
        //    {
        //        Vector3 v = waterItems[i].verts[j];

        //        Vector3 tempV3 = waterItems[i].transform.position + v;
        //        float dis = Vector3.Distance(Vector3.zero, tempV3);
        //        v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f + dis);
        //        waterItems[i].verts[j] = v;
        //    }
        //    waterItems[i].mesh.vertices = waterItems[i].verts;
        //    waterItems[i].mesh.RecalculateNormals();
        //    waterItems[i].mesh.MarkDynamic();
        //    waterItems[i].meshFilter.mesh = waterItems[i].mesh;
        //}
    }
}
