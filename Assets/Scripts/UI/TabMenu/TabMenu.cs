using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    public GameObject tabButtonPrefab;
    public GameObject [] tabs;
    private TabMenuButton[] tabMenuButtons;

    protected int activeTab = 0;

    private void Awake()
    {
        tabMenuButtons = new TabMenuButton[tabs.Length];
        for (int i = 0; i < tabs.Length; i++)
        {
            GameObject tabButton = Instantiate(tabButtonPrefab, transform);
            TabMenuButton button = tabButton.GetComponent<TabMenuButton>();
            
            button.index = i;
            button.onClick.AddListener(() => TabButtonClicked(button.index));
            tabMenuButtons[i] = button;

            Text buttonText = tabButton.GetComponentInChildren<Text>();
            buttonText.text = tabs[i].GetComponent<Tab>().TabName;
        }
    }

    private void Start()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == activeTab)
            {
                tabs[i].SetActive(true);
                tabMenuButtons[i].SetAsActiveTabButton(true);
            }
            else
            {
                tabs[i].SetActive(false);
                tabMenuButtons[i].SetAsActiveTabButton(false);
            }
        }
    }

    void TabButtonClicked(int index)
    {
        if(activeTab != index)
        {
            tabs[activeTab].SetActive(false);
            tabMenuButtons[activeTab].SetAsActiveTabButton(false);
            tabs[index].SetActive(true);
            tabMenuButtons[index].SetAsActiveTabButton(true);
            activeTab = index;
        }
    }
}
