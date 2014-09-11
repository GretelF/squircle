using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public delegate void StringEvent(String data);

    public class Event
    {
        private IList<StringEvent> listeners;
        
        public Event()
        {
            listeners = new List<StringEvent>();
        }

        public void addListener(StringEvent listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void trigger(String data)
        {
            foreach (var listener in listeners)
            {
                listener(data);
            }
        }
    }

    public class EventSystem
    {
        public IDictionary<String, Event> Events { get; set; }
        
        public EventSystem()
        {
            Events = new Dictionary<String, Event>();
        }

        public Event getEvent(String name)
        {
            Event e;

            if (!Events.TryGetValue(name, out e))
            {
                e = new Event();
                Events.Add(name, e);
            }

            return e;
        }

    }
}
