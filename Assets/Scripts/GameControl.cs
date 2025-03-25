using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class GameControl : MonoBehaviour
{
    public LeapServiceProvider spro;
    public GameObject pottery;
    public Frame f;
    //spro = FindObjectOfType<LeapServiceProvider>();
    // Start is called before the first frame update
    void Start()
    {
        f = spro.CurrentFrame;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Hand hand in f.Hands)
        {
            // 如果是左手
            if (hand.IsRight)
            {
                // 如果物体的距离与左手的距离小于1F
                Debug.Log("是右手");
                {
                    // 将手的位置与旋转赋值给当前物体的位置或旋转
                    pottery.transform.position = hand.PalmPosition.ToVector3(); //+ hand.PalmNormal.ToVector3() * (transform.localScale.y * .5f + .02f);
                    pottery.transform.rotation = hand.Basis.CalculateRotation()* Quaternion.Euler(0, 0, -90); 
                }
            }
        }
    }
}
