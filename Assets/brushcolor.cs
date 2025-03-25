using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brushcolor : MonoBehaviour
{
    //给刷子上色
    public GameObject brush;
    public bool havecolor = false;
    private void OnTriggerEnter(Collider other)
    {
        havecolor = true;
        Debug.Log("刷子上色");
        brush.GetComponent<MeshRenderer>().material.color = this.GetComponent<MeshRenderer>().material.color;
        brush.GetComponent<MeshRenderer>().material.mainTexture = this.GetComponent<MeshRenderer>().material.mainTexture;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
