using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModTextUGUI : TextMeshProUGUI
{
    LayoutElement parent;
    int can_refresh = 0;
	protected override Vector2 CalculatePreferredValues(ref float fontSize, Vector2 marginSize, bool isTextAutoSizingEnabled, bool isWordWrappingEnabled){
        Vector2 ret = base.CalculatePreferredValues(ref fontSize, marginSize, isTextAutoSizingEnabled, isWordWrappingEnabled);
        can_refresh++;
        if (parent != null && can_refresh == 2 && ret.y > 0) {
            //Debug.LogWarning(can_refresh + " (" + ret.x + ", " + ret.y + ") " + m_maxTextAscender + "-" + m_ElementDescender);
            parent.minHeight = ret.y;
        }
        return ret;
    }
    protected override void Start(){
        base.Start();
        parent = transform.parent.parent.GetComponent<LayoutElement>();
        if (parent == null) Debug.LogError("No LayoutElement");
    }
    protected void Update(){
        can_refresh = 0;
    }
}
