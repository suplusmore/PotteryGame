using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class handDetect : MonoBehaviour
{
    private void Update()
    {
        Hand leftHand = Hands.Left;

        if(Hands.IsPinching(leftHand))
        {
            Debug.Log("The left hand is pinching");
        }
    }
}
