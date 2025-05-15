using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLOne : MonoBehaviour
{
    public Mod_InputField path_i, name_i;
    public TMP_Text button_t;
    public Toggle select;
    public GameObject delete;
    public SaveLoad sl;
    [NonSerialized] public string path = "", old_name = "", json = null;
    SLFold slFold;
    bool allow_set = true;

    static Regex regex = new Regex("[/\\\\:*?\"<>|]");
    static Regex regex_path = new Regex("^\\.\\.\\s*$");
    static public string CheckName(string text)
    {
        return regex.Replace(text, "");
    }
    public void CheckName(Mod_InputField input)
    {
        input.text = regex.Replace(input.text, "");
    }
    public void CheckPath(Mod_InputField input)
    {
        input.text = regex.Replace(input.text, "");
        input.text = regex_path.Replace(input.text, "");
    }
    public void Select()
    {
        if (slFold && select.isOn) slFold.Select();
    }

    public void SaveLoad()
    {
        if (json != null)
            MessageBox.ShowBox_s("读取合集中的存档\r\n" + getFullPath(), delegate { DoLoadJson(json); }, true);
        else if (!button_t)
            MessageBox.ShowBox_s("保存新存档至\r\n" + getFullPath() + (sl.CheckFile(path, name_i.text) ? "\r\n该存档已存在，确认覆盖？" : ""), delegate { sl.Save(path, name_i.text); }, true);
        else if (button_t.text == "保存")
            MessageBox.ShowBox_s("覆盖存档\r\n" + getFullPath(), delegate { sl.Save(path, name_i.text); }, true);
        else
            MessageBox.ShowBox_s("读取存档\r\n" + getFullPath(), delegate { sl.Load(path, name_i.text); }, true);
    }
    public void Delete()
    {
        MessageBox.ShowBox_s("删除存档\r\n" + getFullPath(), delegate { sl.Delete(path, name_i.text); Destroy(gameObject); }, true);
    }
    public void Rename()
    {
        if (old_name != name_i.text && allow_set)
        {
            allow_set = false;
            MessageBox.ShowBox_s("将存档\r\n" + getFullPath(true) + "\r\n重命名为\r\n" + name_i.text, delegate { allow_set = true; if (sl.Rename(path, old_name, name_i.text)) old_name = name_i.text; else { name_i.text = old_name; MessageBox.ShowBox_s("重命名失败"); } }, true, delegate { allow_set = true; name_i.text = old_name; });
        }
    }
    public void Copy()
    {
        if (json != null)
        {
            GUIUtility.systemCopyBuffer = json;
            MessageBox.ShowBox_s("合集中的存档\r\n" + getFullPath() + "\r\n内容已复制到剪贴板");
        }
        else if (name_i)
        {
            if (delete)
            {
                GUIUtility.systemCopyBuffer = sl.GetJson(path, name_i.text);
                MessageBox.ShowBox_s("存档\r\n" + getFullPath() + "\r\n内容已复制到剪贴板");
            }
            else
            {
                GUIUtility.systemCopyBuffer = name_i.text;
                MessageBox.ShowBox_s("存档合集已复制到剪贴板");
            }
        }
    }
    public void Paste()
    {
        if (path_i) path_i.text = GUIUtility.systemCopyBuffer;
        else if (name_i) name_i.text = GUIUtility.systemCopyBuffer;
    }
    public void LoadJson()
    {
        MessageBox.ShowBox_s("读取文本框中的存档内容", delegate { DoLoadJson(path_i.text); }, true);
    }
    public void SaveConfig()
    {
        MessageBox.ShowBox_s("保存当前配置到本地文件", sl.SaveConfig, true);
    }
    public void LoadConfig()
    {
        MessageBox.ShowBox_s("从本地文件读取配置，将覆盖当前配置", sl.LoadConfig, true);
    }
    public void ResetConfig()
    {
        MessageBox.ShowBox_s("将所有配置重置为默认值（不会写入文件）", sl.GetComponent<Options>().ResetOptions, true);
    }
    public void MakeCollection()
    {
        MessageBox.ShowBox_s("根据所有选中的存档生成合集", delegate{ name_i.text = sl.MakeCollection(); Debug.Log(name_i.text.Length); }, true);
    }
    public void ParseCollection()
    {
        MessageBox.ShowBox_s("从文本框中临时读取存档合集", delegate{ sl.ParseCollection(name_i.text); }, true);
    }
    public void SaveCollection()
    {
        if (button_t.text == "写入存档")
            sl.SaveCollection();
        else
            MessageBox.ShowBox_s("删除选中的文件夹和存档（选中文件夹时若文件夹内没有存档或其中的所有存档都被选中，则会删除整个文件夹及其中的所有文件）", sl.DeleteSelect, true);
    }

    void DoLoadJson(string text)
    {
        try
        {
            sl.LoadJson(text);
        }
        catch
        {
            MessageBox.ShowBox_s("读取失败，请检查Json格式");
        }
    }
    string getFullPath(bool old = false)
    {
        if (path_i)
            path = path_i.text;
        string name = old ? old_name : name_i.text;
        return (path.Length > 0 ? path + "/" : "") + (name.Length > 0 ? name : "（未命名）");
    }

    // Start is called before the first frame update
    void Start()
    {
        slFold = transform.parent.parent.parent.GetComponent<SLFold>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
