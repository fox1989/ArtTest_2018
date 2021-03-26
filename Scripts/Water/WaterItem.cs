using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterItem : MonoBehaviour
{
    public Vector3[] verts;
    public Mesh mesh;
    public MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.vertices = new Vector3[] {
        new Vector3(1,0,1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),
        };
        mesh.triangles = new int[] { 0, 1, 3, 1, 2, 3 };
        mesh.RecalculateNormals();
        mesh.MarkDynamic();

        verts = mesh.vertices;
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // CalcWave();
    }

    void CalcWave()
    {
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            v.y = 0.0f;
            //float dist = Vector3.Distance(v, waveSource1);
            //dist = (dist % waveLength) / waveLength;
            Vector3 tempV3 = transform.position + v;
            float dis = Vector3.Distance(Vector3.zero, tempV3);
            v.y = 1 * Mathf.Sin(Time.time * Mathf.PI * 2.0f + dis);
            verts[i] = v;
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.MarkDynamic();

        GetComponent<MeshFilter>().mesh = mesh;
    }

}
