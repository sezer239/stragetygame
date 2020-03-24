using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public delegate void OnSelectedChanged(Selectable self, bool isSelected);
    public event OnSelectedChanged selectedChanged;

    internal bool isSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
            selectedChanged.Invoke(this,_isSelected);
            //Replace this with your custom code. What do you want to happen to a Selectable when it get's (de)selected?
            Renderer r = GetComponentInChildren<Renderer>();
            if (r != null)
                r.material.color = value ? Color.red : Color.white;
        }
    }

    private bool _isSelected;


    void OnEnable()
    {
        RTSSelector.selectables.Add(this);
    }

    void OnDisable()
    {
        RTSSelector.selectables.Remove(this);
    }

}
