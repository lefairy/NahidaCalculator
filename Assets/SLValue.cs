using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SLValue))]//关联之前的脚本
public class NewBehaviourScript1 : Editor
{
    SerializedObject test;
    SerializedProperty type;
    SerializedProperty[] list;
    void OnEnable()
    {
        test = new SerializedObject(target);
        type = test.FindProperty("type");
        list = new SerializedProperty []{ test.FindProperty("baseItem"), test.FindProperty("buffItem"), test.FindProperty("transItem"), test.FindProperty("rateItem"), test.FindProperty("reactItem"), test.FindProperty("otherItem"), test.FindProperty("skillItem"), null, test.FindProperty("listItem") };
    }
    public override void OnInspectorGUI()
    {
        test.Update();//更新test
        EditorGUILayout.PropertyField(type);
        if (list[type.enumValueIndex] != null)
        {
            EditorGUILayout.PropertyField(list[type.enumValueIndex]);
        }
        test.ApplyModifiedProperties();
    }
}
#endif

public class SLValue : SLStruct
{
    public Types type;
    public BaseItem baseItem;
    public BuffItem buffItem;
    public TransItem transItem;
    public ListItem listItem;
    public RateItem rateItem;
    public ReactItem reactItem;
    public OtherItem otherItem;
    public SkillItem skillItem;

    [Serializable]
    public struct BaseItem
    {
        public Mod_InputField name, hp, atk, def;
    }
    [Serializable]
    public struct BuffItem
    {
        public Mod_InputField type, value;
        public TMP_Text const_type;
        public CDButton remove;
    }
    [Serializable]
    public struct TransItem
    {
        public Mod_InputField src, dest, rate, start, end;
        public CDButton remove;
    }
    [Serializable]
    public struct ListItem
    {
        public CDButton add;
    }
    [Serializable]
    public struct RateItem
    {
        public SLValue normal, trans;
        public Mod_InputField times;
    }
    [Serializable]
    public struct ReactItem
    {
        public Mod_InputField trans, trans_val, amp, amp_val;
        public Toggle ta;
    }
    [Serializable]
    public struct OtherItem
    {
        public Toggle inc, crit, def, res, heal, shield, self;
    }
    [Serializable]
    public struct SkillItem
    {
        public Mod_InputField name;
        public SLValue rate, react, buff, trans, other;
        public CDButton remove;
    }


