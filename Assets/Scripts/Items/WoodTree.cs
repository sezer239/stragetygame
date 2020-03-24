using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTree : MonoBehaviour
{

    public float woodAmount {
        get { return _woodAmount; }
        set {
            _woodAmount = value;
            if (_woodAmount <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                anim.Play();
            }
        }
    }
    private float _woodAmount = 1000;

    private Animation anim;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }
}
