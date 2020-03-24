using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestFarmWork : Work
{
    public enum HarvestFarmWorkStatus
    {
        INFO_COLLECT,
        TOOLS_NEEDED,
        STORAGE_FULL,
        WORKING
    }

    public delegate void OnAreaChanged(HarvestFarmWork work, Vector3 point, float radius);
    public delegate void OnStatusChanged(HarvestFarmWorkStatus status);
    public delegate void OnCheifChanged(NPCLogic currentCheif);

    public event OnAreaChanged onAreaChanged;
    public event OnCheifChanged onCheifChanged;
    public event OnStatusChanged onStatusChanged;

    public Transform[] dropOffLocation;

    public Transform cheifStandPosition;

    public Transform position;
    public float area;

    private System.Random random = new System.Random();

    public HarvestFarmWorkStatus status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;
            if (onStatusChanged != null)
            {
                onStatusChanged.Invoke(_status);
            }
        }
    }

    public HarvestFarmWorkStatus _status;

    public List<GrainField> food;


    void Start()
    {
        food = new List<GrainField>();

        onAreaChanged += HarvestFarmWork_onAreaChanged;
        onCheifChanged += HarvestFarmWork_onCheifChanged;
    }

    private void HarvestFarmWork_onCheifChanged(NPCLogic currentCheif)
    {
        food.Clear();

        status = HarvestFarmWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(position.position, area);
    }

    private void HarvestFarmWork_onAreaChanged(HarvestFarmWork work, Vector3 point, float radius)
    {
        food.Clear();

        status = HarvestFarmWorkStatus.INFO_COLLECT;
        if (cheif != null)
            InfoCollectJob(point, radius);
    }

    void Update()
    {
        switch (status)
        {
            case HarvestFarmWorkStatus.INFO_COLLECT: /*Do nothing */ break;
            case HarvestFarmWorkStatus.STORAGE_FULL: /*TODO*/ break;
            case HarvestFarmWorkStatus.TOOLS_NEEDED: /*TODO*/ break;
            case HarvestFarmWorkStatus.WORKING:
                {
                    for (int i = 0; i < food.Count; i++)
                    {
                        if (food[i] == null)
                        {
                            food.RemoveAt(i);
                        }
                    }
                    CheifRoutineJob();
                }
                break;
        }
    }

    public Job[] GetFoodFarmJobArray()
    {
        GrainField GrainField = Findfoodfield();
        if (GrainField == null)
        {
            Debug.LogAssertion("AAAAA FIX THISSSS");
            return null;
        }

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = GrainField.transform.position;

        Job foodFarmJob = new Job(Job.JobType.FARMING);
        foodFarmJob.extra0 = GrainField;

        Job move2 = new Job(Job.JobType.MOVE);
        move2.position = dropOffLocation[0].position;

        Job dropJob = new Job(Job.JobType.ITEM_DROP);

        Job gotoJob = new Job(Job.JobType.GOTO);
        gotoJob.extra0 = cheif;

        Job reportBackJob = new Job(Job.JobType.REPORT_BACK);
        reportBackJob.extra0 = cheif;
        reportBackJob.extra1 = "Completed";

        return new Job[] { moveJob, foodFarmJob, move2, dropJob, gotoJob, reportBackJob };
    }

    public void GiveFoodFarmJobToWorker(NPCLogic worker)
    {
        var jobs = GetFoodFarmJobArray();
        if (jobs != null)
            foreach (var job in jobs) worker.AddJob(job);
    }

    private GrainField Findfoodfield()
    {
        GrainField result = null;
        for (int i = 0; i < food.Count; i++)
        {
            if (food[i] == null)
            {
                food.RemoveAt(i);
            }
            else
            {
                result = food[random.Next(0, food.Count)];
                break;
            }
        }
        return result;
    }

    private void CheifRoutineJob()
    {
        var availbeWorker = FindAvalibleWorker();
        if (Vector3.Distance(cheif.transform.position, cheifStandPosition.position) > 3 && availbeWorker == null)
        {
            Job moveJob = new Job(Job.JobType.MOVE);
            moveJob.position = cheifStandPosition.position;

            cheif.SetJob(moveJob);
        }
        else if (availbeWorker != null && cheif.npcData.jobQueue.jobs.Count == 0)
        {
            var foodFarmJobArray = GetFoodFarmJobArray();
            if (foodFarmJobArray == null) return;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = availbeWorker;

            Job giveOrderJob = new Job(Job.JobType.GIVEORDER);
            giveOrderJob.extra0 = availbeWorker.npcData;
            giveOrderJob.extra1 = foodFarmJobArray;


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
                GrainField grainField;
                if ((grainField = colliders[i].gameObject.GetComponent<GrainField>()) != null)
                {
                    Job moveJob = new Job(Job.JobType.MOVE);
                    moveJob.position = grainField.transform.position;

                    Job examineJob = new Job(Job.JobType.ACTION);
                    Action actionAddTree = new Action(() => {
                        food.Add(grainField);
                    });
                    examineJob.extra0 = actionAddTree;

                    cheif.AddJob(moveJob);
                    cheif.AddJob(examineJob);
                }
            }
            Job actionJob = new Job(Job.JobType.ACTION);
            Action action = new Action(() =>
            {
                status = HarvestFarmWorkStatus.WORKING;
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
        return WorkType.FOOD_HARVEST_WORK;
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
