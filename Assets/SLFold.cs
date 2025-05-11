using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLFold : MonoBehaviour
{
    public TMPro.TMP_Text slFold;
    public Transform body;
    public SaveLoad sl;
    public MessageBox mBox;
    public Mod_InputField path;


    public string GetText()
    {
        return slFold.text;
    }
    public void SetText(string text)
    {
        slFold.text = text;
    }
    public void Delete()
    {
        mBox.ShowBox("删除文件夹\r\n" + slFold.text + "\r\n文件夹内所有存档和其他文件都将删除", Del, true);
    }
    public void UpdatePath(active active)
    {
        path.text = active.body.activeSelf ? slFold.text : "";
    }
    void Del()
    {
        sl.Delete(slFold.text);
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
