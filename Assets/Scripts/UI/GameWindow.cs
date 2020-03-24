using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameWindow : MonoBehaviour
{
    public delegate void OnShowed(Action action);
    public delegate void OnClosed(Action action);
    public delegate void OnFocus(Action action);
    public delegate void OnFocusLoss(Action action);

    public event OnShowed onShowed;
    public event OnClosed onClosed;
    public event OnFocus onFocus;
    public event OnFocusLoss onFocusLoss;

    private bool dragging;
    private Vector2 firstContactDifference;
    private bool firstContactChecked = false;

    private MoveableUI moveableUI;

    public bool enableDragging {
        get {
            return _enableDragging;
        }
        set {
            _enableDragging = false;
            if (moveableUI != null)
                moveableUI.enableDrag = _enableDragging;
        }
    }
    private bool _enableDragging = true;

    // Start is called before the first frame update
    void Start()
    {
        moveableUI = GetComponent<MoveableUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
