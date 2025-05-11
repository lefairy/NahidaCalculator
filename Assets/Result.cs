using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] Calc calc;
    [SerializeField] Transform statusPanel;
    [SerializeField] Scrollbar skillbar;
    [SerializeField] Mod_InputField entry, dmg, dps, max;
    [SerializeField] Toggle includeTrans;
    Status status;
    double[] val = new double[(int)Calc.Type.元素抗性], trans = new double[(int)Calc.Type.元素抗性];
    double total_entry = 0;
    bool two = false;
    RectTransform entry_rect;

    class Status
    {
        public Mod_InputField[] value;
        public Mod_InputField[] entry;
        public Mod_InputField[] rate;
        public Status(Transform panel)
        {
            value = new Mod_InputField[(int)Calc.Type.元素抗性];
            entry = new Mod_InputField[(int)Calc.Type.元素抗性];
            rate = new Mod_InputField[(int)Calc.Type.元素抗性];
            Transform template = panel.GetChild(0);
            for (int i = 0; i < (int)Calc.Type.元素抗性; i++)
            {
                Transform item = Instantiate(template, panel, false);
                item.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = ValueDict.type_strings[i];
                value[i] = item.GetChild(1).GetChild(0).GetComponent<Mod_InputField>();
                entry[i] = item.GetChild(2).GetChild(0).GetComponent<Mod_InputField>();
                rate[i] = item.GetChild(3).GetChild(0).GetComponent<Mod_InputField>();
            }
            Destroy(template.gameObject);
        }
    }
    public bool isTrans
    {
        get { return includeTrans.isOn; }
        set { includeTrans.isOn = value;}
    }
    public void ToggleMode()
    {
        status.value[(int)Calc.Type.元素精通].transform.parent.parent.gameObject.SetActive(Options.mode == Options.原神);
        status.value[(int)Calc.Type.速度].transform.parent.parent.gameObject.SetActive(Options.mode == Options.星穹铁道);
        status.value[(int)Calc.Type.击破特攻].transform.parent.parent.gameObject.SetActive(Options.mode == Options.星穹铁道);
    }
    public void SetValues(List<SLStruct.Buff> buffs)
    {
        if (buffs != null)
        {
            Array.Clear(val, 0, val.Length);
            foreach (var buff in buffs)
            {
                if (buff.type != null && buff.type.Length > 0)
                {
                    int index = Array.IndexOf(ValueDict.type_strings, buff.type);
                    if (index >= 0 && index < val.Length)
                    {
                        val[index] += buff.value;
                    }
                }
            }
            Array.Copy(val, trans, trans.Length);
        }
        SetValues(null, null);
    }
    public void SetValues(double[] values, double[] trans_values = null, double[] entrys = null, double[] rates = null)
    {
        if (values != null)
        {
            Array.Copy(values, val, val.Length);
            if (values.Length >= (int)Calc.Type.len)
                Array.Copy(values, (int)Calc.Type.总生命值, val, 0, 3);
        }
        if (trans_values != null)
        {
            Array.Copy(trans_values, trans, trans.Length);
            if (trans_values.Length >= (int)Calc.Type.len)
                Array.Copy(trans_values, (int)Calc.Type.总生命值, trans, 0, 3);
        }
        double[] target = includeTrans.isOn ? trans : val;
        for (int i = 0; i < status.value.Length; i++)
        {
            if (target != null)
            {
                if (i <= (int)Calc.Type.速度)
                {
                    status.value[i].text = target[i] >= 10000 ? target[i].ToString("0.#") : target[i].ToString("0.##");
                }
                else
                {
                    status.value[i].text = target[i].ToString("0.##%");
                }
            }
            if (i < (int)Calc.Type.护盾强效)
            {
                if (entrys != null)
                    status.entry[i].text = entrys[i].ToString("0.##");
                if (rates != null)
                    status.rate[i].text = rates[i].ToString("0.##%");
            }
            else
            {
                status.entry[i].text = "—";
                status.rate[i].text = "—";
            }
        }
        if (entrys != null)
        {
            total_entry = entrys.Sum();
            UpdateEntry();
        }
    }
    public void SetValues()
    {
        SetValues(null);
        //for (int i = 0; i < status.value.Length; i++)
        //{
        //    status.value[i].interactable = !includeTrans.isOn;
        //}
    }
    public void CleanValues()
    {
        for (int i = 0; i < status.value.Length; i++)
        {
            status.value[i].text = "";
            status.entry[i].text = "";
            status.rate[i].text = "";
            val[i] = 0;
            trans[i] = 0;
        }
    }
    public List<SLStruct.Buff> GetValuesList()
    {
        var buffs = new List<SLStruct.Buff>();
        var values = GetValues();
        for (int i = 0; i < values.Length; i++)
        {
            buffs.Add(new SLStruct.Buff { type = ValueDict.type_strings[i], value = values[i] });
        }
        return buffs;
    }
    public double[] GetValues(out bool isTrans)
    {
        double[] val = new double[status.value.Length];
        isTrans = includeTrans.isOn;
        __GetValues(val);
        return val;
    }
    public double[] GetValues()
    {
        double[] val = new double[status.value.Length];
        bool old = includeTrans.isOn;
        if (old)
        {
            includeTrans.isOn = false;
        }
        __GetValues(val);
        includeTrans.isOn = old;
        return val;
    }
    void __GetValues(double[] val)
    {
        for (int i = 0; i < status.value.Length; i++)
        {
            double.TryParse(status.value[i].text.TrimEnd('%'), out val[i]);
            if (i > (int)Calc.Type.速度)
            {
                val[i] /= 100;
            }
        }
    }
    public double[] GetEntrys()
    {
        double[] val = new double[status.entry.Length];
        for (int i = 0; i < status.entry.Length; i++)
        {
            double.TryParse(status.entry[i].text, out val[i]);
        }
        return val;
    }

    public bool isEmpty()
    {
        double[] val = GetValues(out var a);
        for (int i = 0; i < val.Length; i++)
        {
            if (val[i] != 0)
                return false;
        }
        val = GetEntrys();
        for (int i = 0; i < val.Length; i++)
        {
            if (val[i] != 0)
                return false;
        }
        return true;
    }

    public void SetDmg(Calc.CalcSkill[] skills, double dmg, double dps, double max)
    {
        Transform parent = skillbar.transform.parent;
        for (int i = parent.childCount - 1; i > 0; i--)
        {
            GameObject obj = parent.GetChild(i).gameObject;
            obj.SetActive(false);
            Destroy(obj);
        }
        for (int i = 0; i < skills.Length; i++)
        {
            Scrollbar s = Instantiate(skillbar, parent, false);
            double percent = dmg != 0 ? skills[i].dmg / dmg : 0;
            s.size = (float)percent;
            s.transform.GetChild(1).GetComponent<TMP_Text>().text = skills[i].name;
            s.transform.GetChild(2).GetComponent<TMP_Text>().text = skills[i].dmg.ToString("0.## ") + percent.ToString("(0.##%)");
            s.gameObject.SetActive(true);
        }
        active.Refresh(skillbar.transform);
        this.dmg.text = dmg.ToString("0.##");
        this.dps.text = dps.ToString("0.##");
        this.max.text = max.ToString("0.##");
    }

    void UpdateEntry()
    {
        entry.text = total_entry.ToString(two ? "0.##词条" : "0.#词条");
    }
    // Start is called before the first frame update
    void Awake()
    {
        status = new Status(statusPanel);
        entry_rect = entry.transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        bool cur_two = Options.force_entry_two || entry_rect.sizeDelta.x > 260;
        if (two != cur_two)
        {
            two = cur_two;
            if (total_entry != 0) UpdateEntry();
        }
    }
}
