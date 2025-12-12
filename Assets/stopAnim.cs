using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopAnim : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void stopAnimation()
    {
        anim.speed = 0;
    }
}
