using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCCardUI : MonoBehaviour
{
    public Text nameText;
    public Toggle toggle;

    public NPCLogic npc;
    public RectTransform rectTransform;
    public void SetNpc(NPCLogic npc)
    {
        this.npc = npc;
        nameText.text = npc.name;
    }
    // Start is called before the first frame update
    void Start()
    {
        //rectTransform = GetComponent<RectTransform>();
        //nameText = GetComponentInChildren<Text>();
        //toggle = GetComponentInChildren<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
