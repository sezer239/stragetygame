using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static NPCData;

/// <summary>
/// Bir görevin bitip bitmediği lociği buraya yazılıyor
/// </summary>
[RequireComponent(typeof(NPCData))]
[RequireComponent(typeof(NPCController))]
public class NPCLogic : MonoBehaviour
{
    public static readonly double MOVE_DONE_THRESHOLD = 1.5;
    public static readonly double ITEM_PICKUP_THRESHOLD = 2;
    public static readonly double WOOD_CUTTING_DISTANCE = 1.5;
    public static readonly double STONE_MINING_DISTANCE = 2.5;
    public static readonly double GIVE_ORDER_DISTANCE = 2;


    public delegate void OnNewJobReceived(Job receviedJob, int jobIndex);
    public delegate void OnJobCompleted(Job job);
    public delegate void OnJobCancelled(Job job);
    public delegate void OnJobFailed(Job job);
    public delegate void OnJobStarted(Job job);
    public delegate void OnJobEnd(Job job);

    public event OnNewJobReceived onNewJobReceived;
    public event OnJobCompleted onJobCompleted;
    public event OnJobCancelled onJobCancelled;
    public event OnJobFailed onJobFailed;
    public event OnJobStarted onJobStarted;
    public event OnJobEnd onJobEnd;

    public NPCData npcData;
    private NPCController npcController;

    private Job currentJob;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void OnEnable()
    {
        npcData = GetComponent<NPCData>();
    }

    private void Start()
    {
        npcData.jobQueue.onJobAdded += JobQueue_onJobAdded;
        onJobStarted += NPCLogic_onJobStarted;
    }

