using UnityEngine;
using UnityEditor;

public static class UEditor
{
    public static Color buttonColor = new Color(0.686f, 0.705f, 0.937f);

    public static bool UToggleButton(string text = null, string information = null, params GUILayoutOption[] options)
    {
        bool output = false;

        if (GUILayout.Button(new GUIContent(text, information), options))
            output = true;

        return output;
    }

    public static bool UToggleButton(bool target, string text = null, string information = null, params GUILayoutOption[] options)
    {
        Color old = GUI.color;

        if (target)
            GUI.color = buttonColor;

        if (GUILayout.Button(new GUIContent(text, information), options))
            target = !target;

        GUI.color = old;
        return target;
    }
}
