using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMountain : MonoBehaviour
{
    public float stoneAmount { get { return _stoneAmount; } set { _stoneAmount = value; if (_stoneAmount <= 0) { Destroy(this.gameObject); } } }
    private float _stoneAmount = 2000;
}

