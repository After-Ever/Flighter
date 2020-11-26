using Flighter;
using Flighter.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class ImageHandle : IImageHandle
    {
        public Size size => sprite.rect.size.ToPoint().ToSize();

        public readonly Sprite sprite;

        public ImageHandle(Sprite sprite)
        {
            this.sprite = sprite ?? throw new ArgumentNullException();
        }
    }
}
