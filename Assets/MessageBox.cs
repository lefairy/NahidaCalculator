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
    public Button enter, cancel;
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
            newbox.ShowBox(msg, f, canCancel, c);
            return;
        }
        message_hide.text = msg;
        message.text = msg;
        cancel.gameObject.SetActive(canCancel);
        if (canCancel && c != null)
            cancel.onClick.AddListener(c);
        cancel.onClick.AddListener(CleanAction);

        if (f != null)
            enter.onClick.AddListener(f);
        enter.onClick.AddListener(CleanAction);
        active = true;
        gameObject.SetActive(true);
        message_hide.gameObject.SetActive(true);
        layout.preferredHeight = message_hide.preferredHeight;
        message_hide.gameObject.SetActive(false);
    }
    public void CleanAction()
    {
        enter.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
        active = false;
        if (gameObject != box.gameObject)
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
        if (gameObject.activeSelf && layout.minHeight != parent.sizeDelta.y)
        {
            layout.minHeight = parent.sizeDelta.y;
        }
    }
}
