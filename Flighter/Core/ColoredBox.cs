using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class ColoredBox : DisplayWidget
    {
        public readonly Color color;

        public ColoredBox(Color color)
        {
            this.color = color;
        }

        public override bool IsSame(Widget other)
        {
            return other is ColoredBox c && c.color.Equals(color);
        }

        public override Element CreateElement()
        {
            return new ColoredBoxElement();
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var size = context.constraints.MaxSize;
            return new BuildResult(size);
        }
    }

    class ColoredBoxElement : Element
    {
        public override string Name => "ColoredBox";

        ColorComponent component;

        protected override void _Init()
        {
            component = componentProvider.CreateComponent<ColorComponent>();
            DisplayRect.AddComponent(component);
        }

        protected override void _Update()
        {
            component.Color = GetWidget<ColoredBox>().color;
        }
    }
}
