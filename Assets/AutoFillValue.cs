using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class AutoFillValue : MonoBehaviour
{
    // Start is called before the first frame update
    public ValueType type;
    Dictionary<string, double> dict;
    bool inited = false;
    public void Start()
    {
        if (inited)
            return;
        inited = true;
        if (type == ValueType.主词条属性)
        {
            dict = ValueDict.main_values;
        }
        else if (type == ValueType.副词条属性)
        {
            dict = ValueDict.sub_values;
        }
        else
        {
            return;
        }
        foreach (var item in dict)
        {
            GameObject child_obj = Instantiate(transform.GetChild(0).gameObject, transform, false);
            Transform child = child_obj.GetComponent<RectTransform>();
            Mod_InputField input = child.GetChild(1).GetChild(0).GetComponent<Mod_InputField>();
            child.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = item.Key;
            input.text = item.Value.ToString();
            input.onValueChanged.AddListener(delegate { UpdateValue(item.Key, input); } );
            child_obj.SetActive(true);
        }
    }
    void UpdateValue(string key, Mod_InputField value)
    {
        double val;
        if (double.TryParse(value.text, out val))
            dict[key] = val;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
