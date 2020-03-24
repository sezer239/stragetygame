using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestStoneWork : Work
{
    public enum HarvestStoneWorkStatus
    {
        INFO_COLLECT,
        TOOLS_NEEDED,
        STORAGE_FULL,
        WORKING
    }

    public delegate void OnAreaChanged(HarvestStoneWork work, Vector3 point, float radius);
    public delegate void OnStatusChanged(HarvestStoneWorkStatus status);
    public delegate void OnCheifChanged(NPCLogic currentCheif);

    public event OnAreaChanged onAreaChanged;
    public event OnCheifChanged onCheifChanged;
    public event OnStatusChanged onStatusChanged;

    public Transform[] dropOffLocation;

    public Transform cheifStandPosition;

    public Transform position;
    public float area;

    private System.Random random = new System.Random();

    public HarvestStoneWorkStatus status {
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

    public HarvestStoneWorkStatus _status;

    public List<StoneMountain> stones;


    void Start()
    {
        stones = new List<StoneMountain>();
  
        onAreaChanged += HarvestStoneWork_onAreaChanged;
        onCheifChanged += HarvestStoneWork_onCheifChanged;
    }

    private void HarvestStoneWork_onCheifChanged(NPCLogic currentCheif)
    {
        stones.Clear();
    
        status = HarvestStoneWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(position.position, area);
    }

    private void HarvestStoneWork_onAreaChanged(HarvestStoneWork work, Vector3 point, float radius)
    {
        stones.Clear();
 
        status = HarvestStoneWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(point, radius);
    }

    void Update()
    {
        switch (status)
        {
            case HarvestStoneWorkStatus.INFO_COLLECT: /*Do nothing */ break;
            case HarvestStoneWorkStatus.STORAGE_FULL: /*TODO*/ break;
            case HarvestStoneWorkStatus.TOOLS_NEEDED: /*TODO*/ break;
            case HarvestStoneWorkStatus.WORKING:
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    if (stones[i] == null)
                    {
                        stones.RemoveAt(i);
                    }
                }
                CheifRoutineJob();
            }
            break;
        }
    }

    public Job[] GetStoneMineJobArray()
    {
        StoneMountain StoneMountain = FindStoneMountain();
        if (StoneMountain == null) {
            Debug.LogAssertion("AAAAA FIX THISSSS");
            return null;
        }

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = StoneMountain.transform.position;

        Job stoneMineJob = new Job(Job.JobType.STONE_MINING);
        stoneMineJob.extra0 = StoneMountain;

        Job move2 = new Job(Job.JobType.MOVE);
        move2.position = dropOffLocation[0].position;

        Job dropJob = new Job(Job.JobType.ITEM_DROP);

        Job gotoJob = new Job(Job.JobType.GOTO);
        gotoJob.extra0 = cheif;

        Job reportBackJob = new Job(Job.JobType.REPORT_BACK);
        reportBackJob.extra0 = cheif;
        reportBackJob.extra1 = "Completed";

        return new Job[] { moveJob, stoneMineJob, move2, dropJob, gotoJob, reportBackJob }; 
    }

    public void GiveStoneMineJobToWorker(NPCLogic worker)
    {
        var jobs = GetStoneMineJobArray();
        if(jobs != null)
            foreach (var job in jobs) worker.AddJob(job);
    }

    private StoneMountain FindStoneMountain()
    {
        StoneMountain result = null;
        for (int i = 0; i < stones.Count; i++)
        {
            if (stones[i] == null)
            {
                stones.RemoveAt(i);
            }
            else
            {
                result = stones[random.Next(0,stones.Count)];
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
            var stoneMineJobArray = GetStoneMineJobArray();
            if (stoneMineJobArray == null) return;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = availbeWorker;

            Job giveOrderJob = new Job(Job.JobType.GIVEORDER);
            giveOrderJob.extra0 = availbeWorker.npcData;
            giveOrderJob.extra1 = stoneMineJobArray;


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
                StoneMountain stoneMountain;
                if ((stoneMountain = colliders[i].gameObject.GetComponent<StoneMountain>()) != null)
                {
                    Job moveJob = new Job(Job.JobType.MOVE);
                    moveJob.position = stoneMountain.transform.position;

                    Job examineJob = new Job(Job.JobType.ACTION);
                    Action actionAddTree = new Action(()=> {
                        stones.Add(stoneMountain);
                    });
                    examineJob.extra0 = actionAddTree;

                    cheif.AddJob(moveJob);
                    cheif.AddJob(examineJob);
                }
            }
            Job actionJob = new Job(Job.JobType.ACTION);
            Action action = new Action(()=>
            {
                status = HarvestStoneWorkStatus.WORKING;
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
        return WorkType.STONE_HARVEST_WORK;
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
