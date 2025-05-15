using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessageBox : MonoBehaviour
{
    public static MessageBox box;
    public static bool allow_show_s = false;
    public TMP_Text message, message_hide;
    public Button[] buttons;
    bool active = false;
    LayoutElement layout;
    RectTransform parent;

    public static void ShowBox_s(string msg, UnityAction f = null, bool canCancel = false, UnityAction c = null)
    {
        if (allow_show_s) box.ShowBox(msg, f, canCancel, c);
    }
    public static void CleanAction_s()
    {
        box.CleanAction();
    }
    public void ShowBox(string msg, UnityAction f = null, bool canCancel = false, UnityAction c = null)
    {
        if (canCancel)
            ShowBox(msg, new UnityAction[] { f, c });
        else
            ShowBox(msg, new UnityAction[] { f });
    }
    public void ShowBox(string msg, UnityAction[] f)
    {
        if (!layout || !parent)
        {
            layout = message.GetComponent<LayoutElement>();
            parent = message.transform.parent.parent.parent.GetComponent<RectTransform>();
        }
        if (active)
        {
            gameObject.SetActive(false);
            MessageBox newbox = Instantiate(this, transform.parent, false);
            newbox.gameObject.SetActive(false);
            newbox.ShowBox(msg, f);
            return;
        }
        message_hide.text = msg;
        message.text = msg;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(i < f.Length);
            if (i < f.Length)
            {
                if (f[i] != null)
                    buttons[i].onClick.AddListener(f[i]);
                buttons[i].onClick.AddListener(CleanAction);
            }
        }
        active = true;
        gameObject.SetActive(true);
        layout.preferredHeight = message_hide.preferredHeight;
    }
    public void CleanAction()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        gameObject.SetActive(false);
        active = false;
        if (gameObject != box.gameObject && buttons.Length == 2)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (!box)
        {
            box = this;
            layout = message.GetComponent<LayoutElement>();
            parent = message.transform.parent.parent.parent.GetComponent<RectTransform>();
            allow_show_s = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (layout.minHeight != parent.sizeDelta.y)
                layout.minHeight = parent.sizeDelta.y;
            if (layout.preferredHeight != message_hide.preferredHeight)
                layout.preferredHeight = message_hide.preferredHeight;
        }
    }
}
