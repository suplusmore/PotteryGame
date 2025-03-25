using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] oldVertices = mesh.vertices;
        Vector3[] tempVertices = mesh.vertices;
        Vector3[] oldNormals = mesh.normals;

        for (var i = 0; i < oldVertices.Length; i++)
        {
            tempVertices[i] = oldVertices[i] * Mathf.Sin(Time.time);
        }

        mesh.vertices = tempVertices;
    }
}
