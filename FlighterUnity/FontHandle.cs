using Flighter;
using Flighter.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class FontHandle : IFontHandle
    {
        public readonly Font font;

        public FontHandle(Font font)
        {
            this.font = font;
        }

        public Size PreferredSize(string text, TextStyle style, BoxConstraints constraints)
        {
            var textGenerator = new TextGenerator();
            var textGeneratorSettings = new TextGenerationSettings
            {
                font = font,
                fontSize = style.size,
                horizontalOverflow = (float.IsInfinity(constraints.maxWidth)
                                   || !style.wrapLines)
                    ? HorizontalWrapMode.Overflow
                    : HorizontalWrapMode.Wrap,
                verticalOverflow = (float.IsInfinity(constraints.maxHeight) 
                                 || style.textOverflow == TextOverflow.Overflow) 
                    ? VerticalWrapMode.Overflow
                    : VerticalWrapMode.Truncate,
                fontStyle = style.fontStyle.ToUnity(),
                textAnchor = style.textAlign.ToUnity(),
                lineSpacing = style.lineSpacing,
                generationExtents = constraints.MaxSize.ToVector2().ToUnity(),
                //generateOutOfBounds = constraints.IsUnconstrained,
                updateBounds = true,
                scaleFactor = 1,
                richText = true
            };

            textGenerator.Populate(text, textGeneratorSettings);

            return new Size(textGenerator.rectExtents.width, textGenerator.rectExtents.height);
        }
    }
}
