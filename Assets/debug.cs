using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class debug : MonoBehaviour
{
    public void log(TMP_Text component)
    {
        Debug.Log(component.text);
    }
    public void log(Mod_InputField component)
    {
        Debug.Log(component.text);
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
