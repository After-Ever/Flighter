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

        public override DisplayBox CreateElement()
        {
            return new ColoredBoxElement();
        }

        public override bool Equals(object obj)
        {
            var box = obj as ColoredBox;
            return box != null &&
                   EqualityComparer<Color>.Default.Equals(color, box.color);
        }

        public override int GetHashCode()
        {
            return 790427672 + EqualityComparer<Color>.Default.GetHashCode(color);
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            var size = context.constraints.MaxSize;
            return size;
        }
    }

    class ColoredBoxElement : DisplayBox
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
