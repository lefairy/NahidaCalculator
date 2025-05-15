using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLFold : MonoBehaviour
{
    public TMP_Text slFold;
    public Transform body;
    public Toggle select;
    public GameObject delete;
    public SaveLoad sl;
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
        MessageBox.ShowBox_s("删除文件夹\r\n" + slFold.text + "\r\n文件夹内所有存档和其他文件都将删除", delegate { sl.Delete(slFold.text); Destroy(gameObject); }, true);
    }
    public void UpdatePath(active active)
    {
        path.text = active.body.activeSelf ? slFold.text : "";
    }

    bool allow_toggle = true;
    public void ToggleAll()
    {
        if (!allow_toggle) return;
        SLOne[] sls = body.GetComponentsInChildren<SLOne>(true);
        foreach (var sl in sls)
        {
            sl.select.isOn = select.isOn;
        }
    }
    public void Select()
    {
        allow_toggle = false;
        select.isOn = true;
        allow_toggle = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        select.onValueChanged.AddListener(delegate { ToggleAll(); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
