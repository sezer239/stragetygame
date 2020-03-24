using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkSelectionWindow : MonoBehaviour
{
    public Button selectButton;
    public Dropdown dropdown;

    private List<Dropdown.OptionData> optionDatas;
    public string selected;

    // Start is called before the first frame update
    void Start()
    {
        optionDatas = new List<Dropdown.OptionData>();
        optionDatas.Add(new Dropdown.OptionData( Work.WorkType.STONE_HARVEST_WORK + "" ));
        optionDatas.Add(new Dropdown.OptionData( Work.WorkType.WOOD_HARVEST_WORK + "" ));
        dropdown.onValueChanged.AddListener((int index) => {
            selected = dropdown.options[index].text;
        });
        dropdown.AddOptions(optionDatas);
        selected = dropdown.options[0].text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
    }
}
