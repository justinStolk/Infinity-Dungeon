using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventType { ON_ENEMY_TURN_STARTED, ON_PLAYER_TURN_STARTED, ON_PARTY_DEFEATED }
public static class EventSystem
{
    private static Dictionary<EventType, Action> actions = new();
    public static void Subscribe(EventType eventType, Action actionToSubscribe)
    {
        if (!actions.ContainsKey(eventType))
        {
            actions.Add(eventType, null);
        }
        actions[eventType] += actionToSubscribe;
    }
    public static void Unsubscribe(EventType eventType, Action actionToUnsubscribe)
    {
        if (actions.ContainsKey(eventType))
        {
            actions[eventType] -= actionToUnsubscribe;
        }
    }
    public static void CallEvent(EventType eventType)
    {
        actions[eventType]?.Invoke();
    }


}
