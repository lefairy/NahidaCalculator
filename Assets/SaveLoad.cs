using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO.Compression;
using UnityEngine.Events;
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
    public MessageBox box1;
    public TMP_Text saveCollection;
    [Serializable]
    public struct SLCollection
    {
        public List<SLFolds> folds;
        public SLCollection(List<SLFolds> folds)
        {
            this.folds = folds;
        }
    }
    SLCollection collection;
    Type stat = Type.Load;
    [Serializable]
    public struct SLFolds
    {
        public string name;
        public List<string> files, jsons;
        public SLFolds(string name)
        {
            this.name = name;
            files = new List<string>();
            jsons = new List<string>();
        }
    }
    public enum Type
    {
        Save,
        Load,
        Multi
    }
    string configPath;

    public string MakeCollection()
    {
        collection = new SLCollection(new List<SLFolds>());
        SLFolds fold;
        SLFold[] fds = panel.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            if (fd.select.isOn)
            {
                fold = new SLFolds(fd.GetText());
                SLOne[] sls = fd.GetComponentsInChildren<SLOne>(true);
                foreach (var sl in sls)
                {
                    if (sl.select.isOn)
                    {
                        string file = sl.name_i.text + (sl.name_i.text.EndsWith(".json") ? "" : ".json");
                        string json = sl.json ?? GetJson(sl.path, file);
                        if (json.Length > 0)
                        {
                            fold.files.Add(file);
                            fold.jsons.Add(json);
                        }
                    }
                }
                collection.folds.Add(fold);
            }
        }
        fold = new SLFolds("");
        for (int i = fds.Length > 0 ? fds[^1].transform.GetSiblingIndex() + 1 : 0; i < body.childCount; i++)
        {
            SLOne one = body.GetChild(i).GetComponent<SLOne>();
            if (!one)
            {
                continue;
            }
            if (one.select.isOn)
            {
                string file = one.name_i.text + (one.name_i.text.EndsWith(".json") ? "" : ".json");
                string json = one.json ?? GetJson(one.path, file);
                if (json.Length > 0)
                {
                    fold.files.Add(file);
                    fold.jsons.Add(json);
                }
            }
        }
        collection.folds.Add(fold);

        string result = JsonUtility.ToJson(collection);
        string zipString;
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(System.Text.Encoding.UTF8.GetBytes(result));
            }
            zipString = Convert.ToBase64String(memoryStream.ToArray());
        }
        return zipString;
    }

    public void ParseCollection(string zipString)
    {
        string result;
        using (var inputStream = new MemoryStream(Convert.FromBase64String(zipString)))
        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            gzipStream.CopyTo(resultStream);
            result = System.Text.Encoding.UTF8.GetString(resultStream.ToArray());
        }
        collection = JsonUtility.FromJson<SLCollection>(result);
        cleanAll(true);
        foreach (var fold in collection.folds)
        {
            if (fold.name.Length > 0)
            {
                var fd = Instantiate(slFold, body, false);
                fd.SetText(fold.name);
                fd.select.transform.parent.gameObject.SetActive(true);
                fd.delete.SetActive(stat != Type.Multi);

                loadFiles(fd.body, fold.name, fold.files.ToArray(), fold.jsons.ToArray());
                fd.gameObject.SetActive(true);
            }
            else
            {
                loadFiles(body, "", fold.files.ToArray(), fold.jsons.ToArray());
            }
        }
        saveCollection.text = "写入存档";
    }

    public void SaveCollection()
    {
        string sames = "";

        SLFold[] fds = panel.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            if (fd.select.isOn && Directory.Exists(FullPath(fd.GetText())))
            {
                SLOne[] sls = fd.GetComponentsInChildren<SLOne>(true);
                foreach (var sl in sls)
                {
                    if (sl.select.isOn && File.Exists(FullPath(sl.path, sl.name_i.text)))
                    {
                        sames += fd.GetText() + "/" + sl.name_i.text + "\n";
                    }
                }
            }
        }
        for (int i = fds.Length > 0 ? fds[^1].transform.GetSiblingIndex() + 1 : 0; i < body.childCount; i++)
        {
            SLOne one = body.GetChild(i).GetComponent<SLOne>();
            if (!one)
            {
                continue;
            }
            if (one.select.isOn && File.Exists(FullPath(one.path, one.name_i.text)))
            {
                sames += one.name_i.text + "\n";
            }
        }
        if (sames.Length > 0)
        {
            box1.ShowBox("将选中的存档保存至本地，已存在下列同名存档：\n" + sames, new UnityAction[] {
                delegate { box1.gameObject.SetActive(false); Do_SaveCollection(0); }, delegate { box1.gameObject.SetActive(false); Do_SaveCollection(1); },
                delegate { box1.gameObject.SetActive(false); Do_SaveCollection(2); }, null,
            });
        }
        else
        {
            MessageBox.ShowBox_s("将选中的存档保存至本地", delegate { box1.gameObject.SetActive(false); Do_SaveCollection(1); }, true);
        }
    }
    void Do_SaveCollection(int type)
    {
        var fails = new List<string>();
        SLFold[] fds = panel.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            if (fd.select.isOn)
            {
                SLOne[] sls = fd.GetComponentsInChildren<SLOne>(true);
                foreach (var sl in sls)
                {
                    if (sl.select.isOn)
                    {
                        if (!SaveOne(sl, type)) fails.Add(sl.path + "/" + sl.name_i.text);
                    }
                }
            }
        }
        for (int i = fds.Length > 0 ? fds[^1].transform.GetSiblingIndex() + 1 : 0; i < body.childCount; i++)
        {
            SLOne one = body.GetChild(i).GetComponent<SLOne>();
            if (!one)
            {
                continue;
            }
            if (one.select.isOn)
            {
                if (!SaveOne(one, type)) fails.Add(one.path + "/" + one.name_i.text);
            }
        }
        Refresh(true);
        if (fails.Count > 0)
        {
            MessageBox.ShowBox_s("下列" + fails.Count + "个文件保存失败：\r\n" + string.Join("\r\n", fails));
        }
    }
    bool SaveOne(SLOne sl, int type)
    {
        if (FullPath(sl.path, sl.name_i.text).Length > Options.save_path_len)
        {
            return false;
        }

        if (File.Exists(FullPath(sl.path, sl.name_i.text)))
        {
            switch (type)
            {
                case 0:
                    Save(sl.path, sl.name_i.text, false, sl.json);
                    break;
                case 1:
                    sl.name_i.text = RenameNum(sl.path, sl.name_i.text);
                    Save(sl.path, sl.name_i.text, false, sl.json);
                    break;
                case 2:
                    break;
            }
        }
        else
        {
            Save(sl.path, sl.name_i.text, false, sl.json);
        }
        return true;
    }
    string RenameNum(string path, string text, bool isFold = false)
    {
        int i = 1, l;
        if (text.EndsWith(')') && (l = text.LastIndexOf('(')) > 0)
        {
            string idx = text[(l + 1)..^1];
            if (int.TryParse(idx, out i))
            {
                i++;
                text = text[..l];
            }
            else
            {
                i = 1;
            }
        }
        while (isFold ? Directory.Exists(FullPath(path + "/" + text + "(" + i + ")")) :
            File.Exists(FullPath(path, text + "(" + i + ")")))
        {
            i++;
        }
        return text + "(" + i + ")";
    }

    public void DeleteSelect()
    {
        SLOne[] sls = body.GetComponentsInChildren<SLOne>(true);
        foreach (var sl in sls)
        {
            if (sl.select.isOn)
            {
                Delete(sl.path, sl.name_i.text);
                sl.transform.SetParent(panel);
                Destroy(sl.gameObject);
            }
        }
        SLFold[] fds = body.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            if (fd.select.isOn && fd.body.childCount == 0)
            {
                Delete(fd.GetText());
                Destroy(fd.gameObject);
            }
        }
    }

    public void SelectAll()
    {
        bool set = false;
        SLOne[] sls = body.GetComponentsInChildren<SLOne>(true);
        foreach (var sl in sls)
        {
            if (!sl.select.isOn)
            {
                set = true;
                break;
            }
        }
        SLFold[] fds = body.GetComponentsInChildren<SLFold>(true);
        if (set == false)
        {
            foreach (var fd in fds)
            {
                if (!fd.select.isOn)
                {
                    set = true;
                    break;
                }
            }
        }

        foreach (var sl in sls)
        {
            sl.select.isOn = set;
        }
        foreach (var fd in fds)
        {
            fd.select.isOn = set;
        }
    }

    public void Toggle(Type stat)
    {
        this.stat = stat;
        Refresh(false);
    }
    public void Toggle(int stat)
    {
        this.stat = (Type)stat;
        Refresh(false);
    }
    public void Refresh(bool full = true)
    {
        if (saveCollection.text == "写入存档")
        {
            full = true;
            saveCollection.text = "删除";
        }
        cleanAll(full);
        if (!full) return;
        string base_path = Options.save_path;
        if (Directory.Exists(base_path))
        {
            string[] full_paths = Directory.GetDirectories(base_path);
            foreach (var full_path in full_paths)
            {
                string single_path = Path.GetFileName(full_path);
                if (SLOne.CheckName(single_path) != single_path)
                {
                    single_path = SLOne.CheckName(single_path);
                    if (Directory.Exists(FullPath(single_path)))
                    {
                        single_path = RenameNum("", single_path, true);
                    }
                    Options.CopyDirectory(full_path, FullPath(single_path));
                    Directory.Delete(full_path, true);
                }
                var fold = Instantiate(slFold, body, false);
                fold.SetText(single_path);
                fold.select.transform.parent.gameObject.SetActive(stat == Type.Multi);
                loadFiles(fold, FullPath(single_path));
                fold.gameObject.SetActive(true);
            }
            loadFiles(base_path);
        }
    }

    void loadFiles(Transform body, string fold, string[] files, string[] jsons = null)
    {
        for (var i = 0; i < files.Length; i++)
        {
            var full_name = files[i];
            if (full_name.EndsWith(".json"))
            {
                string single_name = Path.GetFileNameWithoutExtension(full_name);
                if (SLOne.CheckName(single_name) != single_name)
                {
                    single_name = SLOne.CheckName(single_name);
                    if (File.Exists(FullPath(fold, single_name)))
                    {
                        single_name = RenameNum(fold, single_name);
                    }
                    File.Move(full_name, FullPath(fold, single_name));
                }
                var sl = Instantiate(slTemplate, body, false);
                sl.path = fold;
                sl.name_i.text = single_name;
                sl.old_name = sl.name_i.text;
                sl.button_t.text = stat == Type.Save ? "保存" : "读取";
                sl.select.gameObject.SetActive(stat == Type.Multi);
                if (jsons != null)
                {
                    sl.json = jsons[i];
                    sl.name_i.interactable = false;
                    sl.delete.SetActive(stat != Type.Multi);
                }
                else
                {
                    sl.json = null;
                }
                sl.gameObject.SetActive(true);
            }
        }
    }
    void loadFiles(Transform body, string fold, string fullpath)
    {
        string[] files = Directory.GetFiles(fullpath);
        loadFiles(body, fold, files);
    }
    void loadFiles(SLFold fold, string fullpath)
    {
        loadFiles(fold.body, fold.GetText(), fullpath);
    }
    void loadFiles(string fullpath)
    {
        loadFiles(body, "", fullpath);
    }

    void cleanAll(bool full = true)
    {
        SLFold[] fds = panel.GetComponentsInChildren<SLFold>(true);
        foreach (var fd in fds)
        {
            if (full)
            {
                fd.gameObject.SetActive(false);
                Destroy(fd.gameObject);
            }
            else
            {
                fd.select.transform.parent.gameObject.SetActive(stat == Type.Multi);
            }
        }
        SLOne[] sls = panel.GetComponentsInChildren<SLOne>(true);
        foreach (var sl in sls)
        {
            if (sl.delete)
            {
                if (full)
                {
                    sl.gameObject.SetActive(false);
                    Destroy(sl.gameObject);
                }
                else
                {
                    sl.button_t.text = stat == Type.Save ? "保存" : "读取";
                    sl.select.gameObject.SetActive(stat == Type.Multi);
                }
            }
            else
            {
                sl.gameObject.SetActive((stat == Type.Save && sl.name_i && sl.path_i) || (stat == Type.Load && !sl.name_i) || (stat == Type.Multi && !sl.path_i));
            }
        }
    }

    public void Save(string path, string name, bool isConfig = false, string json = null)
    {
        if (FullPath(path, name).Length > Options.save_path_len)
        {
            MessageBox.ShowBox_s("文件完整路径过长");
            return;
        }

        string filePath = FullPath(path, null, isConfig);
        Directory.CreateDirectory(filePath);
        filePath += "/" + name + (name.EndsWith(".json") ? "" : ".json");

        string text = json;
        if (json == null)
        {
            if (isConfig)
            {
                text = GetComponent<Options>().SaveOptions();
            }
            else
            {
                calc.CalcSL();
                text = calc.json;
            }
        }

        bool need_refresh = false;
        if (!File.Exists(filePath) && !isConfig && json == null)
        {
            need_refresh = true;
        }

        File.WriteAllText(filePath, text);
        if (File.Exists(filePath) && File.ReadAllText(filePath) == text)
        {
            Debug.Log("保存成功: " + filePath);
        }
        else
        {
            Debug.LogWarning("保存失败: " + filePath);
        }
        if (need_refresh) Refresh();
    }

    bool first = true;
    public void Load(string path, string name, bool isConfig = false)
    {
        string filePath = FullPath(path, name, isConfig);
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
        if (isConfig) first = false;
    }

    public bool Rename(string path, string oldname, string newname)
    {
        if (FullPath(path, newname).Length > Options.save_path_len)
        {
            Debug.LogWarning("文件完整路径过长");
            return false;
        }

        string filePath = FullPath(path);
        Directory.CreateDirectory(filePath);
        filePath += "/";
        string _old = filePath + oldname + (oldname.EndsWith(".json") ? "" : ".json");
        string _new = filePath + newname + (newname.EndsWith(".json") ? "" : ".json");
        if (File.Exists(_old))
        {
            File.Move(_old, _new);
        }
        if (File.Exists(_new))
        {
            Debug.Log("重命名成功: " + _new);
            return true;
        }
        else
        {
            Debug.LogWarning("重命名失败: " + _new);
            return false;
        }
    }

    public void SaveConfig()
    {
        Save(string.Empty, "config.json", true);
    }
    public void LoadConfig()
    {
        Load("", "config.json", true);
    }

    public void Delete(string path, string name)
    {
        string filePath = FullPath(path, name);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    public void Delete(string path)
    {
        if (path.Length > 0)
        {
            string filePath = FullPath(path);
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
        }
    }

    public bool CheckFile(string path, string name)
    {
        string filePath = FullPath(path, name);
        return File.Exists(filePath);
    }

    public string GetJson(string path, string name)
    {
        string filePath = FullPath(path, name);
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
    string FullPath(string path, string name = null, bool isConfig = false)
    {
        string full_path = (isConfig ? configPath : Options.save_path) + (path.StartsWith('/') ? "" : "/") + path.TrimEnd(new char[] { '/', '\\' });
        if (name != null) full_path += "/" + name + (name.EndsWith(".json") ? "" : ".json");
        return full_path;
    }

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
