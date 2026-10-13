using System;
using System.Reflection;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private Component source;

    [SerializeField, HideInInspector]
    private string eventName;

    [SerializeField] private string eventKeyOverride;

    [SerializeField] private AudioCue soundOverride;

    private EventInfo boundEvent;
    private Action listener;

    private void OnEnable()
    {
        if (source == null || string.IsNullOrEmpty(eventName))
            return;

        boundEvent = source.GetType().GetEvent(
            eventName,
            BindingFlags.Public | BindingFlags.Instance
        );

        if (boundEvent == null ||
            boundEvent.EventHandlerType != typeof(Action))
        {
            Debug.LogWarning(
                $"AudioTrigger: Invalid event {eventName}",
                this
            );

            boundEvent = null;
            return;
        }

        listener = OnEventTriggered;

        boundEvent.AddEventHandler(source, listener);
    }

    private void OnDisable()
    {
        if (boundEvent != null &&
            source != null &&
            listener != null)
        {
            boundEvent.RemoveEventHandler(source, listener);
        }

        boundEvent = null;
        listener = null;
    }

    private void OnEventTriggered()
    {
        string key = string.IsNullOrEmpty(eventKeyOverride)
            ? $"{eventName}"
            : eventKeyOverride;

        //Debug.Log($"Triggered: {source.GetType().Name}.{eventName}");
        Vector3 pos = gameObject.GetComponentInParent<Transform>().position;
        if (GameObject.Find("MantaRay w_ Swim + Sprite") != null)
        {
            Vector3 ppos = GameObject.Find("MantaRay w_ Swim + Sprite").GetComponent<Transform>().position;
            Vector3 rel = pos - ppos;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    key,
                    rel,
                    soundOverride
                );
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    key,
                    pos,
                    soundOverride
                );
            }
        }
    }
}