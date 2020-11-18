using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public enum KeyEventType
    {
        /// <summary>
        /// The key is being held.
        /// </summary>
        Active,
        /// <summary>
        /// The key was pressed this frame.
        /// </summary>
        Down,
        /// <summary>
        /// The key was released this frame.
        /// </summary>
        Up
    }

    public struct KeyEventFilter
    {
        public KeyCode key;
        public KeyEventType type;

        public KeyEventFilter(KeyEventType type, KeyCode key)
        {
            this.key = key;
            this.type = type;
        }
    }
}
