using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewButton : MonoBehaviour
{
    public void AllSelect(Transform p)
	{
        var toggles = new List<Toggle>();
        bool on = false;
        for (int i = 0; i < p.childCount; i++)
        {
            var f = p.GetChild(i).GetChild(0);
            if (!f.gameObject.activeInHierarchy) continue;
			var t = f.GetComponentInChildren<Toggle>(false);
            if (t == null) continue;
            toggles.Add(t);
            if (!t.isOn) on = true;
		}
        foreach (var t in toggles)
        {
            t.isOn = on;
        }
        
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
