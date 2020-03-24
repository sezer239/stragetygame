using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInfoUI : MonoBehaviour
{

    public Text textSelectedNPCName;
    public Text textSelectedNPCSkills;
    public Text textSelectedNPCCurrentJob;
    public Text textSelectedNPCNextJob;
    public GameObject goSelectedNpcWork;
    public Button buttonSelectedNPCNWork;

    private NPCLogic npcLogic;

    public void DynamicShow(NPCLogic npcLogic)
    {
        this.npcLogic = npcLogic;
    }

    public void DisableDynamicShow()
    {
        this.npcLogic = null;
    }

    public void Show(NPCLogic npcLogic)
    {
        textSelectedNPCName.text = npcLogic.name;
        var selectedNPCJob = npcLogic.npcData.jobQueue.jobs;
        if (selectedNPCJob.Count > 0)
        {
            if (selectedNPCJob.Count == 1)
            {
                textSelectedNPCCurrentJob.text ="Doing " + selectedNPCJob[0].jobType;
                textSelectedNPCNextJob.text ="";
            }
            else
            {
                textSelectedNPCCurrentJob.text ="Doing " + selectedNPCJob[0].jobType;
                textSelectedNPCNextJob.text ="Will Do " + selectedNPCJob[1].jobType;
            }
        }
        else
        {
            textSelectedNPCCurrentJob.text ="Nothing to do...";
            textSelectedNPCNextJob.text ="";
        }

        if (npcLogic.npcData.workingOn != null && buttonSelectedNPCNWork == null)
        {
            var butt = UIManager.Instance.GetCorrectWorkIconRef(npcLogic.npcData.workingOn.GetWorkType());
            buttonSelectedNPCNWork = Instantiate(butt, Vector3.zero, Quaternion.identity);
            buttonSelectedNPCNWork.interactable = false;
            buttonSelectedNPCNWork.GetComponentInChildren<Text>().text = "";
            buttonSelectedNPCNWork.transform.SetParent(goSelectedNpcWork.transform);
            buttonSelectedNPCNWork.transform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (npcLogic != null)
            Show(npcLogic);
    }

    public void Hide()
    {
        textSelectedNPCName.text = "";
        textSelectedNPCSkills.text = "";
        textSelectedNPCCurrentJob.text = "";
        textSelectedNPCNextJob.text = "";
        if (buttonSelectedNPCNWork != null)
            Destroy(buttonSelectedNPCNWork.gameObject);
    }
}
