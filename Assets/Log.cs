using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Log : MonoBehaviour
{
    static Mod_InputField input;
    static public void Add(string message)
    {
        if (!input) return;
        input.text += message + "\r\n";
    }
    static public void Clean()
    {
        if (!input) return;
        input.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<Mod_InputField>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
