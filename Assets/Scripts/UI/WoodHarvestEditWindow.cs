using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WoodHarvestEditWindow : MonoBehaviour
{
    public HarvestWoodWork context;
    public Canvas canvas;

    public WorkerSelectionWindowUI workerSelection;

    public Text jobNameText;
    public Text cheifNameText;

    public bool setLocationUsingMouse;

    public GameObject dropLocation;
    public GameObject woodCutLocation;
    public GameObject cheifStantLocation;

    public Button addWorkerButton;
    public Button fireWorkerButton;
    public Button assingCheifButton;

    public Button changePositionButton;

    public float area;
    public Transform position;

    public ScrollRect scrollRect;
    public List<NPCCardUI> workers;

    public NPCCardUI npcCardUIPrefab;
    private NPCCardUI selected;
    
    public void SetContext(HarvestWoodWork cntx)
    {
        context = cntx;
        context.cheifStandPosition = cheifStantLocation.transform;
        context.dropOffLocation = new Transform[] { dropLocation.transform } ;
        context.position = woodCutLocation.transform;
        UpdateWindow();
    }

    public void UpdateWindow()
    {
        jobNameText.text = context.name;
        if (context.cheif != null)
            cheifNameText.text = context.cheif.name;
        else
            cheifNameText.text = "No Cheif";
        UpdateWorkerList();
    }

    public void IncrementArea()
    {
        area++;
    }

    public void DerementArea()
    {
        area--;
    }
  
    public void ApplyChanges()
    {
        context.SetArea(position, area);
        UpdateWindow();
    }

    // Start is called before the first frame update
    void Start()
    {
        assingCheifButton.onClick.AddListener(() =>
        {
            workerSelection.gameObject.SetActive(true);
            workerSelection.selectButton.onClick.AddListener(() =>
            {
                context.SetCheif(workerSelection.selected.npc);
                workerSelection.gameObject.SetActive(false);
                UpdateWindow();
            });
        });


        addWorkerButton.onClick.AddListener(() =>
        {
            workerSelection.gameObject.SetActive(true);
            workerSelection.selectButton.onClick.AddListener(() =>
            {
                context.AddWorker(workerSelection.selected.npc);
                workerSelection.UpdateWorkerList();
                UpdateWindow();
            });
        });
    }

    public void UpdateWorkerList()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        workers.Clear();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        var npcs = context.npcsWorking;
        foreach (var npc in npcs)
        {
            if (npc.npcData.workingOn != null)
            {
                var npcCard = Instantiate(npcCardUIPrefab, canvas.transform, false);
                npcCard.toggle.onValueChanged.AddListener((bool isSelected) =>
                {
                    if (isSelected)
                        selected = npcCard;
                });
                //npcCard.transform.SetParent(scrollRect.content.transform);
                npcCard.SetNpc(npc);
                workers.Add(npcCard);
            }
        }

        if (workers.Count == 0) return;
        scrollRect.content.transform.localScale = new Vector2(scrollRect.content.localScale.x, workers[0].transform.localScale.y * workers.Count);
        for (int i = 0; i < workers.Count; i++)
        {
            workers[i].rectTransform.anchorMin = new Vector2(0.5f, 1);
            workers[i].rectTransform.anchorMax = new Vector2(0.5f, 1);
            workers[i].rectTransform.pivot = new Vector2(0, 1);
            workers[i].transform.SetParent(scrollRect.content);
            workers[i].transform.localPosition = new Vector3(0, -i * workers[i].transform.localScale.y * workers[i].rectTransform.sizeDelta.y);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (setLocationUsingMouse)
        {

        }
        else
        {

        }
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }
}
