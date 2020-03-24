using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Bu class entitylere görev vermekle yükümlü
/// </summary>
public class Player : MonoBehaviour
{

    public static readonly float WoodCutRadious = 1f;
    public static readonly float StoneMineRadious = 1.5f;
    public static readonly float FoodFarmRadious = 1.5f;

    public Button toggleRunButt;
    public Button dropItemButt;
    public Button openInventoryButt;



    public GameObject waypointPrefab;
    public LineRenderer lineRenderer;

    private RTSSelector RTSSelector;

    private HarvestWoodWork woodWork;
    private HarvestStoneWork stoneWork;
    private HarvestFarmWork farmWork;

    List<List <NPCLogic>> group = new List<List <NPCLogic>>();//For Groupping function can be replaced later

    private void Start()
    {
        for (int i = 0; i < 9; i++)//For Groupping function can be replaced later
        {
            group.Add(new List<NPCLogic>());
        }
        
        RTSSelector = GetComponent<RTSSelector>();
        /*   toggleRunButt.onClick.AddListener(() =>
           {
               var selected = RTSSelector.GetSelectedAsNPCLogic();
               if (selected.Count >= 0)
                   foreach (var npcLogic in selected) ToggleRunCommand(npcLogic);
           });
           dropItemButt.onClick.AddListener(() =>
           {
               var selected = RTSSelector.GetSelectedAsNPCLogic();
               if (selected.Count >= 0)
                   foreach (var npcLogic in selected) npcLogic.AddPriorityJob(new Job(Job.JobType.ITEM_DROP));
           });
           openInventoryButt.onClick.AddListener(() =>
           {

           });*/

    }

    private void Update()
    {
        bool noUIcontrolsInUse = EventSystem.current.currentSelectedGameObject == null;
        if (!noUIcontrolsInUse) return;

        var selectedNPCLogic = RTSSelector.GetSelectedAsNPCLogic();
        if (selectedNPCLogic.Count == 0) return; // if 0 npc's chosen exit update method
        if (Input.GetKey(KeyCode.RightShift) && selectedNPCLogic.Count >= 0)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKey(KeyCode.RightShift) && Input.GetKey((KeyCode)(49 + i)) && selectedNPCLogic.Count >= 0)// Ctrl+Num grouping

