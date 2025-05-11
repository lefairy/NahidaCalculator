using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SilderRotate : MonoBehaviour, IEndDragHandler
{
    [SerializeField] double step;
    [SerializeField] Mod_InputField input;
    Slider slider;
    RectTransform rect;
    bool allow_set = true;
    public void OnEndDrag(PointerEventData eventData)
    {
        slider.value = (float)limitStep(slider.value);
    }
    public void SetRotate()
    {
        rect.SetLocalPositionAndRotation(rect.localPosition, Quaternion.Euler(0, 0, slider.value * -45f));
        if (!allow_set)
        {
            return;
        }
        if (input)
        {
            Debug.Log("set");
            input.text = limitStep(slider.value).ToString();
        }
    }
    public void SetValue()
    {
        if (float.TryParse(input.text, out float value))
        {
            float val = (float)limitStep(value);
            allow_set = val == slider.value;
            slider.value = val;
            allow_set = true;
        }
    }
    double limitStep(float value)
    {
        if (step == 0) return value;
        return Math.Round((value - slider.minValue) / step) * step +  slider.minValue;
    }
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        rect = slider.handleRect;
        rect.SetLocalPositionAndRotation(rect.localPosition, Quaternion.Euler(0, 0, slider.value * -45f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
