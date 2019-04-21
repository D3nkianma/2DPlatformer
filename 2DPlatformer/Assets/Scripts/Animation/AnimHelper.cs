using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimHelper : MonoBehaviour
{

    private Animator myAnim;

    private Action animCallback;


    private void Awake()
    {
        myAnim = GetComponent<Animator>();
    }



    public void PlayWalk()
    {
        if (myAnim.GetBool("Walking") == true)
            return;

        myAnim.SetBool("Walking", true);
    }

    public void StopWalk()
    {
        if (myAnim.GetBool("Walking") == false)
            return;

        myAnim.SetBool("Walking", false);
    }


    public void PlayOrStopAnimBool(string boolName, bool play = true)
    {

        if (myAnim.GetBool(boolName) == false && play == false)
            return;

        if (myAnim.GetBool(boolName) == true && play == true)
            return;

        myAnim.SetBool(boolName, play);
    }

    public void StartAnimTrigger(string trigger)
    {
        myAnim.SetTrigger(trigger);
    }

    public void SetAnimCallback(Action callback)
    {
        if (animCallback != callback)
            animCallback = callback;
    }


    public void RecieveAnimEvent(AnimationEvent param)
    {
        Debug.Log("I've got a thing to do " + param.stringParameter);


        if (animCallback != null)
            animCallback();
        
    }

}