    public object GetValue()
    {
        switch (type)
        {
            case Types.Base:
                {
                    var o = new Base();
                    o.name = baseItem.name.text;
                    double.TryParse(baseItem.hp.text, out o.baseHp);
                    double.TryParse(baseItem.atk.text, out o.baseAtk);
                    double.TryParse(baseItem.def.text, out o.baseDef);
                    return o;
                }
            case Types.Buff:
                {
                    var o = new Buff();
                    if (buffItem.const_type != null)
                    {
                        o.type = buffItem.const_type.text;
                    }
                    else
                    {
                        o.type = buffItem.type.text;
                    }
                    double.TryParse(buffItem.value.text, out o.value);
                    return o;
                }
            case Types.TransBuff:
                {
                    var o = new TransBuff();
                    o.src = transItem.src.text;
                    o.dest = transItem.dest.text;
                    double.TryParse(transItem.rate.text, out o.rate);
                    double.TryParse(transItem.start.text, out o.start);
                    double.TryParse(transItem.end.text, out o.end);
                    return o;
                }
            case Types.Rate:
                {
                    var o = new Rate();
                    o.normal = (List<Buff>)rateItem.normal.GetValue<Buff>();
                    o.trans = (List<TransBuff>)rateItem.trans.GetValue<TransBuff>();
                    double.TryParse(rateItem.times.text, out o.times);
                    return o;
                }
            case Types.React:
                {
                    var o = new React();
                    o.trans.type = reactItem.trans.text;
                    double.TryParse(reactItem.trans_val.text, out o.trans.value);
                    o.amp.type = reactItem.amp.text;
                    double.TryParse(reactItem.amp_val.text, out o.amp.value);
                    o.ta = reactItem.ta.isOn;
                    return o;
                }
            case Types.Other:
                {
                    var o = new Other();
                    o.inc = otherItem.inc.isOn;
                    o.crit = otherItem.crit.isOn;
                    o.def = otherItem.def.isOn;
                    o.res = otherItem.res.isOn;
                    o.heal = otherItem.heal.isOn;
                    o.shield = otherItem.shield.isOn;
                    o.self = otherItem.self.isOn;
                    return o;
                }
            case Types.Skill:
                {
                    var o = new Skill();
                    o.name = skillItem.name.text;
                    o.rate = (Rate)skillItem.rate.GetValue();
                    o.react = (React)skillItem.react.GetValue();
                    o.buff = (List<Buff>)skillItem.buff.GetValue<Buff>();
                    o.trans = (List<TransBuff>)skillItem.trans.GetValue<TransBuff>();
                    o.other = (Other)skillItem.other.GetValue();
                    return o;
                }
            default:
                return null;
        }
    }
    public object GetValue<T>()
    {
        switch (type)
        {
            case Types.List:
                {
                    var o = new List<T>();
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        GameObject obj = transform.GetChild(i).gameObject;
                        if (obj.activeSelf && obj.GetComponent<SLValue>() != null)
                        {
                            o.Add((T)obj.GetComponent<SLValue>().GetValue());
                        }

                    }
                    return o;
                }
            default:
                return null;
        }
    }
    public void SetValue(object val)
    {
        switch (type)
        {
            case Types.Base:
                {
                    var o = (Base)val;
                    baseItem.name.text = o.name;
                    baseItem.hp.text = o.baseHp.ToString();
                    baseItem.atk.text = o.baseAtk.ToString();
                    baseItem.def.text = o.baseDef.ToString();
                    break;
                }
            case Types.Buff:
                {
                    var o = (Buff)val;
                    if (buffItem.const_type == null)
                    {
                        buffItem.type.text = o.type;
                    }
                    buffItem.value.text = o.value.ToString();
                    break;
                }
            case Types.TransBuff:
                {
                    var o = (TransBuff)val;
                    transItem.src.text = o.src;
                    transItem.dest.text = o.dest;
                    transItem.rate.text = o.rate.ToString();
                    transItem.start.text = o.start.ToString();
                    transItem.end.text = o.end.ToString();
                    break;
                }
            case Types.Rate:
                {
                    var o = (Rate)val;
                    rateItem.normal.SetValue(o.normal);
                    rateItem.trans.SetValue(o.trans);
                    rateItem.times.text = o.times.ToString();
                    break;
                }
            case Types.React:
                {
                    var o = (React)val;
                    reactItem.trans.text = o.trans.type;
                    reactItem.trans_val.text = o.trans.value.ToString();
                    reactItem.amp.text = o.amp.type;
                    reactItem.amp_val.text = o.amp.value.ToString();
                    reactItem.ta.isOn = o.ta;
                    break;
                }
            case Types.Other:
                {
                    var o = (Other)val;
                    otherItem.inc.isOn = o.inc;
                    otherItem.crit.isOn = o.crit;
                    otherItem.def.isOn = o.def;
                    otherItem.res.isOn = o.res;
                    otherItem.heal.isOn = o.heal;
                    otherItem.shield.isOn = o.shield;
                    otherItem.self.isOn = o.self;
                    break;
                }
            case Types.Skill:
                {
                    var o = (Skill)val;
                    skillItem.name.text = o.name;
                    skillItem.rate.SetValue(o.rate);
                    skillItem.react.SetValue(o.react);
                    skillItem.buff.SetValue(o.buff);
                    skillItem.trans.SetValue(o.trans);
                    skillItem.other.SetValue(o.other);
                    break;
                }
        }
    }
    public void SetValue<T>(List<T> val)
    {
        switch (type)
        {
            case Types.List:
                {
                    var o = val;
                    for (int i = 0, j = 0; i < transform.childCount; i++)
                    {
                        GameObject obj = transform.GetChild(i).gameObject;
                        if (!obj.activeSelf)
                        {
                            continue;
                        }
                        if (j < o.Count)
                        {
                            if (obj.GetComponent<SLValue>() == null)
                            {
                                obj = listItem.add.CopyItem(i);
                            }
                            if (obj.activeSelf && obj.GetComponent<SLValue>() != null)
                            {
                                obj.GetComponent<SLValue>().SetValue(o[j]);
                            }
                            j++;
                        }
                        else
                        {
                            if (obj.GetComponent<SLValue>() != null)
                            {
                                obj.GetComponent<SLValue>().Remove();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
                }
        }
    }
    public void Remove()
    {
        switch (type)
        {
            case Types.Buff:
                {
                    if (buffItem.remove)
                    {
                        buffItem.remove.DeleteItem();
                    }
                    break;
                }
            case Types.TransBuff:
                {
                    if (transItem.remove)
                    {
                        transItem.remove.DeleteItem();
                    }
                    break;
                }
            case Types.Skill:
                {
                    if (skillItem.remove)
                    {
                        skillItem.remove.DeleteItem();
                    }
                    break;
                }
        }
    }
}
