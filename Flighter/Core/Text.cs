﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Text : DisplayWidget
    {
        public readonly string data;
        public readonly TextStyle? style;

        public Text(
            string data,
            TextStyle? style = null)
        {
            this.data = data;
            this.style = style;
        }

        public override Element CreateElement()
        {
            return new TextElement();
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            // TODO: Currently just taking up as much space as possible.
            //       Should take min space where possible.
            return new BuildResult(context.constraints.MaxSize);
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
