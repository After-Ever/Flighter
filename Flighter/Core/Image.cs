using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public interface IImageHandle
    {
        Size size { get; }
    }

    public class Image : StatelessWidget
    {
        public readonly IImageHandle imageHandle;
        /// <summary>
        /// If not null, this color will be blended with the image.
        /// </summary>
        public readonly Color? color;
        public readonly BoxFit fit;
        public readonly Alignment alignment;

        public Image(
            IImageHandle imageHandle,
            BoxFit fit = BoxFit.Contain,
            Alignment? alignment = null,
            Color? color = null)
        {
            this.imageHandle = imageHandle ?? throw new ArgumentNullException();
            this.color = color;
            this.fit = fit;
            this.alignment = alignment ?? Alignment.MiddleCenter;
        }

        public override Widget Build(BuildContext context)
        {
            if (context.constraints.IsUnconstrained)
                throw new Exception("Image cannot be unconstrained.");

            var maxSize = context.constraints.MaxSize;
            var boxRatio = maxSize.width / maxSize.height;
            var imageRatio = imageHandle.size.width / imageHandle.size.height;

            float width, height;
            bool overflowed = false;
            switch (fit)
            {
                case BoxFit.Contain:
                    if (boxRatio > imageRatio)
                    {
                        height = maxSize.height;
                        width = height * imageRatio;
                    }
                    else
                    {
                        width = maxSize.width;
                        height = width / imageRatio;
                    }
                    break;
                case BoxFit.Cover:
                    if (boxRatio > imageRatio)
                    {
                        width = maxSize.width;
                        height = width * imageRatio;
                    }
                    else
                    {
                        height = maxSize.height;
                        width = height / imageRatio;
                    }

                    overflowed = boxRatio != imageRatio;
                    break;
                case BoxFit.Fill:
                    width = maxSize.width;
                    height = maxSize.height;
                    break;
                case BoxFit.FitHeight:
                    height = maxSize.height;
                    width = height * imageRatio;

                    overflowed = boxRatio < imageRatio;
                    break;
                case BoxFit.FitWidth:
                    width = maxSize.width;
                    height = width / imageRatio;

                    overflowed = imageRatio < boxRatio;
                    break;
                default:
                    throw new NotSupportedException();
            }

            var widget = new Align(
                    alignment: alignment,
                    child: new BoxConstrained(
                        constraints: BoxConstraints.Tight(new Size(width, height)),
                        child: new _Image(this)));

            // If overflowed, need to clip.
            return overflowed
                ? new Clip(widget) as Widget
                : widget;
        }
    }

    class _Image : DisplayWidget
    {
        public readonly Image image;

        public _Image(Image image)
        {
            this.image = image ?? throw new ArgumentNullException();
        }

        public override Element CreateElement() => new _ImageElement();

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            return new BuildResult(context.constraints.MaxSize);
        }
    }

    class _ImageElement : Element
    {
        ImageComponent imageComponent;

        protected override void _Init()
        {
            imageComponent = componentProvider.CreateComponent<ImageComponent>();

            DisplayRect.AddComponent(imageComponent);
        }

        protected override void _Update()
        {
            var w = GetWidget<_Image>();

            imageComponent.Color = w.image.color;
            imageComponent.ImageHandle = w.image.imageHandle;
        }
    }
}
