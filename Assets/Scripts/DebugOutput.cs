using UnityEngine;
using UnityEditor;
using TMPro;

public class DebugOutput : MonoBehaviour
{
    public static DebugOutput main;

    void Awake()
    {
        main = this;
    }

    public static void Log(string text)
    {
        if (main)
            main._log(text);
    }

    public void _log(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}