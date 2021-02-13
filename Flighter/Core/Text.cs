﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Text : DisplayWidget
    {
        public readonly string data;
        public readonly TextStyle style;

        public Text(
            string data,
            TextStyle style)
        {
            this.data = data;
            this.style = style;
        }
        
        public override Element CreateElement()
        {
            return new TextElement();
        }

        public override bool Equals(object obj)
        {
            var text = obj as Text;
            return text != null &&
                   data == text.data &&
                   EqualityComparer<TextStyle?>.Default.Equals(style, text.style);
        }

        public override int GetHashCode()
        {
            var hashCode = 362845251;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(data);
            hashCode = hashCode * -1521134295 + EqualityComparer<TextStyle?>.Default.GetHashCode(style);
            return hashCode;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            return new BuildResult(style.font.PreferredSize(data, style, context.constraints));
        }
    }

    public class TextElement : Element
    {
        public override string Name => "Text";

        TextComponent component;

        protected override void _Init()
        {
            component = componentProvider.CreateComponent<TextComponent>();
            DisplayRect.AddComponent(component);
        }

        protected override void _Update()
        {
            var w = GetWidget<Text>();
            component.Data = w.data;
            component.Style = w.style;
        }
    }
}
