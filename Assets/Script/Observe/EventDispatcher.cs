using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChangeType
{
    Character,
    Cloth
}
public interface IEventParam { }
[System.Serializable]
public class EventDefine
{
    // ─────────────────────────────────────────
    //  GAME STATE EVENTS
    // ─────────────────────────────────────────
    public struct OnStartGame : IEventParam { }
    public struct OnWin : IEventParam { }
    public struct OnDraw : IEventParam { }
    public struct OnLose : IEventParam { }
    public struct OnRevive : IEventParam { }
    public struct OnPause : IEventParam { public bool IsPause; }
    public struct OnNextRound : IEventParam { }
    public struct OnFinishTutorial : IEventParam { }
    public struct OnChangeScene : IEventParam { public string SceneName; }

    // ─────────────────────────────────────────
    //  PLAYER & BOT EVENTS
    // ─────────────────────────────────────────
    public struct OnPlayerDead : IEventParam { }
    public struct OnBotDead : IEventParam { public GameObject GO; public bool IsEnemy; }
    public struct OnAlertAllEnemy : IEventParam { }

    // Character selection & customization

    public struct OnChangeCharacter : IEventParam
    {
        public int CharacterIdx;
        public int CharacterClothIdx;
        public ChangeType changeType;
    }

    public struct OnSelectCharacter : IEventParam
    {
        public int CharacterIdx;
        public int CharacterClothIdx;
        public ChangeType changeType;
    }

    // ─────────────────────────────────────────
    //  UI & GAMEPLAY EVENTS
    // ─────────────────────────────────────────
    public struct OnUpdateCurrency : IEventParam { }
}


public static class GameEvent
{
    public static Action OnJump;
    public static void Trigger(Action action)
    {
        action?.Invoke();
    }

    public static void Trigger<T>(Action<T> action, T param)
    {
        action?.Invoke(param);
    }
}

public static class EventDispatcher
{
    private static readonly Dictionary<Type, Delegate> eventDictionary = new();

    public static void AddListener<T>(Action<T> listener) where T : struct, IEventParam
    {
        var eventType = typeof(T);
        eventDictionary[eventType] = eventDictionary.TryGetValue(eventType, out var existing)
        ? Delegate.Combine(existing, listener) : listener;

        //Debug.Log($"[EventDispatcher] Added listener for event: {eventType}");
    }

    public static void RemoveListener<T>(Action<T> listener) where T : struct, IEventParam
    {
        var eventType = typeof(T);
        if (!eventDictionary.TryGetValue(eventType, out var existing)) return;

        var updated = Delegate.Remove(existing, listener);
        if (updated is null) eventDictionary.Remove(eventType);
        else eventDictionary[eventType] = updated;

        //Debug.Log($"[EventDispatcher] Removed listener for event: {eventType}");
    }

    public static void AddListener(Type eventType, Action listener)
    {
        if (!typeof(IEventParam).IsAssignableFrom(eventType))
            Debug.LogWarning($"Type {eventType} must implement IEventParam.");

        eventDictionary[eventType] = eventDictionary.TryGetValue(eventType, out var existing)
            ? Delegate.Combine(existing, listener)
            : listener;

        //Debug.Log($"[EventDispatcher] Added listener for event: {eventType}");
    }

    public static void RemoveListener(Type eventType, Action listener)
    {
        if (!eventDictionary.TryGetValue(eventType, out var existing)) return;

        var updated = Delegate.Remove(existing, listener);
        if (updated is null) eventDictionary.Remove(eventType);
        else eventDictionary[eventType] = updated;

        //Debug.Log($"[EventDispatcher] Removed listener for event: {eventType}");
    }

    public static void Dispatch<T>(T param) where T : struct, IEventParam
    {
        if (eventDictionary.TryGetValue(typeof(T), out var action) && action is Action<T> typedAction)
        {
            //Debug.Log($"[EventDispatcher] Dispatching event: {typeof(T)} with param: {param}");
            typedAction(param);
        }
    }

    public static void Dispatch<T>() where T : struct, IEventParam
    {
        if (eventDictionary.TryGetValue(typeof(T), out var action))
        {
            //Debug.Log($"[EventDispatcher] Dispatching event: {typeof(T)} with default param.");
            if (action is Action eventAction) eventAction();
            else if (action is Action<T> typedAction) typedAction(default);
        }
    }

    public static void ClearAll()
    {
        // Debug.Log($"[EventDispatcher] Cleared all listeners.");
        eventDictionary.Clear();
    }
}





