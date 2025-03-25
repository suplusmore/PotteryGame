using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brushwenli : MonoBehaviour
{
    public GameObject brush;
    //public bool havecolor = false;
    private void OnTriggerEnter(Collider other)
    {
        //havecolor = true;
        Debug.Log("刷子上色");
        brush.GetComponent<MeshRenderer>().material.mainTexture = this.GetComponent<MeshRenderer>().material.mainTexture;
    }
}
