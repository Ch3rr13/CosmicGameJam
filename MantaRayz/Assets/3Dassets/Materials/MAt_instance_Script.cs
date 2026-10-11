using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Materials : MonoBehaviour
{
    public GameObject obj;
    public Material material;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        obj = this.gameObject;
        material = obj.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.color = color;
    }
}