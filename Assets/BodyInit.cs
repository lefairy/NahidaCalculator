using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyInit : MonoBehaviour
{
    [SerializeField] active[] folds;
    bool first = true;

    [SerializeField] Canvas canvas;
    [SerializeField] VerticalLayoutGroup[] blank;
    [SerializeField] MessageBox box;


    // Start is called before the first frame update
    void Start()
    {
        var opt = GetComponent<Options>();
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        opt.SetSaveDef(Application.persistentDataPath + "/Save");
#else
        opt.SetSaveDef(Application.dataPath + "/Save");
#endif
        GetComponent<SaveLoad>().Load("config.json", true);
        box.Start();

        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned.");
            return;
        }
        //SortUIElementsBreadthFirst(canvas.transform);
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidStatusBar.dimmed = false;
            AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
            int height = AndroidStatusBar.GetStatusBarHeight();
            foreach (var b in blank)
            {
                var layout = b.transform.GetChild(0).GetComponent<LayoutElement>();
                if (layout)
                {
                    layout.minHeight = height / canvas.transform.localScale.y - b.spacing;
                }
            }
            //mbox.ShowBox(string.Format("StatusBarHeight{0}, scale{1}, rect{2}, canvasrect{3}", height, canvas.scaleFactor, transform.localScale.y, canvas.transform.localScale.y));
        }
#endif
    }

    void SortUIElementsBreadthFirst(Transform root)
    {
        Queue<Transform> queue = new Queue<Transform>();
        List<Transform> sortedList = new List<Transform>();

        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            sortedList.Add(current);

            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }

        for (int i = 0; i < sortedList.Count; i++)
        {
            sortedList[i].SetSiblingIndex(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            first = false;
            for (int i = 0; i < folds.Length; i++)
            {
                folds[i].toggle_active(false);
            }
        }
        //SortUIElementsBreadthFirst(canvas.transform);

    }
}
