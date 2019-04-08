using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHelper : MonoBehaviour
{

    private Animator myAnim;



    private void Awake()
    {
        myAnim = GetComponent<Animator>();
    }
}
