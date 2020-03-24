using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Bir görevin nasıl yapılacağı buraya
/// </summary>
[RequireComponent(typeof(NPCData))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCController : MonoBehaviour
{
    private Animator animator;
    private NPCData npcData;
    private NavMeshAgent agent;
    private Job currentJob;
    public Chest chest;

    private bool run = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcData = GetComponent<NPCData>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        npcData.npcLogic.npcData.selectedChanged += SelectedChanged;
        npcData.npcLogic.onJobStarted += OnJobStarted;
        npcData.npcLogic.onJobEnd += OnJobEnded;
    }

    private void OnJobEnded(Job job)
    {
        UnityEngine.Debug.Log("Job " + job.jobType + " Ended with " + job.jobStatus);

        switch (currentJob.jobType)
        {
            case Job.JobType.MOVE:
            {
                agent.isStopped = true;
            }
            break;
            case Job.JobType.HALT:
            {
                agent.isStopped = true;
            }
            break;
            case Job.JobType.ITEM_RECEIVE:
            {
                agent.isStopped = true;

            }
            break;
            case Job.JobType.ITEM_TRANSFER:
            {
                agent.isStopped = true;
            }
            break;
            case Job.JobType.ITEM_DROP:
            {
                agent.isStopped = true;
            }
            break;
            case Job.JobType.ITEM_PICKUP:
            {
                agent.isStopped = true;
            }
            break;
            case Job.JobType.WOOD_CUTTING:
            {
                animator.SetBool("WoodCutting", false);
            }
            break;
            case Job.JobType.STONE_MINING:
            {
                animator.SetBool("StoneCutting", false);
            }
            break;
        }
    }

    private void OnJobStarted(Job job)
    {
        if (job == null) UnityEngine.Debug.Log("Job is null");
        UnityEngine.Debug.Log("Job " + job.jobType + " Started");
        currentJob = job;
        switch (currentJob.jobType)
        {
            case Job.JobType.MOVE:
            MoveControlJob(currentJob);
            break;
            case Job.JobType.HALT:
            HaltControlJob(currentJob);
            break;
            case Job.JobType.ITEM_RECEIVE:
            {
                ItemReceiveControlJob(currentJob);
            }
            break;
            case Job.JobType.ITEM_TRANSFER:
            {
                animator.SetTrigger("Receive");
            }
            break;
            case Job.JobType.ITEM_DROP:
            {
                if (npcData.carryingItem != null)
                    animator.SetTrigger("Drop");
            }
            break;
            case Job.JobType.ITEM_PICKUP:
            {
                var pickable = currentJob.extra0 as Pickupable;
                if (pickable.carriedBy == null)
                    animator.SetTrigger("Pickup");
            }
            break;
            case Job.JobType.STONE_MINING:
            {
                if (currentJob.extra0 as StoneMountain != null)
                {
                    animator.SetBool("StoneCutting", true);
                    var look = transform.position - ((StoneMountain)currentJob.extra0).transform.position;
                    look.y = transform.position.y;
                    agent.transform.rotation = Quaternion.LookRotation(-look, Vector3.up);
                }
            }
            break;
            case Job.JobType.GIVEORDER:
            {
                GiveOrderControlJob(currentJob);
            }
            break;
            case Job.JobType.WOOD_CUTTING:
            {
                if (currentJob.extra0 as WoodTree != null)
                {
                    animator.SetBool("WoodCutting", true);
                    var look = transform.position - ((WoodTree)currentJob.extra0).transform.position;
                    look.y = transform.position.y;
                    agent.transform.rotation = Quaternion.LookRotation(-look, Vector3.up);

                }
            }
            break;
        }
    }

    private void SelectedChanged(Selectable self, bool isSelected)
    {

    }

    public bool ToggleRun()
    {
        run = !run;
        return run;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return agent;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentJob == null) return;

        if (currentJob.jobType == Job.JobType.GOTO)
        {
            agent.isStopped = false;
            NPCLogic guy = (NPCLogic)currentJob.extra0;

            agent.SetDestination(guy.transform.position);
        }
        else if (currentJob.jobType ==  Job.JobType.REPORT_BACK)
        {
            var cheif = currentJob.extra0 as NPCLogic;
            var data = currentJob.extra1 as string;

            if (Vector3.Distance(cheif.transform.position, transform.position) <  NPCLogic.GIVE_ORDER_DISTANCE)
            {
                cheif.Report(npcData.npcLogic, data);
            }
        }
    }

    private void LateUpdate()
    {
        if (agent.velocity.magnitude > 0.1)
        {

            animator.SetBool("Walk", true);
            if (run)
            {
                animator.SetBool("Run", true);
                agent.speed = npcData.runSpeed;
            }
            else
            {
                animator.SetBool("Run", false);
                agent.speed = npcData.walkSpeed;
            }
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }


    private void MoveControlJob(Job job)
    {
        agent.isStopped = false;
        agent.SetDestination(job.position);
    }

    private void ItemReceiveControlJob(Job job)
    {
        agent.isStopped = true;
    }

    public void TransferAnimEvent()
    {

        if (currentJob.jobType == Job.JobType.ITEM_TRANSFER)
        {
            UnityEngine.Debug.Log("Here");
            var toWho = currentJob.extra0 as NPCLogic;
            if (toWho.npcData.carryingItem == null)
            {
                var item = npcData.carryingItem;
                npcData.npcController.DropItem();
                toWho.npcData.npcController.PickUpItem(item);
            }
        }
    }

    public void PickUpItem(Pickupable item)
    {
        item.PickUp(npcData);
    }

    public void DropItem()
    {
        npcData.carryingItem.Drop();
    }

    public void PutDownAnimEvent()
    {
        DropItem();
    }

    public void PickupAnimEvent()
    {
        var chest = currentJob.extra0 as Chest;
        if (chest != null && chest.carriedBy == null)
            PickUpItem(chest);
    }

    public void StoneCuttingHitAnimEvent()
    {
        if (npcData.carryingItem == null)
        {
            Chest newChest = Instantiate(chest, transform.position, Quaternion.identity);
            npcData.npcController.PickUpItem(newChest);
        }

        var minedStone = currentJob.extra0 as StoneMountain;
        if (minedStone != null)
        {
            if (minedStone.stoneAmount >= 5)
            {
                minedStone.stoneAmount -= 5;
                var c = npcData.carryingItem as Chest;
                c.ItemAdd("Stone", 5);
            }
            else
            {
                var c = npcData.carryingItem as Chest;
                c.ItemAdd("Stone", minedStone.stoneAmount);
                minedStone.stoneAmount = 0;
            }
        }
    }

    public void WoodCuttingHitAnimEvent()
    {
        if (npcData.carryingItem == null)
        {
            Chest newChest = Instantiate(chest, transform.position, Quaternion.identity);
            PickUpItem(newChest);
        }

        var cuttedTree = currentJob.extra0 as WoodTree;

        if (cuttedTree != null)
        {
            if (cuttedTree.woodAmount >= 5)
            {
                cuttedTree.woodAmount -= 5;
                var c = npcData.carryingItem as Chest;
                c.ItemAdd("Wood", 5);
            }
            else
            {
                var c = npcData.carryingItem as Chest;
                c.ItemAdd("Wood", cuttedTree.woodAmount);
                cuttedTree.woodAmount = 0;
            }
        }
    }

    private void GiveOrderControlJob(Job job)
    {
        var toWho = job.extra0 as NPCData;
        var jobsToGive = job.extra1 as Job[];
        Job jobToGive;
        var dis = Vector3.Distance(toWho.transform.position, transform.position);
        if (dis < NPCLogic.GIVE_ORDER_DISTANCE)
        {
            if (jobsToGive != null)
            {
                toWho.npcLogic.GivePriorityOrders(npcData.npcLogic, jobsToGive);
            }
            else if ((jobToGive = currentJob.extra1 as Job) != null)
            {
                toWho.npcLogic.GivePriorityOrder(npcData.npcLogic, jobToGive);
            }
        }
    }

    private void HaltControlJob(Job job)
    {
        agent.isStopped = true;
    }
}
