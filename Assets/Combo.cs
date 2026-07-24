using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.EventSystems;

public class Combo : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool allow_show = true;
    Mod_InputField input;
    [SerializeField] Transform template, content;
    [SerializeField] RectTransform item;
    [SerializeField] Mod_InputField text;
    float one_height;
    public ValueType type;
    string[] values;
    bool allow_fill = true;

    public void Show(bool change)
    {
        if (change)
        {
            allow_fill = true;
        }
        Show();
    }
    public void Show()
    {
        Refresh();
        if (allow_show)
        {
            template.gameObject.SetActive(true);
        }
    }

    public void Refresh(bool change)
    {
        if (change)
        {
            allow_fill = true;
        }
        Refresh();
    }
    public void Refresh()
    {
        int cnt = 0;
        if (values == null)
        {
            return;
        }
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == input.text)
            {
                cnt = AllActive();
                break;
            }
            else if (values[i].IndexOf(input.text) != -1)
            {
                cnt += OneActive(i);
            }
            else
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
        ((RectTransform)template).sizeDelta = new Vector2(0, Math.Max(Math.Min(cnt, 6), 1) * one_height + 20);
    }
    int AllActive()
    {
        int ret = 0;
        for (int i = 0; i < values.Length; i++)
        {
            ret += OneActive(i);
        }
        return ret;
    }
    int OneActive(int i)
    {
        if (Options.mode == Options.原神 && (values[i] == "速度" || values[i] == "击破特攻%" || values[i] == "笑点" ||
            values[i] == "欢愉度%" || values[i] == "速度倍率%" || values[i] == "击破倍率%" || values[i] == "欢愉倍率%"))
        {
            content.GetChild(i).gameObject.SetActive(false);
            return 0;
        }
        else if (Options.mode == Options.星穹铁道 && (values[i] == "元素精通" || values[i] == "剧变加成%" ||
            values[i] == "增幅加成%" || values[i] == "反应伤害值提升" || values[i] == "反应基础伤害提升%" || values[i] == "反应层数" || values[i] == "元素精通倍率%"))
        {
            content.GetChild(i).gameObject.SetActive(false);
            return 0;
        }
        content.GetChild(i).gameObject.SetActive(true);
        return 1;
	}
    public void Hide()
    {
        template.gameObject.SetActive(false);
    }
    public void Select(TMP_Text t)
    {
        input.text = t.text;
        input.DeactivateInputField();
        allow_fill = true;
        FillValue();
        Hide();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void FillValue()
    {
        if (allow_fill)
        {
            try
            {
                text.text = Convert.ToString(ValueDict.main_values[input.text]);
                allow_fill = false;
            }
            catch { }
        }
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (input != null)
    //    {
    //        input.Select();
    //        input.ActivateInputField();
    //    }
    //}
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    eventData.Use(); // 阻止事件传播
    //}
            //Transform parent = transform.parent;
            //while (parent != null)
            //{
            //    ExecuteEvents.Execute(parent.gameObject, eventData, ExecuteEvents.dragHandler);
            //    parent = parent.parent;
            //}

    void Start()
    {
        values = ValueDict.buff_types[type];
        input = GetComponent<Mod_InputField>();
        one_height = content.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        if (item != null)
        {
            foreach (string value in values)
            {
                GameObject it = Instantiate(item.gameObject, content, false);
                it.GetComponent<TMP_Text>().text = value;
            }
            Destroy(item.gameObject);
        }
        if (text && text.text.Length == 0)
            FillValue();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
