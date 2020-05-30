using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace JamesOR.Edna
{
    [System.Serializable]
    public class Event : UnityEvent<EventArgs> { }

    public class EventManager
    {
        #region Singleton
        private static EventManager m_instance;
        public static EventManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new EventManager();
                }

                return m_instance;
            }
        }
        #endregion

        Dictionary<string, Event> m_eventDictionary = new Dictionary<string, Event>();

        public static void StartListening(string eventName, UnityAction<EventArgs> listener)
        {
            if (Instance != null)
            {
                Event thisEvent = null;
                if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEvent))
                {
                    thisEvent.AddListener(listener);
                }
                else
                {
                    thisEvent = new Event();
                    thisEvent.AddListener(listener);
                    Instance.m_eventDictionary.Add(eventName, thisEvent);
                }
            }
        }

        public static void StopListening(string eventName, UnityAction<EventArgs> listener)
        {
            if (Instance != null)
            {
                Event thisEvent = null;
                if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEvent))
                {
                    thisEvent.RemoveListener(listener);
                }
            }
        }

        public static void TriggerEvent(string eventName, EventArgs arg = null)
        {
            if (Instance != null)
            {
                Event thisEvent = null;
                if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEvent))
                {
                    thisEvent.Invoke(arg);
                }
            }
        }
    }
}
