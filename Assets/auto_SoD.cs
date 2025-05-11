using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class auto_SoD : MonoBehaviour
{
    bool first = true;
    bool is_single = false;
    Transform single_t, double_t, left_t, right_t;

    void Toggle()
    {
        if (is_single)
        {
            is_single = false;
            left_t.SetParent(double_t);
            right_t.SetParent(double_t);
        }
        else
        {
            is_single = true;
            left_t.SetParent(single_t);
            right_t.SetParent(single_t);
        }
        active.Refresh(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        single_t = transform.GetChild(0);
        double_t = transform.GetChild(1);
        left_t = double_t.GetChild(0);
        right_t = double_t.GetChild(1);
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_single != Screen.width < Screen.height)
        {
            Toggle();
        }
        if (first)
        {
            first = false;
            active.Refresh(transform);
        }
    }
	void OnDisable()
	{
        first = true;
	}
}
