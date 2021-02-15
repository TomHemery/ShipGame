using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    public GameObject tabButtonPrefab;
    public GameObject [] tabs;
    public TabMenuButton[] TabMenuButtons { get; private set; }

    protected int activeTab = 0;

    private void Awake()
    {
        TabMenuButtons = new TabMenuButton[tabs.Length];
        for (int i = 0; i < tabs.Length; i++)
        {
            GameObject tabButton = Instantiate(tabButtonPrefab, transform);
            TabMenuButton button = tabButton.GetComponent<TabMenuButton>();
            
            button.index = i;
            button.onClick.AddListener(() => TabButtonClicked(button.index));
            TabMenuButtons[i] = button;

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
                TabMenuButtons[i].SetAsActiveTabButton(true);
            }
            else
            {
                tabs[i].SetActive(false);
                TabMenuButtons[i].SetAsActiveTabButton(false);
            }
        }
    }

    void TabButtonClicked(int index)
    {
        if(activeTab != index)
        {
            tabs[activeTab].SetActive(false);
            TabMenuButtons[activeTab].SetAsActiveTabButton(false);
            tabs[index].SetActive(true);
            tabs[index].GetComponent<Tab>().OnSelected?.Invoke();
            TabMenuButtons[index].SetAsActiveTabButton(true);
            activeTab = index;
        }
    }
}
