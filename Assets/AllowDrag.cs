using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AllowDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] bool m_AllowDragH = false;
    [SerializeField] bool m_AllowDragV = true;
    [SerializeField] bool m_AllowDragInactive = true;

    Vector2 startDragPosition;
    bool firstDrag = true, allowDrag = false;
    GameObject dragObj;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        startDragPosition = eventData.position;
        Transform parent = transform;
        while (parent = parent.parent)
        {
            if (ExecuteEvents.Execute(parent.gameObject, eventData, ExecuteEvents.beginDragHandler))
            {
                dragObj = parent.gameObject;
                break;
            }
        }
        if (m_AllowDragInactive && !Active())
        {
            firstDrag = false;
            allowDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if (firstDrag)
        {
            Vector2 diff = eventData.position - startDragPosition;
            if (diff.x != 0 || diff.y != 0)
            {
                bool allow = m_AllowDragV && (diff.x == 0 || Math.Abs(diff.y / diff.x) > 1);
                allow |= m_AllowDragH && (diff.y == 0 || Math.Abs(diff.x / diff.y) > 1);
                //Debug.Log("allow drag: " + allow + ", diff: " + diff + ", m_AllowDragV: " + m_AllowDragV + ", m_AllowDragH: " +     m_AllowDragH + ", diff.x: " + diff.x + ", diff.y: " + diff.y);
                firstDrag = false;
                allowDrag = allow;
            }
        }
        if (allowDrag && dragObj)
        {
            ExecuteEvents.Execute(dragObj, eventData, ExecuteEvents.dragHandler);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if (dragObj)
        {
            ExecuteEvents.Execute(dragObj, eventData, ExecuteEvents.endDragHandler);
            dragObj = null;
        }
        allowDrag = false;
        firstDrag = true;
    }
    bool Active()
    {
        Component c;
        if (c = GetComponent<Scrollbar>())
            return ((Scrollbar)c).IsInteractable();
        else if (c = GetComponent<Mod_InputField>())
            return ((Mod_InputField)c).IsInteractable() && ((Mod_InputField)c).isFocused;
        return false;
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
