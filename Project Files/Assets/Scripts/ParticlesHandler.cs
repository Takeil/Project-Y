using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesHandler : MonoBehaviour
{

    public void EnableParticles(GameObject parent)
    {
        parent.gameObject.SetActive(true);
    }

    public void DisableParticles(GameObject parent)
    {
        parent.gameObject.SetActive(false);
    }
}
