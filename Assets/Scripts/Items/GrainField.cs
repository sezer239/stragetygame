using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrainField : MonoBehaviour
{
    public float grainAmount { get { return _grainAmount; } set { _grainAmount = value; if (_grainAmount <= 0) { Destroy(this.gameObject); } } }
    private float _grainAmount = 100;
}

