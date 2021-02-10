using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    public class InputEvent
    {
        HashSet<KeyEventFilter> absorbedKeyEvents = new HashSet<KeyEventFilter>();
        HashSet<MouseEventFilter> absorbedMouseEvents = new HashSet<MouseEventFilter>();

        public readonly IInputPoller inputPoller;

        public bool FullyAbsorbed { get; private set; }

        public InputEvent(IInputPoller inputPoller)
        {
            this.inputPoller = inputPoller;
        }

        public bool CheckKeyEvent(KeyEventFilter keyEvent, bool absorb = true)
        {
            if (FullyAbsorbed || absorbedKeyEvents.Contains(keyEvent))
                return false;

            if (inputPoller.CheckForKeyEvent(keyEvent))
            {
                if (absorb)
                    absorbedKeyEvents.Add(keyEvent);
                return true;
            }

            return false;
        }

        public bool CheckMouseEvent(MouseEventFilter mouseEvent, bool absorb = true)
        {

            if (FullyAbsorbed || absorbedMouseEvents.Contains(mouseEvent))
                return false;

            if (inputPoller.CheckForMouseEvent(mouseEvent))
            {
                if (absorb)
                    absorbedMouseEvents.Add(mouseEvent);
                return true;
            }

            return false;
        }

        public void SetFullyAbsorbed()
        {
            FullyAbsorbed = true;
        }
    }
}
