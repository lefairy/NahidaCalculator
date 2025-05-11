using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsChange : MonoBehaviour
{
    public GameObject[] tabs, images;
    public void ChangeTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == index);
            images[i].SetActive(i == index);
        }
        active.Refresh(tabs[index].transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeTab(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
