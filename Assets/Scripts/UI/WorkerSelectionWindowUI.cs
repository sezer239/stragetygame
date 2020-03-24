using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerSelectionWindowUI : MonoBehaviour
{
    public NPCCardUI npcCardUIPrefab;
    public NPCCardUI selected;
    public Button selectButton;

    public ScrollRect scrollRect;
    public List<NPCCardUI> avalibleNPCS;

    private Canvas canvas;

    void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        
        avalibleNPCS = new List<NPCCardUI>();
    }

    void Update()
    {
        
    }

    public void UpdateWorkerList()
    {
        if(canvas == null)
            canvas = FindObjectOfType<Canvas>();

        avalibleNPCS.Clear();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        var npcs = GameManager.Instance.npcLogics;
        foreach(var npc in npcs)
        {
            if(npc.npcData.workingOn == null)
            {
                var npcCard = Instantiate(npcCardUIPrefab, canvas.transform, false);
                npcCard.toggle.onValueChanged.AddListener((bool isSelected) =>
                {
                    if (isSelected)
                        selected = npcCard;
                });
                //npcCard.transform.SetParent(scrollRect.content.transform);
                npcCard.SetNpc(npc);
                avalibleNPCS.Add(npcCard);
            }
        }

        if (avalibleNPCS.Count == 0) return;
        scrollRect.content.transform.localScale = new Vector2(scrollRect.content.localScale.x, avalibleNPCS[0].transform.localScale.y * avalibleNPCS.Count);
        for (int i = 0;i < avalibleNPCS.Count; i++)
        {
            avalibleNPCS[i].rectTransform.anchorMin = new Vector2(0.5f, 1);
            avalibleNPCS[i].rectTransform.anchorMax = new Vector2(0.5f, 1);
            avalibleNPCS[i].rectTransform.pivot = new Vector2(0, 1);
            avalibleNPCS[i].transform.SetParent(scrollRect.content);
            avalibleNPCS[i].transform.localPosition = new Vector3(0, -i * avalibleNPCS[i].transform.localScale.y * avalibleNPCS[i].rectTransform.sizeDelta.y);

        }
    }

    private void OnEnable()
    {
        UpdateWorkerList();
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
     
    }
}
