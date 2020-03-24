using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkEntryUI : MonoBehaviour
{
    public Toggle toggleButton;
    public GameObject interactionButtonOrigin;
    public Button interactionButton;
    public Text cheifNameText;
    public Text workerCountText;

    public Work work;
    public bool dynamicShow = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dynamicShow)
            Show(work);
    }

    public void Show(Work work)
    {
        this.work = work;
        cheifNameText.text = work.cheif != null ? work.cheif.name : "No Cheif";
        workerCountText.text = work.npcsWorking.Count + "";
        if (interactionButton == null)
        {
            var workType = work.GetWorkType();
            var workButt = UIManager.Instance.GetCorrectWorkIconRef(workType);
          
            interactionButton = Instantiate(workButt, interactionButtonOrigin.transform, false);
            interactionButton.transform.localPosition = Vector3.zero;
            interactionButton.GetComponentInChildren<Text>().text = "Wood Cutting";
            interactionButton.interactable = true;
            interactionButton.onClick.AddListener(() =>
            {
                if (workType.Equals(Work.WorkType.WOOD_HARVEST_WORK))
                {
                    var woodHarvestEditWinodow = UIManager.Instance.woodHarvestEditWindow;
                    woodHarvestEditWinodow.gameObject.SetActive(false);
                    woodHarvestEditWinodow.gameObject.SetActive(true);
                    woodHarvestEditWinodow.SetContext((HarvestWoodWork)work);
                }
            });
        }
    }

    public void DynamicShow()
    {
        dynamicShow = true;
    }

    public void DynamicShowClose()
    {
        dynamicShow = false;
    }
}
