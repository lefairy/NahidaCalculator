using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine
{
    public class active : MonoBehaviour
    {
        //Transform arrow;
        public GameObject body;
        Transform fold;

        public void toggle_active()
        {
            toggle_active(!body.activeSelf);
        }
        public void toggle_active(bool active)
        {
            body.SetActive(active);
            //arrow.Rotate(0, 0, body.activeSelf ? -90 : 90);
            //arrow.SetPositionAndRotation(arrow.position, Quaternion.Euler(0, 0, body.activeSelf ? 0 : 90));
            Refresh(transform);
        }

        // Start is called before the first frame update
        void Start()
        {
            fold = transform.parent;
            if (!body) body = fold.GetChild(1).gameObject;
            //arrow = transform.GetChild(0);
            //arrow.SetPositionAndRotation(arrow.position, Quaternion.Euler(0, 0, body.activeSelf ? 0 : 90));
            //if (body.activeSelf)
            //    arrow.Rotate(0, 0, -90);
        }

        public static void Refresh(Transform obj)
        {
            //Transform parent = obj;
            //while (parent = parent.parent)
            //    //LayoutRebuilder.MarkLayoutForRebuild((RectTransform)parent);
            //    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
