using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public delegate void KeyEventCallback(KeyCode key, KeyEventType type);
    public delegate void MouseEventCallback(MouseEventData data);

    public enum KeyEventType
    {
        Active,
        Down,
        Up
    }

    public class MouseEventData
    {
    }

    public class Input
    {
        public readonly MouseInputPoller mouseInputPoller;
        public readonly KeyInputPoller keyInputPoller;

        public event MouseEventCallback MouseEvent;

        readonly Dictionary<(KeyCode, KeyEventType), List<KeyEventCallback>> keyListeners
            = new Dictionary<(KeyCode, KeyEventType), List<KeyEventCallback>>();

        public Input(MouseInputPoller mousePoller, KeyInputPoller keyPoller)
        {
            this.mouseInputPoller = mousePoller;
            this.keyInputPoller = keyPoller;
        }

        /// <summary>
        /// A frame has passed, and this should recheck input for events.
        /// </summary>
        public void Update()
        {
            // Send updates to all the key listeners.
            var keyListenerEnumerator = keyListeners.GetEnumerator();
            while (keyListenerEnumerator.MoveNext())
            {
                var kvp = keyListenerEnumerator.Current;
                (var key, var type) = kvp.Key;

                if (!CheckKeyEvent(key, type))
                    continue;

                var listeners = kvp.Value;
                listeners.ForEach((c) => c(key, type));
            }

            // TODO: Mouse events.
        }

        public void ListenToKeyEvent(KeyCode key, KeyEventType type, KeyEventCallback callback)
        {
            keyListeners.TryGetValue((key, type), out List<KeyEventCallback> listeners);
            if (listeners == null)
            {
                listeners = keyListeners[(key, type)] = new List<KeyEventCallback>();
            }

            listeners.Add(callback);
        }

        public void RemoveKeyEventListener(KeyCode key, KeyEventType type, KeyEventCallback callback)
        {
            keyListeners.TryGetValue((key, type), out List<KeyEventCallback> listeners);
            if (keyListeners == null)
                throw new Exception("Listener not found.");

            if (!keyListeners.Remove((key, type)))
                throw new Exception("Listener not found.");
        }

        bool CheckKeyEvent(KeyCode key, KeyEventType type)
        {
            switch (type)
            {
                case KeyEventType.Active:
                    return keyInputPoller.GetKey(key);
                case KeyEventType.Down:
                    return keyInputPoller.GetKeyDown(key);
                case KeyEventType.Up:
                    return keyInputPoller.GetKeyUp(key);
                default:
                    throw new Exception("KeyEventType not supported: " + type.ToString());
            }
        }
    }
}
