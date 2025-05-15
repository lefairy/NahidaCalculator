using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsChange : MonoBehaviour
{
    public GameObject[] tabs, images;
    public int def_tab = 0;
    public void ChangeTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i])
                tabs[i].SetActive(i == index);
            if (images[i])
                images[i].SetActive(i == index);
        }
        //active.Refresh(tabs[index].transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeTab(def_tab);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
