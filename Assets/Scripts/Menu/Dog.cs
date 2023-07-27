using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    [SerializeField] Animator anim;
    public void Toggle(bool ans)
    {
        anim.SetBool("On", ans);
    }

    public void Tap()
    {
        anim.SetTrigger("Tap");
    }
}
