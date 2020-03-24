using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Basit npc dataları
/// </summary>
public class NPCData : Selectable
{
    public double los = 20;
    public Pickupable carryingItem {
        get {
            return _carryingItem;
        }
        set {
            _carryingItem = value;
            var chest = _carryingItem as Chest;

            if (chest != null)
            {
                _carryingItem.transform.SetParent(backpackTransform);
                _carryingItem.transform.localPosition = new Vector3(-0.233f, -0.368f, -0.434f);
                _carryingItem.transform.localRotation = Quaternion.Euler(-6.25f, -177.809f, -0.239f);
            }

        }
    }
    private Pickupable _carryingItem;

    public Transform weaponTransform;
    public Transform backpackTransform;

    public enum NPCStatus
    {
        WAITING_FOR_ORDERS,
        BUSY,
        BUSY_NOT_INTERRUPTIBLE
    }


    public delegate void OnNPCStatusChanged(NPCData npcdata, NPCStatus status);
    public delegate void OnWorkChanged(NPCData npcdata, Work work);


    public event OnNPCStatusChanged npcStatusChanged;
    public event OnWorkChanged onWorkChanged;



    public string npcName = "Temp name";

    public float walkSpeed = 3;
    public float runSpeed = 10;

    public bool isCheif {
        get {
            return (workingOn != null) && (workingOn.cheif.Equals(npcLogic));
        }
    }

    public Work workingOn {
        get {
            return _workingOn;
        }
        set {
            _workingOn = value;
            if (onWorkChanged != null)
                onWorkChanged.Invoke(this, _workingOn);
        }
    }
    private Work _workingOn;

    public JobQueue jobQueue;
    public NPCController npcController {
        get {
            if (_npcController == null)
            {
                _npcController = GetComponent<NPCController>();
            }
            return _npcController;
        }
    }
    private NPCController _npcController;

    public NPCLogic npcLogic {
        get {
            if (_npcLogic == null)
            {
                _npcLogic = GetComponent<NPCLogic>();
            }
            return _npcLogic;
        }
    }
    private NPCLogic _npcLogic;


    public NPCVisual npcVisual {
        get {
            if (_npcVisual == null)
            {
                _npcVisual = GetComponent<NPCVisual>();
            }
            return _npcVisual;
        }
    }
    private NPCVisual _npcVisual;

    public NPCStatus npcStatus {
        get {
            return _npcStatus;
        }
        set {
            _npcStatus = value;
            if (npcStatusChanged != null)
                npcStatusChanged.Invoke(this, _npcStatus);
        }
    }
    private NPCStatus _npcStatus;


    // Start is called before the first frame update
    void Awake()
    {
        jobQueue = new JobQueue();
        npcName = RandomNameGenerator.NameGenerator() + " " + RandomNameGenerator.NameGenerator();
        name = npcName;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
}
