using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Options")]
public class ExplosiinParameters : ScriptableObject
{
    public float radius;

    public LayerMask layerMask;

    public ParticleSystem effectExplosion;
}
