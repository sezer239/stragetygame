using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Work : MonoBehaviour
{
    public enum WorkType
    {
        WOOD_HARVEST_WORK,
        STONE_HARVEST_WORK,
        FOOD_HARVEST_WORK
    }

    public List<NPCLogic> npcsWorking;
    public NPCLogic cheif;

    public abstract WorkType GetWorkType();

    void Awake()
    {
        npcsWorking = new List<NPCLogic>();
    }

    public abstract void SetCheif(NPCLogic cheif);

    public abstract void AddWorker(NPCLogic worker);

    public abstract void RemoveWorker(NPCLogic worker);
    
}
