using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHandler : MonoBehaviour
{
    public Animator animator;
    public UnityEvent extraEvents;
    bool extraEvent = false, pass = false;

    [SerializeField] bool setOnStart = false;
    private void Start()
    {
        if (setOnStart)
            animator.SetBool("Open", true);
    }

    public void Toggle(bool yes)
    {
        extraEvent = yes;
    }

    public void SetBoolTrue(string name)
    {
        if (pass)
        {
            if (extraEvent)
                extraEvents.Invoke();
            pass = false;
        }

        animator.SetBool(name, true);
        pass = true;
    }

    public void SetBoolFalse(string name)
    {
        if (pass)
        {
            if (extraEvent)
                extraEvents.Invoke();
            pass = false;
        }

        animator.SetBool(name, false);
        pass = true;
    }

}
