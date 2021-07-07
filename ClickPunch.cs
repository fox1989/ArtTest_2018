using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPunch : MonoBehaviour
{

    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;

    }
    Vector4 punchPos = Vector4.zero;

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject == go)
                {
                    Vector3 pos = go.transform.InverseTransformPoint(hit.point);
                    punchPos = new Vector4(pos.x, pos.y, pos.z, 0.8f);

                }
            }
        }

        if (punchPos.w > 0)
        {
            punchPos.w -= Time.deltaTime;
            punchPos.w = Mathf.Max(0, punchPos.w);
            Material material = go.GetComponent<MeshRenderer>().material;
            material.SetVector("_PunchPos", punchPos);

        }



    }


    public Material material;

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{

    //    //Graphics.Blit(source, destination, material);
    //}


}
