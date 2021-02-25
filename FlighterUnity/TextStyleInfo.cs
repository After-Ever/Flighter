using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using Flighter.Core;

using Color = UnityEngine.Color;
using FontStyle = Flighter.Core.FontStyle;

namespace FlighterUnity
{
    [CreateAssetMenu(fileName = "TextStyle", menuName = "Text Style Info")]
    public class TextStyleInfo : ScriptableObject
    {
        public Font font;
        public int size;
        public float lineSpacing;
        public TextAlign textAlign;
        public FontStyle fontStyle;
        public bool wrapLines;
        public TextOverflow textOverflow;
        public Color color;

        public TextStyle ToTextStyle()
            => new TextStyle
            {
                font = new FontHandle(font),
                size = size,
                lineSpacing = lineSpacing,
                textAlign = textAlign,
                fontStyle = fontStyle,
                wrapLines = wrapLines,
                textOverflow = textOverflow,
                color = color.ToFlighter()
            };
    }
}
