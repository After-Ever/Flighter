using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Text : DisplayWidget
    {
        public readonly string data;
        public readonly TextStyle style;
        public readonly TextAlign alignment;
        public readonly TextOverflow overflow;

        public Text(
            string data,
            TextStyle? style = null,
            TextAlign alignment = TextAlign.TopLeft,
            TextOverflow overflow = TextOverflow.Ellipsis)
        {
            this.data = data;
            this.style = style ?? GetDefaultStyle();
            this.alignment = alignment;
            this.overflow = overflow;
        }

        public override bool IsSame(Widget other)
        {
            return other is Text t &&
                data == t.data &&
                style.Equals(t.style) &&
                alignment.Equals(t.alignment) &&
                overflow == t.overflow;
        }

        public override Element CreateElement()
        {
            return new TextElement();
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            throw new NotImplementedException();
        }

        TextStyle GetDefaultStyle()
        {
            throw new NotImplementedException();
        }
    }

    public class TextElement : Element
    {
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
            component.Alignment = w.alignment;
            component.Overflow = w.overflow;
        }
    }
}
