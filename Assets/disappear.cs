using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject pottery;
    void Start()
    {
        Destroy(gameObject, 2.0f);
        GameObject.Find("TurnTable").GetComponent<UIcontrol>().turnstart();
        pottery.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 1);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
