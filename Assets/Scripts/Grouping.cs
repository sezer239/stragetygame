using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Formations : MonoBehaviour
{
    List<NPCLogic> Group1 = new List<NPCLogic>();
    private RTSSelector RTSSelector;

    public enum Group
    {
        Party1,
        Group2,
        Group3,
        Group4,
        Group5,
        Group6,
        Group7,
        Group8,
        Group9,
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeGroup(Group group)
    {
        var selectedNPCLogic = RTSSelector.GetSelectedAsNPCLogic();
        





        foreach (var npcLogic in selectedNPCLogic)
        {
            if (npcLogic != null) group1.Add(npcLogic);

        }
    }
    public void SelectGroup(int group)
    {
        {

            RTSSelector.ClearSelected();
            foreach (var npc in group1)
            {
                var npcselect = npc.GetComponent<Selectable>();
                RTSSelector.UpdateSelection(npcselect, true);
            }
        }
    }
}
*/