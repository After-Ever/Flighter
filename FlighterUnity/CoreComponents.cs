using System;

using UnityEngine;
using Flighter.Core;
using Color = Flighter.Core.Color;
using Text = UnityEngine.UI.Text;
using Image = UnityEngine.UI.Image;
using UnityEngine.UI;
using FontStyle = Flighter.Core.FontStyle;

using Resources = UnityEngine.Resources;

namespace FlighterUnity
{
    public class UnityTextComponent : TextComponent, IUnityFlighterComponent
    {
        public static readonly TextStyle defaultStyle = new TextStyle
        {
            // TODO: This is an important idea! It must be well and clearly documented.
            //       Is there a more reliable way to load a default font?
            font = new FontHandle(Resources.Load<Font>("default_font")),
            size = 12,
            lineSpacing = 1,
            textAlign = TextAlign.TopLeft,
            fontStyle = FontStyle.Normal,
            wrapLines = true,
            textOverflow = TextOverflow.Clip,
            color = new Color(0,0,0),
        };

        string data;
        TextStyle? style;

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
        public override TextStyle? Style
        {
            get => style;
            set
            {
                style = value;
                if (text != null)
                {
                    var s = style ?? defaultStyle;

                    if (!(s.font is FontHandle fontHandle))
                        throw new NotSupportedException();

                    text.font = fontHandle.font;
                    text.fontStyle = s.fontStyle.ToUnity();
                    text.fontSize = s.size;
                    text.lineSpacing = s.lineSpacing;
                    text.alignment = s.textAlign.ToUnity();
                    text.color = s.color.ToUnity();
                    text.horizontalOverflow = s.wrapLines 
                        ? HorizontalWrapMode.Wrap 
                        : HorizontalWrapMode.Overflow;

                    switch (s.textOverflow)
                    {
                        case TextOverflow.Clip:
                            text.verticalOverflow = VerticalWrapMode.Truncate;
                            break;
                        case TextOverflow.Overflow:
                            text.verticalOverflow = VerticalWrapMode.Overflow;
                            break;
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
                    image.color = color.ToUnity();
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

    public class UnityImageComponent : ImageComponent, IUnityFlighterComponent
    {
        public override Color? Color
        {
            get => image?.color.ToFlighter();
            set
            {
                if (image != null)
                    image.color = value?.ToUnity() ?? UnityEngine.Color.white;
            }
        }

        public override IImageHandle ImageHandle
        {
            get => imageHandle;
            set
            {
                if (value != null && !(value is ImageHandle))
                    throw new NotSupportedException();

                imageHandle = value as ImageHandle;

                if (image != null)
                    image.sprite = imageHandle?.sprite;
            }
        }

        Image image;
        ImageHandle imageHandle;
        
        public void InflateGameObject(GameObject gameObject)
        {
            if (image != null)
                throw new Exception("Component already inflated.");

            image = gameObject.AddComponent<Image>();
        }

        public void Clear()
        {
            UnityEngine.Object.Destroy(image);
            image = null;
        }
    }

    public class UnityClipComponent : ClipComponent, IUnityFlighterComponent
    {
        RectMask2D mask;

        public void InflateGameObject(GameObject gameObject)
        {
            mask = gameObject.AddComponent<RectMask2D>();
        }

        public void Clear()
        {
            UnityEngine.Object.Destroy(mask);
            mask = null;
        }
    }
}
