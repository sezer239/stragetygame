using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeoplesWindowUI : MonoBehaviour
{

    private Animation anim;
    private ScrollRect scrollRect;

    public NPCInfoUI npcInfoUIPrefab;
    public List<NPCInfoUI> spawnedNPCUIInfo;
    private Canvas canvas;

    RTSSelector rtsSelector;
    // Start is called before the first frame update
    void Awake()
    {
        rtsSelector = FindObjectOfType<RTSSelector>();
        canvas = FindObjectOfType<Canvas>();
        spawnedNPCUIInfo = new List<NPCInfoUI>();
        anim = GetComponent<Animation>();
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        var npcs = GameManager.Instance.npcLogics;
        
        anim.Play();
        scrollRect.content.localScale = new Vector3(scrollRect.content.localScale.x , npcs.Length);
     
        for (int i = 0; i < npcs.Length; i++)
        {
            var npcInfoUI = Instantiate(npcInfoUIPrefab, canvas.transform);
            npcInfoUI.transform.SetParent(scrollRect.content);
            var rect = npcInfoUI.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0, 1);
            rect.transform.localPosition = new Vector3(0,-i * rect.rect.height * rect.localScale.y , 0);
            npcInfoUI.DynamicShow(npcs[i]);
            spawnedNPCUIInfo.Add(npcInfoUI);
        }
    }

    private void OnDisable()
    {
        for(int i = 0; i < spawnedNPCUIInfo.Count; i++)
        {
            spawnedNPCUIInfo[i].DisableDynamicShow();
            Destroy(spawnedNPCUIInfo[i].gameObject);
        }
        spawnedNPCUIInfo.Clear();
        anim.Stop();
    }
}
