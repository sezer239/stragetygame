using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorksWindowUI : MonoBehaviour
{
    public WorkEntryUI workEntryRef;

    public WorkEntryUI selectedWorkEntry;

    public WorkSelectionWindow workSelectionWindow;

    public UnityEngine.UI.Button addNewWorkButton;
    public UnityEngine.UI.Button deleteWorkButton;

    public List<WorkEntryUI> workEntries;

    public ScrollRect scrollRect;

    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        scrollRect = GetComponentInChildren<ScrollRect>();
        workEntries = new List<WorkEntryUI>();
        GameManager.Instance.onWorkAdded +=Instance_onWorkAdded;
        GameManager.Instance.onWorkDeleted +=Instance_onWorkDeleted;

        deleteWorkButton.onClick.AddListener(() =>
        {
            if (selectedWorkEntry != null)
            {
                GameManager.Instance.RemoveWork(selectedWorkEntry.work);
                selectedWorkEntry = null;
                UpdateWorkEntries();
            }
        });

        addNewWorkButton.onClick.AddListener(() =>
        {
            if (workSelectionWindow.gameObject.activeSelf) return;
            else workSelectionWindow.gameObject.SetActive(true);


            workSelectionWindow.selectButton.onClick.AddListener(() =>
            {
                Work.WorkType worktype;
                Debug.Log(workSelectionWindow.selected);
                worktype = (Work.WorkType)Enum.Parse(typeof(Work.WorkType), workSelectionWindow.selected, true);
                switch (worktype)
                {
                    case Work.WorkType.STONE_HARVEST_WORK:
                    {
                        GameManager.Instance.CreateHarvestStoneWork();
                    }
                    break;
                    case Work.WorkType.WOOD_HARVEST_WORK:
                    {
                        GameManager.Instance.CreateHarvestWoodWork();
                    }
                    break;
                }
                workSelectionWindow.gameObject.SetActive(false);
            });

        });
    }

    private void Instance_onWorkDeleted(Work work)
    {
        for (int i = 0; i < workEntries.Count; i++)
        {
            if (workEntries[i].work.Equals(work))
            {
                Debug.Log("here");
                workEntries.RemoveAt(i);
                return;
            }
        }
    }

    private void Instance_onWorkAdded(Work work)
    {
        Debug.Log("here");
        var workEntry = Instantiate(workEntryRef, canvas.transform, false);

/*        var butt = UIManager.Instance.GetCorrectWorkIconRef(work.GetWorkType());

        butt.onClick.AddListener(() => {
            Debug.Log("Spawn Correct UI Window");
        });

        butt.transform.SetParent(workEntry.interactionButtonOrigin.transform);
        workEntry.interactionButton = butt;
  */      
        workEntry.transform.SetParent(scrollRect.content);

        var rect = workEntry.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0, 1);

        workEntry.transform.localPosition = new Vector3(0, -workEntries.Count * rect.localScale.y * rect.sizeDelta.y);

        workEntry.toggleButton.onValueChanged.AddListener(delegate (bool isSelected)
        {
            if (isSelected)
            {
                Debug.Log("Selected ");
                selectedWorkEntry = workEntry;
            }
        });
        workEntries.Add(workEntry);
        workEntry.Show(work);
        workEntry.DynamicShow();
        UpdateWorkEntries();
    }

    private void UpdateWorkEntries()
    {
        for(int i = 0; i < workEntries.Count; i++)
        {
            if(workEntries[i].work == null)
            {
                Destroy(workEntries[i].gameObject);
                workEntries.RemoveAt(i);
            }
        }

        for (int i = 0; i < workEntries.Count; i++)
        {
            workEntries[i].transform.SetParent(canvas.transform);
        }

        scrollRect.content.localScale = new Vector3(scrollRect.content.localScale.x, workEntries.Count);

        for (int i = 0; i < workEntries.Count; i++)
        {
            workEntries[i].transform.SetParent(scrollRect.content);
            var rect = workEntries[i].GetComponent<RectTransform>();
            rect.transform.localPosition = new Vector3(0, -i * rect.rect.height * rect.localScale.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
