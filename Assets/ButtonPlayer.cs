using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ButtonPlayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float normal, highlight, speed = 2f;
    [SerializeField] Image image;
    bool on = false;
    float val, target;

	public void OnPointerEnter(PointerEventData eventData)
    {
        on = true;
    }

	public void OnPointerExit(PointerEventData eventData)
	{
		on = false;
	}

    // Start is called before the first frame update
    void Start()
    {
        val = target = normal;
        if (!image) image = GetComponent<Image>();
        if (image)
        {
            image.pixelsPerUnitMultiplier = normal;
        }
    }
    // d = max - min = 3
    // a...z s=1 x = (z-a)/2
    //       s=2 x = (z-a)/m -> x = (z-x)/m   (z-a)m = 2(z-(z-a)/m)  zm^2-am^2=2zm-2z+2a    (z-a)m^2 - 2zm + 2(z-a) = 0
    //             = (z-a)/2
    // Update is called once per frame
    void Update()
    {
        target = on ? highlight : normal;
        if (val == target || !image) return;

        float deltaTime = Time.deltaTime; // 获取当前帧的时间间隔
        // 使用指数逼近公式：x_{n+1} = x_n + (1 - e^{-α * Δt}) * (T - x_n)
        float factor = 1.0f - Mathf.Exp(-speed * deltaTime);
        float diff = factor * (target - val);
        float min = Math.Abs(highlight - normal) * 0.02f * speed * deltaTime;
        if (Math.Abs(target - val) < min)
        {
            val = target;
        }
        else
        {
            if (Math.Abs(diff) < min)
                diff = target > val ?  min : -min;
            val += diff;
        }
        image.pixelsPerUnitMultiplier = val;
        //Debug.Log(val + " " + diff + " " + Math.Abs(highlight - normal) * 0.02f * deltaTime);
    }

}
