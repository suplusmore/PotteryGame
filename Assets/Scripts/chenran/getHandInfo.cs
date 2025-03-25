using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

public class getHandInfo : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider;
    private Hand _hand;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < LeapServiceProvider.CurrentFrame.Hands.Count; i++)
        {
            _hand = LeapServiceProvider.CurrentFrame.Hands[i];
        }

        Hand leftHand = Hands.Left;
        Hand rightHand = Hands.Right;

        if(rightHand.IsRight && rightHand.IsPinching() )
        {
            Debug.Log("niehe");
        }


    }

    private void OnEnable()
    {
        LeapServiceProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        LeapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame)
    {
        //Debug.Log("We got a frame");
    }

    //private void OnEnable()
    //{
    //    LeapServiceProvider.OnUpdateFrame += OnUpdateFrame;

    //}
    //private void OnDisable()
    //{
    //    LeapServiceProvider.OnUpdateFrame -= OnUpdateFrame;
    //}

    //void OnUpdateFrame(Frame frame)
    //{
    //    Hand leftHand = Hands.Left;
    //    Hand rightHand = Hands.Right;

    //    Debug.Log(leftHand.WristPosition);

    //}

    //void OnUpdateHand(Hand _hand)
    //{

    //}
}
