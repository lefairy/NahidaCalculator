using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;

public class SLOne : MonoBehaviour
{
    public Mod_InputField path_i, name_i;
    public TMP_Text button_t;
    public SaveLoad sl;
    public MessageBox mBox;
    public string path = "";

    public void SaveLoad()
    {
        if (!button_t)
            mBox.ShowBox("保存新存档至\r\n" + getFullPath() + (sl.CheckFile(path, name_i.text) ? "\r\n该存档已存在，确认覆盖？" : ""), Save, true);
        else if (button_t.text == "保存")
            mBox.ShowBox("覆盖存档\r\n" + getFullPath(), Save, true);
        else
            mBox.ShowBox("读取存档\r\n" + getFullPath(), Load, true);
    }
    public void Delete()
    {
        mBox.ShowBox("删除存档\r\n" + getFullPath(), Del, true);
    }
    public void Copy()
    {
        GUIUtility.systemCopyBuffer = sl.GetJson(path, name_i.text);
        mBox.ShowBox("存档\r\n" + getFullPath() + "\r\n已复制到剪贴板", null);
    }
    public void Paste()
    {
        path_i.text = GUIUtility.systemCopyBuffer;
    }
    public void LoadJson()
    {
        mBox.ShowBox("读取文本框中的存档内容", DoLoadJson, true);
    }
    public void SaveConfig()
    {
        mBox.ShowBox("保存当前配置到本地文件", sl.SaveConfig, true);
    }
    public void LoadConfig()
    {
        mBox.ShowBox("从本地文件读取配置，将覆盖当前配置", sl.LoadConfig, true);
    }
    public void ResetConfig()
    {
        mBox.ShowBox("将所有配置重置为默认值（不会写入文件）", sl.GetComponent<Options>().ResetOptions, true);
    }

    void Save()
    {
        sl.Save(path, name_i.text);
    }
    void Load()
    {
        sl.Load(path, name_i.text);
    }
    void Del()
    {
        sl.Delete(path, name_i.text);
    }
    void DoLoadJson()
    {
        try
        {
            sl.LoadJson(path_i.text);
        }
        catch
        {
            MessageBox.ShowBox_s("读取失败，请检查Json格式");
        }
    }
    string getFullPath()
    {
        if (path_i)
            path = path_i.text;
        return (path.Length > 0 ? path + "/" : "") + (name_i.text.Length > 0 ? name_i.text : "（未命名）");
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