                {
                    group[i].Clear();
                    foreach (var npcLogic in selectedNPCLogic)
                    {

                        if (npcLogic != null) group[i].Add(npcLogic);

                    }
                    break;
                }
            }
        }
        for (int i = 0; i < 9; i++)
        {
            if (!Input.anyKey)
            {
            
                if (Input.GetKeyUp((KeyCode)(49+i)))// Group choosing
                {

                    RTSSelector.ClearSelected();
                    foreach (var npc in group[i])
                    {
                        var npcselect = npc.GetComponent<Selectable>();
                        RTSSelector.UpdateSelection(npcselect, true);
                    }
                    break;
                }

                

            }
            
        }

        if (Input.GetKeyDown(KeyCode.H)) // if H key pressed
        {
            foreach (var npc in selectedNPCLogic) // For all Npc's
            {
                npc.SetJob(new Job(Job.JobType.HALT)); // Sets Halt Job
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && selectedNPCLogic.Count >= 0)
            foreach (var npc in selectedNPCLogic) ToggleRunCommand(npc);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (woodWork == null)
                woodWork = FindObjectOfType<HarvestWoodWork>();
            if (woodWork.cheif == null && selectedNPCLogic.Count == 1)
            {
                woodWork.SetCheif(selectedNPCLogic[0]);
            }
            else if (woodWork.cheif != null)
            {
                foreach (var npc in selectedNPCLogic)
                    woodWork.AddWorker(npc);
            }

        }
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (stoneWork == null)
                stoneWork = FindObjectOfType<HarvestStoneWork>();
            if (stoneWork.cheif == null && selectedNPCLogic.Count == 1)
            {
                stoneWork.SetCheif(selectedNPCLogic[0]);
            }
            else if (stoneWork.cheif != null)
            {
                foreach (var npc in selectedNPCLogic)
                    stoneWork.AddWorker(npc);
            }

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (farmWork == null)
                farmWork = FindObjectOfType<HarvestFarmWork>();
            if (farmWork.cheif == null && selectedNPCLogic.Count == 1)
            {
                farmWork.SetCheif(selectedNPCLogic[0]);
            }
            else if (farmWork.cheif != null)
            {
                foreach (var npc in selectedNPCLogic)
                    farmWork.AddWorker(npc);
            }

        }

        if (Input.GetMouseButtonDown(1) && RTSSelector.GetSelected().Count >= 0)
        {
            Ray mouseToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseToWorldRay, out hitInfo))
            {
                GameObject go = hitInfo.collider.gameObject;
                Vector3 point = hitInfo.point;
                Pickupable item;
                NPCLogic otherGuy;
                Building building;
                WoodTree tree;
                StoneMountain stone;
                GrainField food;
                if (go.tag.Equals("ground"))
                {
                    Vector3 x = new Vector3(0, 0, 0);
                    Vector3 z = new Vector3(0, 0, 0);
                    foreach (var npcLogic in selectedNPCLogic)
                    {

                        if (selectedNPCLogic.Count == 1)
                        {
                            MoveCommand(npcLogic, point);
                        }
                        else if (selectedNPCLogic.Count > 1)
                        {
                            if (x.x > Math.Sqrt(selectedNPCLogic.Count))
                            {
                                z.z++;
                                x.x = 0;
                            }
                            MoveCommand(npcLogic, point - new Vector3(x.x, 1, z.z));

                            x.x++;
                            //Debug.Log(x.x + " " + z.z + " " + Math.Sqrt(selectedNPCLogic.Count) + " " + npcLogic.name);

                        }
                    }
                }
                else if ((item = go.GetComponent<Pickupable>()) != null)
                    foreach (var npcLogic in selectedNPCLogic) PickUpCommand(npcLogic, item);
                else if ((tree = go.GetComponent<WoodTree>()) != null)
                {
                    if (selectedNPCLogic.Count == 1 && Input.GetKey(KeyCode.C))
                    {
                        TestGiveWoodCutOrderCommand(selectedNPCLogic[0], tree);
                    }
                    else
                    {
                        foreach (var npcLogic in selectedNPCLogic) WoodCuttingCommand(npcLogic, tree);
                    }
                }
                else if ((stone = go.GetComponent<StoneMountain>()) != null)
                {
                    if (selectedNPCLogic.Count == 1 && Input.GetKey(KeyCode.C))
                    {
                        TestGiveStoneCutOrderCommand(selectedNPCLogic[0], stone);
                    }
                    else
                    {
                        foreach (var npcLogic in selectedNPCLogic) StoneMiningCommand(npcLogic, stone);
                    }
                }
                else if((food = go.GetComponent<GrainField>()) != null)
                {
                    if (selectedNPCLogic.Count == 1 && Input.GetKey(KeyCode.C))
                    {
                        TestGiveFoodCutOrderCommand(selectedNPCLogic[0], food);
                    }
                    else
                    {
                        foreach (var npcLogic in selectedNPCLogic) FoodFarmingCommand(npcLogic, food);
                    }
                }

                else if ((otherGuy = go.GetComponent<NPCLogic>()) != null)
                    foreach (var npcLogic in selectedNPCLogic) ItemTransfer(npcLogic, otherGuy);
                else if ((building = go.GetComponent<Building>()) != null)
                    foreach (var npcLogic in selectedNPCLogic) BuildingActionCommand();
               
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && selectedNPCLogic.Count >= 0)
            foreach (var npcLogic in selectedNPCLogic) DropCommand(npcLogic);
    }

    private void MoveCommand(NPCLogic npcLogic, Vector3 destination)
    {
        if (Input.GetKey(KeyCode.LeftShift)) npcLogic.AddWaypoint(destination);
        else npcLogic.SetWaypoint(destination);
    }

    private void PickUpCommand(NPCLogic npcLogic, Pickupable item)
    {
        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = item.transform.position;

        Job pickupJob = new Job(Job.JobType.ITEM_PICKUP);
        pickupJob.extra0 = item;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            npcLogic.AddJob(moveJob);
            npcLogic.AddJob(pickupJob);
        }
        else
        {
            npcLogic.SetJob(moveJob);
            npcLogic.AddJob(pickupJob);
        }
    }

    private void BuildingActionCommand()
    {

    }

    private void ItemTransfer(NPCLogic npcLogic, NPCLogic otherGuy)
    {
        Job gotoJob = new Job(Job.JobType.GOTO);
        gotoJob.extra0 = otherGuy;

        Job giveOrder = new Job(Job.JobType.GIVEORDER);
        giveOrder.extra0 = otherGuy.npcData;

        Job receiveJob = new Job(Job.JobType.ITEM_RECEIVE);
        receiveJob.extra0 = npcLogic;
        giveOrder.extra1 = receiveJob;

        Job itemTransfer = new Job(Job.JobType.ITEM_TRANSFER);
        itemTransfer.extra0 = otherGuy;

        if (Input.GetKey(KeyCode.LeftShift) && (npcLogic.npcData.jobQueue.HasPickUpJob() || npcLogic.npcData.carryingItem != null))
        {
            npcLogic.AddJob(gotoJob);
            npcLogic.AddJob(giveOrder);
            npcLogic.AddJob(itemTransfer);
        }
        else if (npcLogic.npcData.carryingItem != null)
        {
            npcLogic.SetJob(gotoJob);
            npcLogic.AddJob(giveOrder);
            npcLogic.AddJob(itemTransfer);
        }
    }

    private void DropCommand(NPCLogic npcLogic)
    {
        if ((npcLogic.npcData.carryingItem != null || npcLogic.npcData.jobQueue.HasPickUpJob()))
        {
            Job drobJob = new Job(Job.JobType.ITEM_DROP);
            if (Input.GetKey(KeyCode.LeftShift)) npcLogic.AddJob(drobJob);
            else npcLogic.SetJob(drobJob);
        }
    }

    private void ToggleRunCommand(NPCLogic npcLogic)
    {
        npcLogic.npcData.npcController.ToggleRun();
    }

    private void WoodCuttingCommand(NPCLogic npcLogic, WoodTree woodTree)
    {

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = woodTree.transform.position;

        Job WoodCutting = new Job(Job.JobType.WOOD_CUTTING);
        WoodCutting.extra0 = woodTree;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            npcLogic.AddJob(moveJob);
            npcLogic.AddJob(WoodCutting);
        }
        else
        {
            npcLogic.SetJob(moveJob);
            npcLogic.AddJob(WoodCutting);
        }
    }

    private void TestGiveStoneCutOrderCommand(NPCLogic npc, StoneMountain stoneMountain)
    {
        var others = FindObjectsOfType<NPCLogic>();
        List<Job> jobs = new List<Job>();
        double CurrentCurve = 0;
        double Curve = (double)(360) / (double)(others.Length);
        Vector3 x = new Vector3(0, 0, 0);
        Vector3 z = new Vector3(0, 0, 0);
        foreach (var other in others)
        {
            if (other.Equals(npc)) continue;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = other;

            Job otherMoveCmd = new Job(Job.JobType.MOVE);

            var CircleVector = Quaternion.AngleAxis((float)(CurrentCurve), Vector3.up) * Vector3.forward * StoneMineRadious;
            CurrentCurve += Curve;
            otherMoveCmd.position = stoneMountain.transform.position + CircleVector;
            Debug.Log(CircleVector + " " + Curve);
            Job otherWoodCutOrder = new Job(Job.JobType.STONE_MINING);
            otherWoodCutOrder.extra0 = stoneMountain;
            Job otherMove2Cmd = new Job(Job.JobType.MOVE);


            if (others.Length == 1)
            {
                otherMove2Cmd.position = npc.transform.position;
            }
            else if (others.Length > 1)
            {
                if (x.x > Math.Sqrt(others.Length))
                {
                    z.z++;
                    x.x = 0;
                }
                otherMove2Cmd.position = npc.transform.position - new Vector3(x.x, 1, z.z);

                x.x++;
                //Debug.Log(x.x + " " + z.z + " " + Math.Sqrt(selectedNPCLogic.Count) + " " + npcLogic.name);

            }



            Job otherDropCmd = new Job(Job.JobType.ITEM_DROP);

            Job goto2Job = new Job(Job.JobType.GOTO);
            goto2Job.extra0 = npc;

            Job otherReportback = new Job(Job.JobType.REPORT_BACK);
            otherReportback.extra0 = npc;

            Job[] otherJobs = new Job[] { otherMoveCmd, otherWoodCutOrder, otherMove2Cmd, otherDropCmd, goto2Job, otherReportback };

            Job giveOrder = new Job(Job.JobType.GIVEORDER);
            giveOrder.extra0 = other.npcData;
            giveOrder.extra1 = otherJobs;

            jobs.Add(gotoJob);
            jobs.Add(giveOrder);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            foreach (var job in jobs)
            {
                npc.AddJob(job);
            }
        }
        else
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                if (i == 0)
                    npc.SetJob(jobs[i]);
                else
                    npc.AddJob(jobs[i]);
            }
        }
    }

    private void TestGiveWoodCutOrderCommand(NPCLogic npc, WoodTree woodTree)
    {
        var others = FindObjectsOfType<NPCLogic>();
        List<Job> jobs = new List<Job>();
        double CurrentCurve = 0;
        double Curve = (double)(360) / (double)(others.Length);
        Vector3 x = new Vector3(0, 0, 0);
        Vector3 z = new Vector3(0, 0, 0);
        foreach (var other in others)
        {
            if (other.Equals(npc)) continue;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = other;

            Job otherMoveCmd = new Job(Job.JobType.MOVE);

            var CircleVector = Quaternion.AngleAxis((float)(CurrentCurve), Vector3.up) * Vector3.forward * WoodCutRadious;
            CurrentCurve += Curve;

            otherMoveCmd.position = woodTree.transform.position + CircleVector;

            Job otherWoodCutOrder = new Job(Job.JobType.WOOD_CUTTING);
            otherWoodCutOrder.extra0 = woodTree;
            Job otherMove2Cmd = new Job(Job.JobType.MOVE);
            if (others.Length == 1)
            {
                otherMove2Cmd.position = npc.transform.position;
            }
            else if (others.Length > 1)
            {
                if (x.x > Math.Sqrt(others.Length))
                {
                    z.z++;
                    x.x = 0;
                }
                otherMove2Cmd.position = npc.transform.position - new Vector3(x.x, 1, z.z);

                x.x++;
                //Debug.Log(x.x + " " + z.z + " " + Math.Sqrt(selectedNPCLogic.Count) + " " + npcLogic.name);

            }


            Job otherDropCmd = new Job(Job.JobType.ITEM_DROP);

            Job[] otherJobs = new Job[] { otherMoveCmd, otherWoodCutOrder, otherMove2Cmd, otherDropCmd };

            Job giveOrder = new Job(Job.JobType.GIVEORDER);
            giveOrder.extra0 = other.npcData;
            giveOrder.extra1 = otherJobs;

            jobs.Add(gotoJob);
            jobs.Add(giveOrder);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            foreach (var job in jobs)
            {
                npc.AddJob(job);
            }
        }
        else
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                if (i == 0)
                    npc.SetJob(jobs[i]);
                else
                    npc.AddJob(jobs[i]);
            }
        }
    }

    private void StoneMiningCommand(NPCLogic npcLogic, StoneMountain stoneMountain)
    {

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = stoneMountain.transform.position;

        Job StoneMountain = new Job(Job.JobType.STONE_MINING);
        StoneMountain.extra0 = stoneMountain;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            npcLogic.AddJob(moveJob);
            npcLogic.AddJob(StoneMountain);
        }
        else
        {
            npcLogic.SetJob(moveJob);
            npcLogic.AddJob(StoneMountain);
        }


        
    }
    private void FoodFarmingCommand(NPCLogic npcLogic, GrainField grainField)
    {

        Job moveJob = new Job(Job.JobType.MOVE);
        moveJob.position = grainField.transform.position;

        Job GrainField = new Job(Job.JobType.FARMING);
        GrainField.extra0 = grainField;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            npcLogic.AddJob(moveJob);
            npcLogic.AddJob(GrainField);
        }
        else
        {
            npcLogic.SetJob(moveJob);
            npcLogic.AddJob(GrainField);
        }



    }

    private void TestGiveFoodCutOrderCommand(NPCLogic npc, GrainField grainField)
    {
        var others = FindObjectsOfType<NPCLogic>();
        List<Job> jobs = new List<Job>();
        double CurrentCurve = 0;
        double Curve = (double)(360) / (double)(others.Length);
        Vector3 x = new Vector3(0, 0, 0);
        Vector3 z = new Vector3(0, 0, 0);
        foreach (var other in others)
        {
            if (other.Equals(npc)) continue;

            Job gotoJob = new Job(Job.JobType.GOTO);
            gotoJob.extra0 = other;

            Job otherMoveCmd = new Job(Job.JobType.MOVE);

            var CircleVector = Quaternion.AngleAxis((float)(CurrentCurve), Vector3.up) * Vector3.forward * FoodFarmRadious;
            CurrentCurve += Curve;
            otherMoveCmd.position = grainField.transform.position + CircleVector;
            Debug.Log(CircleVector + " " + Curve);
            Job otherWoodCutOrder = new Job(Job.JobType.FARMING);
            otherWoodCutOrder.extra0 = grainField;
            Job otherMove2Cmd = new Job(Job.JobType.MOVE);


            if (others.Length == 1)
            {
                otherMove2Cmd.position = npc.transform.position;
            }
            else if (others.Length > 1)
            {
                if (x.x > Math.Sqrt(others.Length))
                {
                    z.z++;
                    x.x = 0;
                }
                otherMove2Cmd.position = npc.transform.position - new Vector3(x.x, 1, z.z);

                x.x++;
                //Debug.Log(x.x + " " + z.z + " " + Math.Sqrt(selectedNPCLogic.Count) + " " + npcLogic.name);

            }



            Job otherDropCmd = new Job(Job.JobType.ITEM_DROP);

            Job goto2Job = new Job(Job.JobType.GOTO);
            goto2Job.extra0 = npc;

            Job otherReportback = new Job(Job.JobType.REPORT_BACK);
            otherReportback.extra0 = npc;

            Job[] otherJobs = new Job[] { otherMoveCmd, otherWoodCutOrder, otherMove2Cmd, otherDropCmd, goto2Job, otherReportback };

            Job giveOrder = new Job(Job.JobType.GIVEORDER);
            giveOrder.extra0 = other.npcData;
            giveOrder.extra1 = otherJobs;

            jobs.Add(gotoJob);
            jobs.Add(giveOrder);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            foreach (var job in jobs)
            {
                npc.AddJob(job);
            }
        }
        else
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                if (i == 0)
                    npc.SetJob(jobs[i]);
                else
                    npc.AddJob(jobs[i]);
            }
        }
    }

    private void LateUpdate()
    {

    }
}
