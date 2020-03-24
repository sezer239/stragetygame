using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnWorkAdded(Work work);
    public delegate void OnWorkDeleted(Work work);


    public event OnWorkAdded onWorkAdded;
    public event OnWorkDeleted onWorkDeleted;

    public static GameManager Instance {
        get {
            return _instance;
        }
    }
    private static GameManager _instance;

    public HarvestStoneWork harvestStoneWorkRef;
    public HarvestWoodWork harvestWoodWorkRef;

    public GameObject cameraOrigin;
    public NPCLogic[] npcLogics;


    private List<Work> works;

    void AddWork(Work work)
    {
        if (works.Contains(work)) return;
        works.Add(work);
        if (onWorkAdded != null)
            onWorkAdded.Invoke(work);
    }

    public void RemoveWork(Work work)
    {
        if (works.Contains(work))
        {
            works.Remove(work);
            Destroy(work.gameObject);
            if (onWorkDeleted != null)
                onWorkDeleted.Invoke(work);
        }
    }

    public HarvestStoneWork CreateHarvestStoneWork()
    {
        var work = Instantiate(harvestStoneWorkRef,cameraOrigin.transform.position, Quaternion.identity);
        AddWork(work);
        return work;
    }

    public HarvestWoodWork CreateHarvestWoodWork()
    {
        var work = Instantiate(harvestWoodWorkRef, cameraOrigin.transform.position, Quaternion.identity);
        AddWork(work);
        return work;
    }

    // Start is called before the first frame update
    void Start()
    {
        works = new List<Work>();
        _instance = this;
        npcLogics = FindObjectsOfType<NPCLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
