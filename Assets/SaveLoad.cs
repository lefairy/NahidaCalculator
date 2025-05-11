using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using UnityEditor.Overlays;

public class SLStruct : MonoBehaviour
{
    [Serializable] public struct Base
    {
        public string name;
        public double baseHp, baseAtk, baseDef;
    }
    [Serializable] public struct Buff
    {
        public string type;
        public double value;
    }
    [Serializable] public struct TransBuff
    {
        public string src, dest;
        public double rate, start, end;
    }
    [Serializable] public struct Rate
    {
        public List<Buff> normal;
        public List<TransBuff> trans;
        public double times;
    }
    [Serializable] public struct React
    {
        public Buff trans, amp;
        public bool ta;
    }
    [Serializable] public struct Other
    {
        public bool inc, crit, def, res, heal, shield, self;
    }
    [Serializable] public struct Skill
    {
        public string name;
        public Rate rate;
        public React react;
        public List<Buff> buff;
        public List<TransBuff> trans;
        public Other other;
    }
    public enum Types
    {
        Base,
        Buff,
        TransBuff,
        Rate,
        React,
        Other,
        Skill,
        MainSubDef,
        List
    }
}
public class SLData
{
    public SLStruct.Base character, weapon;
    public List<SLStruct.Buff> other, main, sub, maindef, subdef, cur;
    public List<SLStruct.TransBuff> trans;
    public double len;
    public List<SLStruct.Skill> skill;
    public bool isTrans;
}

public class SaveLoad : MonoBehaviour
{
    public Calc calc;
    public SLOne slTemplate;
    public SLFold slFold;
    public Transform body;
    public Transform panel;
    bool isSave = false;
    string configPath;

    public void Toggle(bool isSave)
    {
        this.isSave = isSave;
        Refresh();
    }
    public void Toggle()
    {
        isSave = !isSave;
        Refresh();
    }
    public void Refresh()
    {
        cleanAll();
        string path = Options.save_path;
        if (Directory.Exists(path))
        {
            string[] paths = Directory.GetDirectories(path);
            foreach (var p in paths)
            {
                var fold = Instantiate(slFold, body, false);
                fold.SetText(Path.GetFileName(p));
                loadFiles(fold, p);
                fold.gameObject.SetActive(true);
            }
            loadFiles(path);
        }
    }

    void loadFiles(Transform body, string fold, string path)
    {
        string[] files = Directory.GetFiles(path);
        foreach (var f in files)
        {
            if (f.EndsWith(".json"))
            {
                var sl = Instantiate(slTemplate, body, false);
                sl.path = fold;
                sl.name_i.text = Path.GetFileNameWithoutExtension(f);
                sl.button_t.text = isSave ? "保存" : "读取";
                sl.gameObject.SetActive(true);
            }
        }
    }
    void loadFiles(SLFold fold, string path)
    {
        loadFiles(fold.body, fold.GetText(), path);
    }
    void loadFiles(string path)
    {
        loadFiles(body, "", path);
    }

    void cleanAll()
    {
        SLFold[] fds = panel.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            fd.gameObject.SetActive(false);
            Destroy(fd.gameObject);
        }
        SLOne[] sls = panel.GetComponentsInChildren<SLOne>(true);
        foreach (var sl in sls)
        {
            if (sl.button_t)
            {
                sl.gameObject.SetActive(false);
                Destroy(sl.gameObject);
            }
            else
            {
                sl.gameObject.SetActive(isSave ^ !(bool)sl.name_i);
            }
        }
    }

    public void Save(string path, string name, bool isConfig = false)
    {
        string filePath = (isConfig ? configPath : Options.save_path) + (path.StartsWith('/') ? "" : "/") + path;
        Directory.CreateDirectory(filePath);
        filePath += (path.EndsWith("/") || path.Length == 0 ? "" : "/") + name + (name.EndsWith(".json") ? "" : ".json");

        string json;
        if (isConfig)
        {
            json = GetComponent<Options>().SaveOptions();
        }
        else
        {
            calc.CalcSL();
            json = calc.json;
        }
        File.WriteAllText(filePath, json);
        if (File.Exists(filePath) && File.ReadAllText(filePath) == json)
        {
            Debug.Log("保存成功: " + filePath);
        }
        else
        {
            Debug.LogWarning("保存失败: " + filePath);
        }
        if (!isConfig) Refresh();
    }

    public void Load(string file, bool isConfig = false)
    {
        string filePath = isConfig ? configPath : Options.save_path;
        filePath += (file.StartsWith('/') ? "" : "/") + file + (file.EndsWith(".json") ? "" : ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            if (isConfig)
                GetComponent<Options>().LoadOptions(json, first);
            else
                calc.Load(json);
            Debug.Log("读取成功: " + filePath);
        }
        else
        {
            Debug.LogWarning("读取失败: " + filePath);
        }
        if (!isConfig) Refresh();
        else first = false;
    }
    public void Load(string path, string name)
    {
        Load(path + "/" + name);
    }

    public void SaveConfig()
    {
        Save(string.Empty, "config.json", true);
    }
    public void LoadConfig()
    {
        Load("config.json", true);
    }

    public void Delete(string path, string name)
    {
        string filePath = Options.save_path + (path.StartsWith('/') ? "" : "/") + path;
        filePath += (path.EndsWith("/") || path.Length == 0 ? "" : "/") + name + (name.EndsWith(".json") ? "" : ".json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        Refresh();
    }
    public void Delete(string path)
    {
        if (path.Length > 0)
        {
            string filePath = Options.save_path + (path.StartsWith('/') ? "" : "/") + path;
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
            Refresh();
        }
    }

    public bool CheckFile(string path, string name)
    {
        string filePath = Options.save_path + (path.StartsWith('/') ? "" : "/") + path;
        filePath += (path.EndsWith("/") || path.Length == 0 ? "" : "/") + name + (name.EndsWith(".json") ? "" : ".json");
        return File.Exists(filePath);
    }

    public string GetJson(string path, string name)
    {
        string filePath = Options.save_path + (path.StartsWith('/') ? "" : "/") + path;
        filePath += (path.EndsWith("/") || path.Length == 0 ? "" : "/") + name + (name.EndsWith(".json") ? "" : ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("读取成功: " + filePath);
            return json;
        }
        else
        {
            Debug.LogWarning("读取失败: " + filePath);
            return "";
        }
    }
    public void LoadJson(string json)
    {
        calc.Load(json);
    }

    public bool CheckPath(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
            string testfile = path + "/testC(W艹)" + UnityEngine.Random.Range(float.MinValue, float.MaxValue);
            File.Create(testfile).Close();
            File.Delete(testfile);
            return true;
        }
        catch
        {
            return false;
        }
    }

    bool first = true;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        configPath = Application.persistentDataPath;
#else
        configPath = Application.dataPath;
#endif
    }
}
