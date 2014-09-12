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

        public string Name { get; set; }
        
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

        public override string ToString()
        {
            return string.Format("\"{0}\" ({1})", Name, listeners.Count);
        }
    }

    public class EventSystem
    {
        public IList<Event> Events { get; set; }
        
        public EventSystem()
        {
            Events = new List<Event>();
        }

        public Event getEvent(String name)
        {
            Event result = Events.SingleOrDefault(e => e.Name == name);

            if (result == null)
            {
                result = new Event() { Name = name };
                Events.Add(result);
            }

            return result;
        }

    }
}
