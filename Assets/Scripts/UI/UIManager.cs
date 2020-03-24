using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {
        get { return _instance; }
    }
    private static UIManager _instance;

    private RTSSelector RTSSelector;

    public Button GotoJobUIButton;
    public Button HaltJobUIButton;
    public Button ItemPickupJobUIButton;
    public Button ItemDropJobUIButton;
    public Button ItemReceiveJobUIButton;
    public Button ItemTransferJobUIButton;
    public Button MoveJobUIButton;
    public Button WoodCutUIButton;
    public Button DefaultJobUIButton;

    public Button StoneCutUIButton;
    public Button OrderJobtUIButton;
    public Button ReportBackJobtUIButton;
    public Button ExamineJobtUIButton;
    public Button ActionJobtUIButton;

    public RectTransform UIJobQueueSpawn;

    public GameWindow focusedGameWindow;

    public List<GameWindow> gameWindows;

    public NPCInfoUI npcInfoUI;


    public GameObject worksWindow;
    public Button worksButton;

    public GameObject peopleWindow;
    public Button peoplesButton;

    public WoodHarvestEditWindow woodHarvestEditWindow;

   /* public GameWindow RequestWindow(GameWindow window, object context)
    {

    }*/

    void Start()
    {
        gameWindows = new List<GameWindow>();
        _instance = this;
        RTSSelector = GetComponent<RTSSelector>();
        var allNPCs = FindObjectsOfType<NPCLogic>();
        foreach (var npc in allNPCs)
        {
            npc.npcData.npcStatusChanged += NpcData_npcStatusChanged;
            npc.npcData.selectedChanged += NpcData_selectedChanged;
        }

        worksButton.onClick.AddListener(() =>
        {
            peopleWindow.SetActive(false);
            worksWindow.SetActive(!worksWindow.activeSelf);
        });

        peoplesButton.onClick.AddListener(() =>
        {
            worksWindow.SetActive(false);
            peopleWindow.SetActive(!peopleWindow.activeSelf);
        });
    }

    private void NpcData_selectedChanged(Selectable self, bool isSelected)
    {
        var selectedNPC = RTSSelector.GetSelectedAsNPCLogic();
        NPCData npcData;
        if (selectedNPC.Count == 1 && isSelected && (npcData = self.GetComponent<NPCData>()) != null)
        {
            DeleteJobQueueButtons();
            UpdateJobQueueButtons(npcData.npcLogic);
            UpdateSelectedNPCUI(npcData.npcLogic);
        }
    }

    private void NpcData_npcStatusChanged(NPCData npcdata, NPCData.NPCStatus status)
    {
        var selectedNPC = RTSSelector.GetSelectedAsNPCLogic();
        if (selectedNPC.Count == 1 && npcdata.isSelected)
        {
            DeleteJobQueueButtons();
            UpdateJobQueueButtons(npcdata.npcLogic);
            UpdateSelectedNPCUI(npcdata.npcLogic);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var selectedNPC = RTSSelector.GetSelectedAsNPCLogic();
        if (selectedNPC.Count != 1)
        {
            DeleteJobQueueButtons();
            ClearSelectedNPCUI();            
        }

    }

    private void ClearSelectedNPCUI()
    {
        npcInfoUI.Hide();
    }

    private void UpdateSelectedNPCUI(NPCLogic npcLogic)
    {
        npcInfoUI.Show(npcLogic);
    }



    private void LateUpdate()
    {

    }

    private void DeleteJobQueueButtons()
    {
        var childs = UIJobQueueSpawn.GetComponentsInChildren<Button>();
        foreach (var child in childs)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateJobQueueButtons(NPCLogic npc)
    {
        var jobs = npc.npcData.jobQueue.jobs;
        int xindex = 0;
        int yindex = 0;
        int maxIndexOnXAxis = 5;
        foreach (var job in jobs)
        {
            var butt = Instantiate(GetCorrectButtonRef(job.jobType));
            butt.onClick.AddListener(() =>
            {
                npc.npcData.jobQueue.jobs.Remove(job); Debug.Log("here"); npc.npcData.npcStatus = NPCData.NPCStatus.BUSY;
                if (npc.npcData.jobQueue.jobs.Count == 0)
                {
                    Job dur = new Job(Job.JobType.HALT);
                    npc.AddJob(dur);
                }
            });

            butt.transform.SetParent(UIJobQueueSpawn);
            if (xindex % maxIndexOnXAxis == 0 && xindex != 0)
            {
                yindex--;
            }

            butt.transform.position = UIJobQueueSpawn.position + new Vector3((xindex % maxIndexOnXAxis) * 50, yindex * 70);
            xindex++;
        }
    }

    public Button GetCorrectButtonRef(Job.JobType jobType)
    {
        Button result;
        switch (jobType)
        {
            case Job.JobType.MOVE:
            result = MoveJobUIButton;
            break;
            case Job.JobType.HALT:
            result = HaltJobUIButton;
            break;
            case Job.JobType.ITEM_DROP:
            result = ItemDropJobUIButton;
            break;
            case Job.JobType.ITEM_RECEIVE:
            result = ItemReceiveJobUIButton;
            break;
            case Job.JobType.ITEM_TRANSFER:
            result = ItemTransferJobUIButton;
            break;
            case Job.JobType.ITEM_PICKUP:
            result = ItemPickupJobUIButton;
            break;
            case Job.JobType.WOOD_CUTTING:
            result = WoodCutUIButton;
            break;
            case Job.JobType.STONE_MINING:
            result = StoneCutUIButton;
            break;
            case Job.JobType.GIVEORDER:
            result = OrderJobtUIButton;
            break;
            case Job.JobType.GOTO:
            result = GotoJobUIButton;
            break;
            case Job.JobType.REPORT_BACK:
            result = ReportBackJobtUIButton;
            break;
            case Job.JobType.EXAMINE:
            result = ExamineJobtUIButton;
            break;
            case Job.JobType.ACTION:
            result = ActionJobtUIButton;
            break;
            default:
            result = DefaultJobUIButton;
            break;
        }
        return result;
    }

    public Button GetCorrectWorkIconRef(Work.WorkType workType)
    {
        switch (workType)
        {
            case Work.WorkType.WOOD_HARVEST_WORK:
            {
                return WoodCutUIButton;
            }
            case Work.WorkType.STONE_HARVEST_WORK:
            {
                return StoneCutUIButton;
            }
            default:
            return null;

        }
    }
}
