using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Flighter.Core;
using UnityEngine;
using Color = Flighter.Core.Color;
using Text = UnityEngine.UI.Text;
using Image = UnityEngine.UI.Image;

namespace FlighterUnity
{
    public class UnityTextComponent : TextComponent, IUnityFlighterComponent
    {
        string data;
        TextStyle style;
        TextAlign alignment;
        TextOverflow overflow;

        Text text;

        public override string Data
        {
            get => data;
            set
            {
                data = value;
                if (text != null)
                    text.text = data;
            }
        }
        public override TextStyle Style
        {
            get => style;
            set
            {
                style = value;
                if (text != null)
                {
                    // TODO: Set all the style stuff.
                }
            }
        }
        public override TextAlign Alignment
        {
            get => alignment;
            set
            {
                alignment = value;
                if (text != null)
                {
                    switch (alignment)
                    {
                        case TextAlign.TopLeft:
                            text.alignment = TextAnchor.UpperLeft;
                            break;

                        case TextAlign.TopCenter:
                            text.alignment = TextAnchor.UpperCenter;
                            break;

                        case TextAlign.TopRight:
                            text.alignment = TextAnchor.UpperRight;
                            break;

                        case TextAlign.MiddleLeft:
                            text.alignment = TextAnchor.MiddleLeft;
                            break;

                        case TextAlign.MiddleCenter:
                            text.alignment = TextAnchor.MiddleCenter;
                            break;

                        case TextAlign.MiddleRight:
                            text.alignment = TextAnchor.MiddleRight;
                            break;

                        case TextAlign.BottomLeft:
                            text.alignment = TextAnchor.LowerLeft;
                            break;

                        case TextAlign.BottomCenter:
                            text.alignment = TextAnchor.LowerCenter;
                            break;

                        case TextAlign.BottomRight:
                            text.alignment = TextAnchor.LowerRight;
                            break;
                    }
                }
            }
        }
        public override TextOverflow Overflow
        {
            get => overflow;
            set
            {
                overflow = value;
                if (text != null)
                {
                    switch (overflow)
                    {
                        case TextOverflow.Clip:
                            text.verticalOverflow = VerticalWrapMode.Truncate;
                            break;
                        case TextOverflow.Overflow:
                            text.verticalOverflow = VerticalWrapMode.Overflow;
                            break;
                        case TextOverflow.Ellipsis:
                            throw new NotImplementedException();
                    }
                }
            }
        }
        
        public void InflateGameObject(GameObject gameObject)
        {
            if (text != null)
                throw new Exception("Component already inflated.");

            text = gameObject.AddComponent<Text>();

            Data = data;
            Style = style;
            Alignment = alignment;
            Overflow = overflow;
        }

        public void Clear()
        {
            UnityEngine.Object.Destroy(text);
            text = null;
        }
    }

    public class UnityColorComponent : ColorComponent, IUnityFlighterComponent
    {
        Color color;

        public override Color Color
        {
            get => color;
            set
            {
                color = value;
                if (image != null)
                {
                    var unityColor = new UnityEngine.Color(color.r, color.g, color.b, color.a);
                    image.color = unityColor;
                }
            }
        }

        Image image;

        public void InflateGameObject(GameObject gameObject)
        {
            if (image != null)
                throw new Exception("Component already inflated.");

            image = gameObject.AddComponent<Image>();

            Color = color;
        }

        public void Clear()
        {
            UnityEngine.Object.Destroy(image);
            image = null;
        }
    }
}
