﻿using System;
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
            var constraints = context.constraints;
            float newMaxWidth = constraints.maxWidth;
            float newMaxHeight = constraints.maxHeight;

            if (constraints.IsUnconstrained)
            {
                if (float.IsInfinity(constraints.maxWidth))
                {
                    if (float.IsInfinity(constraints.maxHeight))
                        throw new Exception("Cannot have both axis unconstrained.");

                    var heightToWidthRatio = imageHandle.size.width / imageHandle.size.height;
                    newMaxWidth = constraints.maxHeight * heightToWidthRatio;
                }
                else
                {
                    var widthToHeightRatio = imageHandle.size.height / imageHandle.size.width;
                    newMaxHeight = constraints.maxWidth * widthToHeightRatio;
                }
            }

            constraints = new BoxConstraints(
                maxWidth: newMaxWidth,
                maxHeight: newMaxHeight,
                minWidth: constraints.minWidth,
                minHeight: constraints.minHeight);
            var maxSize = constraints.MaxSize;
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

        public override bool Equals(object obj)
        {
            var image = obj as Image;
            return image != null &&
                   EqualityComparer<IImageHandle>.Default.Equals(imageHandle, image.imageHandle) &&
                   EqualityComparer<Color?>.Default.Equals(color, image.color) &&
                   fit == image.fit &&
                   EqualityComparer<Alignment>.Default.Equals(alignment, image.alignment);
        }

        public override int GetHashCode()
        {
            var hashCode = -1833654010;
            hashCode = hashCode * -1521134295 + EqualityComparer<IImageHandle>.Default.GetHashCode(imageHandle);
            hashCode = hashCode * -1521134295 + EqualityComparer<Color?>.Default.GetHashCode(color);
            hashCode = hashCode * -1521134295 + fit.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Alignment>.Default.GetHashCode(alignment);
            return hashCode;
        }
    }

    class _Image : DisplayWidget
    {
        public readonly Image image;

        public _Image(Image image)
        {
            this.image = image ?? throw new ArgumentNullException();
        }

        public override DisplayBox CreateElement() => new _ImageElement();

        public override bool Equals(object obj)
        {
            var image = obj as _Image;
            return image != null &&
                   EqualityComparer<Image>.Default.Equals(this.image, image.image);
        }

        public override int GetHashCode()
        {
            return -1125144822 + EqualityComparer<Image>.Default.GetHashCode(image);
        }

        public override Size Layout(BuildContext context, ILayoutController node)
        {
            return context.constraints.MaxSize;
        }
    }

    class _ImageElement : DisplayBox
    {
        ImageComponent imageComponent;

        public override string Name => "Image";

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
