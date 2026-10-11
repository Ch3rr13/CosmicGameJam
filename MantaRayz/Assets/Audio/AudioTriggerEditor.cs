// Place in an Editor folder.

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioTrigger))]
public class AudioTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        var source = serializedObject
            .FindProperty("source")
            .objectReferenceValue as Component;

        var eventProperty = serializedObject
            .FindProperty("eventName");

        if (source != null)
        {
            string[] eventNames = source.GetType()
                .GetEvents(BindingFlags.Public | BindingFlags.Instance)
                .Where(e => e.EventHandlerType == typeof(Action))
                .Select(e => e.Name)
                .ToArray();

            if (eventNames.Length > 0)
            {
                int index = Array.IndexOf(
                    eventNames,
                    eventProperty.stringValue
                );

                int selected = EditorGUILayout.Popup(
                    "Listen For",
                    index,
                    eventNames
                );

                if (selected >= 0)
                    eventProperty.stringValue = eventNames[selected];
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No compatible events found.",
                    MessageType.Info
                );
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}