    private void NPCLogic_onJobStarted(Job job)
    {
        switch (currentJob.jobType)
        {
            case Job.JobType.GIVEORDER:
            GiveOrderLogicJob(); break;
            case Job.JobType.ACTION:
            {
                ((Action)currentJob.extra0).Invoke();
                CompleteCurrentLogicJob();
            }
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (npcData.jobQueue.jobs.Count == 0) return;

        switch (currentJob.jobType)
        {
            case Job.JobType.WOOD_CUTTING:
            WoodCuttingLogicJob(); break;
            case Job.JobType.STONE_MINING:
            StoneMiningLogicJob(); break;
            case Job.JobType.HALT:
            HaltLogicJob(); break;
            case Job.JobType.MOVE:
            MoveLogicJob(); break;
            case Job.JobType.GOTO:
            GotoLogicJob(); break;
            case Job.JobType.ITEM_PICKUP:
            ItemPickUpLogicJob(); break;
            case Job.JobType.ITEM_DROP:
            ItemDropLogicJob(); break;
            case Job.JobType.ITEM_TRANSFER:
            ItemTransferLogicJob(); break;
            case Job.JobType.ITEM_RECEIVE:
            ItemReceiveLogicJob(); break;
        }
    }

    private void JobQueue_onJobAdded(Job job, int index)
    {
        if (index == 0)
        {
            JobStart();
        }
        npcData.npcStatus = NPCStatus.BUSY; /*STATUSU TRIGIRLIYOR BU SAYEDE UI GUNCELLENIYOR TODO: BU GUZEL DEGIL SIL BI ARA*/
    }

    public void AddPriorityJobs(Job[] jobs)
    {
        var jobsCopy = npcData.jobQueue.jobs.ToArray();
        for (int i = 0; i < jobs.Length; i++)
        {
            if (i == 0)
                SetJob(jobs[i]);
            else
                AddJob(jobs[i]);
        }

        foreach (var job in jobsCopy)
        {
            AddJob(job);
        }
    }

    public void AddPriorityJob(Job job)
    {
        npcData.jobQueue.AddPriorityJob(job);
    }

    public void SetJob(Job job)
    {
        npcData.jobQueue.jobs.Clear();
        CancelCurrentLogicJob();
        npcData.jobQueue.SetSingleJob(job);
    }

    public void AddJob(Job job)
    {
        npcData.jobQueue.AddJob(job);
    }

    public void SetWaypoint(Vector3 vector3)
    {
        npcData.jobQueue.jobs.Clear();
        CancelCurrentLogicJob();
        npcData.jobQueue.SetSingleMoveJob(vector3);
    }

    public void AddWaypoint(Vector3 vector3)
    {
        npcData.jobQueue.AddMoveJob(vector3);
    }

    /// <summary>
    /// Bu diğer npcler tarafından çağrılan fonksiyon bu sayede 2 npc arasında tuhaf callbacklere gerek kalmıyor
    /// </summary>
    /// <param name="who"></param>
    /// <param name="data"></param>
    public void InteractionReceive(NPCLogic who, string data)
    {
        switch (currentJob.jobType)
        {
            case Job.JobType.GIVEORDER:
            {
                var toWho = currentJob.extra0 as NPCData;

                if (toWho.Equals(who.npcData))
                {
                    if (data.Equals("order_received"))
                    {
                        CompleteCurrentLogicJob();
                    }
                    else
                    {
                        FailCurrentLogicJob();
                    }
                }

            }
            break;
            case Job.JobType.REPORT_BACK:
            {
                var toWho = currentJob.extra0 as NPCLogic;
                var dataStr = currentJob.extra1 as string;

                if (toWho.Equals(who) && data.Equals(dataStr))
                {
                    CompleteCurrentLogicJob();
                }
                else
                {
                    FailCurrentLogicJob();
                }
            }
            break;
        }
    }

    /// <summary>
    /// Karakterler arası görev dağılımı priority olarak ekler öncelikli görev olarak ekler
    /// </summary>
    /// <param name="byWho"></param>
    /// <param name="order"></param>
    /// <param name="receivedCallback"></param>
    public void GivePriorityOrder(NPCLogic byWho, Job order)
    {
        if (byWho != null /*Eğer karakteri sevmiyorsa görevi almasın falan filan logic buraya*/)
        {
            AddPriorityJob(order);
            byWho.InteractionReceive(this, "order_received");
        }
        else
        {
            byWho.InteractionReceive(this, "order_declined");
        }
    }

    /// <summary>
    /// Karakterler arası görev dağılımı bütün görevleri siler bunu ekler
    /// </summary>
    /// <param name="byWho"></param>
    /// <param name="order"></param>
    /// <param name="receivedCallback"></param>
    public void GiveSetOrder(NPCLogic byWho, Job order)
    {
        if (byWho != null /*Eğer karakteri sevmiyorsa görevi almasın falan filan logic buraya*/)
        {
            SetJob(order);
            byWho.InteractionReceive(this, "order_received");
        }
        else
        {
            byWho.InteractionReceive(this, "order_declined");
        }
    }

    /// <summary>
    /// Karakterler arası görev dağılımı priority olarak ekler öncelikli görev olarak ekler
    /// </summary>
    /// <param name="byWho"></param>
    /// <param name="orders"></param>
    /// <param name="receivedCallback"></param>
    public void GivePriorityOrders(NPCLogic byWho, Job[] orders)
    {
        if (byWho != null /*Eğer karakteri sevmiyorsa görevi almasın falan filan logic buraya*/)
        {
            AddPriorityJobs(orders);

            byWho.InteractionReceive(this, "order_received");
        }
        else
        {
            byWho.InteractionReceive(this, "order_declined");
        }
    }

    /// <summary>
    /// Karakterler arası görev dağılımı bütün görevleri siler bunu ekler
    /// </summary>
    /// <param name="byWho"></param>
    /// <param name="orders"></param>
    /// <param name="receivedCallback"></param>
    public void GiveSetOrders(NPCLogic byWho, Job[] orders)
    {
        if (byWho != null /*Eğer karakteri sevmiyorsa görevi almasın falan filan logic buraya*/)
        {
            SetJob(orders[0]);
            for (int i = 1; i < orders.Length; i++)
            {
                AddJob(orders[i]);
            }
            byWho.InteractionReceive(this, "order_received");
        }
        else
        {
            byWho.InteractionReceive(this, "order_declined");
        }
    }

    private void JobEnd(Job job)
    {
        if (onJobEnd != null)
        {
            npcData.npcStatus = NPCStatus.WAITING_FOR_ORDERS;
            onJobEnd.Invoke(job);
        }
        if (npcData.jobQueue.jobs.Count > 0)
        {
            npcData.jobQueue.jobs.RemoveAt(0);
            JobStart();
        }
        npcData.npcStatus = NPCStatus.WAITING_FOR_ORDERS;
    }

    private void JobStart()
    {
        if (npcData.jobQueue.jobs.Count > 0)
        {
            if (onJobEnd != null)
            {
                if (navMeshAgent == null)
                {
                    npcController = GetComponent<NPCController>();
                    navMeshAgent = npcController.GetNavMeshAgent();
                }
                currentJob = npcData.jobQueue.jobs[0];
                npcData.npcStatus = NPCStatus.BUSY;
                currentJob.jobStatus = Job.JobStatus.WORKING;
                onJobStarted.Invoke(currentJob);
            }
        }
    }


    /// <summary>
    /// This function is called by other npc's in the game
    /// </summary>
    /// <param name="whoIsReporting"> Who is doing to reportig </param>
    /// <param name="data"> Data that is being sent </param>
    public void Report(NPCLogic whoIsReporting, string data)
    {
        if (npcData.isCheif)
        {
            Debug.Log(name + " " + npcData.isCheif + " cheif and " + whoIsReporting.name + " is reporting");
            var workType = npcData.workingOn.GetWorkType();
            switch (workType)
            {
                case Work.WorkType.WOOD_HARVEST_WORK:
                {
                    Debug.Log("HERE1");
                    if (data.Equals("Completed"))
                    {
                        Debug.Log("HERE2");
                        var woodHarvestWork = npcData.workingOn as HarvestWoodWork;
                        woodHarvestWork.GiveWoodCutJobToWorker(whoIsReporting);
                        whoIsReporting.InteractionReceive(this, data);
                    }
                }

                    break;
                case Work.WorkType.STONE_HARVEST_WORK:
                    {
                        Debug.Log("HERE1");
                        if (data.Equals("Completed"))
                        {
                            Debug.Log("HERE2");
                            var stoneHarvestWork = npcData.workingOn as HarvestStoneWork;
                            stoneHarvestWork.GiveStoneMineJobToWorker(whoIsReporting);
                            whoIsReporting.InteractionReceive(this, data);
                        }
                    }
                    break;
            }
        }
        else
        {
        }
    }

    private void CompleteCurrentLogicJob()
    {
        if (npcData.jobQueue.jobs.Count > 0 && currentJob != null)
        {
            Job current = currentJob;
            current.jobStatus = Job.JobStatus.COMPLETED;

            if (onJobCompleted != null)
                onJobCompleted.Invoke(current);

            JobEnd(current);
        }
    }

    private void CancelCurrentLogicJob()
    {
        if (currentJob != null)
        {
            Job current = currentJob;
            current.jobStatus = Job.JobStatus.CANCELLED;

            if (onJobCancelled != null)
                onJobCancelled.Invoke(current);

            JobEnd(current);
        }
    }

    private void FailCurrentLogicJob()
    {
        if (npcData.jobQueue.jobs.Count > 0 && currentJob != null)
        {
            Job current = currentJob;
            current.jobStatus = Job.JobStatus.FAILED;

            if (onJobFailed != null)
                onJobFailed.Invoke(current);

            JobEnd(current);
        }
    }

    private void MoveLogicJob()
    {
        if (npcController == null && navMeshAgent.pathPending) return;
        var agent = navMeshAgent;
        var dis = Vector3.Distance(currentJob.position, transform.position);
        if (dis < MOVE_DONE_THRESHOLD)
        {
            CompleteCurrentLogicJob();
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            FailCurrentLogicJob();
        }
    }

    private void GotoLogicJob()
    {
        if (npcController == null && navMeshAgent.pathPending) return;
        var agent = navMeshAgent;
        var dis = Vector3.Distance(((NPCLogic)currentJob.extra0).transform.position, transform.position);
        if (dis < MOVE_DONE_THRESHOLD)
        {
            CompleteCurrentLogicJob();
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            FailCurrentLogicJob();
        }
    }

    private void ItemReceiveLogicJob()
    {
        var otherGuy = (NPCLogic)currentJob.extra0;
        if (npcData.carryingItem != null)
        {
            CompleteCurrentLogicJob();
        }
        else if (otherGuy.npcData.carryingItem == null)
        {
            FailCurrentLogicJob();
        }
    }

    private void ItemTransferLogicJob()
    {
        var otherGuy = (NPCLogic)currentJob.extra0;
        var dis = Vector3.Distance(otherGuy.transform.position, transform.position);

        if (dis < ITEM_PICKUP_THRESHOLD && otherGuy.npcData.carryingItem != null && npcData.carryingItem == null)
        {
            CompleteCurrentLogicJob();
        }
        else if (dis > ITEM_PICKUP_THRESHOLD || otherGuy.npcData.carryingItem != null && npcData.carryingItem != null)
        {
            FailCurrentLogicJob();
        }
    }

    private void ItemPickUpLogicJob()
    {
        var item = (Pickupable)currentJob.extra0;

        if (npcData.carryingItem != null && npcData.carryingItem.Equals(item))
        {
            CompleteCurrentLogicJob();
        }
        else if (item == null || item.carriedBy != null && !item.carriedBy.Equals(this))
        {
            FailCurrentLogicJob();
        }
    }

    private void ItemDropLogicJob()
    {
        if (npcData.carryingItem == null)
        {
            CompleteCurrentLogicJob();
        }
    }

    private void HaltLogicJob()
    {
        if (npcData.npcController.GetNavMeshAgent().velocity.magnitude < 0.1)
        {
            CompleteCurrentLogicJob();
        }
    }

    private void WoodCuttingLogicJob()
    {
        var woodTree = currentJob.extra0 as WoodTree;

        if (woodTree == null)
        {
            CompleteCurrentLogicJob();
        }
        else if (npcData.carryingItem != null && (npcData.carryingItem as Chest).isfull())
        {
            CancelCurrentLogicJob();
        }
        else if (Vector3.Distance(woodTree.transform.position, transform.position) > WOOD_CUTTING_DISTANCE)
        {
            //FailCurrentLogicJob();
            Job job = new Job(Job.JobType.MOVE);
            job.position = woodTree.transform.position;
            AddPriorityJob(job);
        }
    }


    private void GiveOrderLogicJob()
    {
        var toWho = currentJob.extra0 as NPCData;
        var dis = Vector3.Distance(toWho.transform.position, transform.position);
        if (dis > GIVE_ORDER_DISTANCE)
        {
            FailCurrentLogicJob();
        }
    }

    private void StoneMiningLogicJob()
    {
        var stone = currentJob.extra0 as StoneMountain;
        if (stone == null)
        {
            CompleteCurrentLogicJob();
        }
        else if (npcData.carryingItem != null && (npcData.carryingItem as Chest).isfull())
        {
            CancelCurrentLogicJob();
        }
        else if (Vector3.Distance(stone.transform.position, transform.position) > STONE_MINING_DISTANCE)
        {
            //FailCurrentLogicJob();
            Job job = new Job(Job.JobType.MOVE);
            job.position = stone.transform.position;
            AddPriorityJob(job);
        }
    }
}
