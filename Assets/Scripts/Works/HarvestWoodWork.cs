using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestWoodWork : Work
{
    public enum HarvestWoodWorkStatus
    {
        INFO_COLLECT,
        TOOLS_NEEDED,
        STORAGE_FULL,
        WORKING
    }

    public delegate void OnAreaChanged(HarvestWoodWork work, Vector3 point, float radius);
    public delegate void OnStatusChanged(HarvestWoodWorkStatus status);
    public delegate void OnCheifChanged(NPCLogic currentCheif);

    public event OnAreaChanged onAreaChanged;
    public event OnCheifChanged onCheifChanged;
    public event OnStatusChanged onStatusChanged;

    public Transform[] dropOffLocation;

    public Transform cheifStandPosition;

    public Transform position;
    public float area;

    private System.Random random = new System.Random();

    public HarvestWoodWorkStatus status {
        get {
            return _status;
        }
        set {
            _status = value;
            if (onStatusChanged != null)
            {
                onStatusChanged.Invoke(_status);
            }
        }
    }

    public HarvestWoodWorkStatus _status;

    public List<WoodTree> woodTrees;


    void Start()
    {
        woodTrees = new List<WoodTree>();
  
        onAreaChanged += HarvestWoodWork_onAreaChanged;
        onCheifChanged +=HarvestWoodWork_onCheifChanged;
    }

    private void HarvestWoodWork_onCheifChanged(NPCLogic currentCheif)
    {
        woodTrees.Clear();
    
        status = HarvestWoodWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(position.position, area);
    }

    private void HarvestWoodWork_onAreaChanged(HarvestWoodWork work, Vector3 point, float radius)
    {
        woodTrees.Clear();
 
        status = HarvestWoodWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(point, radius);
    }

    void Update()
    {
        switch (status)
        {
            case HarvestWoodWorkStatus.INFO_COLLECT: /*Do nothing */ break;
            case HarvestWoodWorkStatus.STORAGE_FULL: /*TODO*/ break;
            case HarvestWoodWorkStatus.TOOLS_NEEDED: /*TODO*/ break;
            case HarvestWoodWorkStatus.WORKING:
            {
                for (int i = 0; i < woodTrees.Count; i++)
                {
                    if (woodTrees[i] == null)
                    {
                        woodTrees.RemoveAt(i);
                    }
                }
                CheifRoutineJob();
            }
            break;
        }
    }

    public Job[] GetWoodCutJobArray()
    {
        WoodTree woodTree = FindWoodTree();
        if (woodTree == null) {
            Debug.LogAssertion("AAAAA FIX THISSSS");
            return null;
        }

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = woodTree.transform.position;

        Job woodCutJob = new Job(Job.JobType.WOOD_CUTTING);
        woodCutJob.extra0 = woodTree;

        Job move2 = new Job(Job.JobType.MOVE);
        move2.position = dropOffLocation[0].position;

        Job dropJob = new Job(Job.JobType.ITEM_DROP);

        Job gotoJob = new Job(Job.JobType.GOTO);
        gotoJob.extra0 = cheif;

        Job reportBackJob = new Job(Job.JobType.REPORT_BACK);
        reportBackJob.extra0 = cheif;
        reportBackJob.extra1 = "Completed";

        return new Job[] { moveJob, woodCutJob, move2, dropJob, gotoJob, reportBackJob }; 
    }

    public void GiveWoodCutJobToWorker(NPCLogic worker)
    {
        var jobs = GetWoodCutJobArray();
        if(jobs != null)
            foreach (var job in jobs) worker.AddJob(job);
    }

    private WoodTree FindWoodTree()
    {
        WoodTree result = null;
        for (int i = 0; i < woodTrees.Count; i++)
        {
            if (woodTrees[i] == null)
            {
                woodTrees.RemoveAt(i);
            }
            else
            {
                result = woodTrees[random.Next(0,woodTrees.Count)];
                break;
            }
        }
        return result;
    }

    private void CheifRoutineJob()
    {
        var availbeWorker = FindAvalibleWorker();
        if (Vector3.Distance(cheif.transform.position , cheifStandPosition.position) > 3 && availbeWorker == null)
        {
            Job moveJob = new Job(Job.JobType.MOVE);
            moveJob.position = cheifStandPosition.position;

            cheif.SetJob(moveJob);
        }else if (availbeWorker != null && cheif.npcData.jobQueue.jobs.Count == 0)
        {
            var woodCutJobArray = GetWoodCutJobArray();
            if (woodCutJobArray == null) return;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = availbeWorker;

            Job giveOrderJob = new Job(Job.JobType.GIVEORDER);
            giveOrderJob.extra0 = availbeWorker.npcData;
            giveOrderJob.extra1 = woodCutJobArray;


            cheif.AddJob(gotoJob);
            cheif.AddJob(giveOrderJob);
        }
    }

    private void InfoCollectJob(Vector3 point, float radius)
    {
        if (area != 0 && !position.Equals(Vector3.zero))
        {
            var colliders = Physics.OverlapSphere(point, radius);

            for (int i = 0; i < colliders.Length; i++)
            {
                WoodTree woodTree;
                if ((woodTree = colliders[i].gameObject.GetComponent<WoodTree>()) != null)
                {
                    Job moveJob = new Job(Job.JobType.MOVE);
                    moveJob.position = woodTree.transform.position;

                    Job examineJob = new Job(Job.JobType.ACTION);
                    Action actionAddTree = new Action(()=> {
                        woodTrees.Add(woodTree);
                    });
                    examineJob.extra0 = actionAddTree;

                    cheif.AddJob(moveJob);
                    cheif.AddJob(examineJob);
                }
            }
            Job actionJob = new Job(Job.JobType.ACTION);
            Action action = new Action(()=>
            {
                status = HarvestWoodWorkStatus.WORKING;
            });
            actionJob.extra0 = action;
            cheif.AddJob(actionJob);
        }
    }

    public NPCLogic FindAvalibleWorker()
    {
        foreach (var worker in npcsWorking) // Çalışma grubundaki tüm işçiler
        {
            if (worker.npcData.jobQueue.jobs.Count == 0) // işçinin JobQueue de bir işi yoksa
            {
                return worker; //İşçiyi Returnla
            }
        }

        return null;
    }

    public void SetArea(Transform pos, float area)
    {
        this.position = pos;
        this.area = area;
        if (onAreaChanged != null)
        {
            onAreaChanged.Invoke(this, this.position.position, this.area);
        }
    }

    public override WorkType GetWorkType()
    {
        return WorkType.WOOD_HARVEST_WORK;
    }

    public override void SetCheif(NPCLogic cheif)
    {
        this.cheif = cheif;
        this.cheif.npcData.workingOn = this;
        if (onCheifChanged != null)
            onCheifChanged.Invoke(this.cheif);
    }

    public override void AddWorker(NPCLogic worker)
    {
        if (npcsWorking.Contains(worker) && !worker.Equals(cheif)) return;

        npcsWorking.Add(worker);
        worker.npcData.workingOn = this;
    }

    public override void RemoveWorker(NPCLogic worker)
    {
        npcsWorking.Remove(worker);
        worker.npcData.workingOn = null;
    }
}
