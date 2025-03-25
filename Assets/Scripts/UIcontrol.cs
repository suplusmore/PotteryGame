using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIcontrol : MonoBehaviour
{
    public GameObject table;
    public GameObject pottery;
    public int speed=1;
    public bool Isturn=true;//判断是否开始旋转
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Isturn)
        {
            table.transform.Rotate(0, speed * Time.time, 0, Space.Self);
            pottery.transform.Rotate(0, speed * Time.time, 0, Space.Self);
        }
    }
    public void turnstart()
    {
        Isturn = false;
        table.transform.Rotate(0, 0, 0, Space.Self);
        pottery.transform.Rotate(0, 0, 0, Space.Self);
    }
}
