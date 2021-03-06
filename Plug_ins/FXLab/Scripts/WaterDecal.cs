using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WaterDecal : MonoBehaviour
{
    public float Duration = 1;
    public float MaxSize = 1;

    private float _elapsedTime = 0;

    void Awake()
    {
        this.transform.localScale = Vector3.one * MaxSize;
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Transparency", 0);
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Scale", 0);
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > Duration)
        {
            Destroy(this.gameObject);
            return;
        }

        GetComponent<Renderer>().sharedMaterial.SetFloat("_Transparency", 1 - _elapsedTime / Duration);
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Scale", _elapsedTime / Duration);
    }
}
