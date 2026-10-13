using System;
using System.Reflection;
using UnityEngine;

public class GameTrigger : MonoBehaviour
{
    [SerializeField] private Component source;

    [SerializeField] private string eventName;

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
                $"GameTrigger: Invalid event {eventName}",
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
        string key = string.IsNullOrEmpty(eventName)
            ? $"{eventName}"
            : eventName;

        Debug.Log($"Triggered: {eventName}");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEvent(key);
            Debug.Log($"GameTrigger: {key}");
        }

                    /*if (GameManager.Instance != null)
            {
                GameManager.Instance.GameEvent("startGame");
                Debug.Log("gameStart!!!"); 
            } else
            {
                Debug.Log("Title Screen: No game manager available, unable to start game");
            }*/
    }
}