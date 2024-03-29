﻿using AEUtils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter.Core
{
    [Serializable]
    public struct EdgeInsets
    {
        public float left, top, right, bottom;

        public float horizontal => left + right;
        public float vertical => top + bottom;

        /// <summary>
        /// Set all the edges with the same value.
        /// </summary>
        /// <param name="all"></param>
        public EdgeInsets(float all)
        {
            left = top = right = bottom = all;
        }

        /// <summary>
        /// Set edges individually.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        public EdgeInsets(
            float left = 0,
            float top = 0, 
            float right = 0, 
            float bottom = 0)
        {
            this.left = left;
            this.top = top;
            this.bottom = bottom;
            this.right = right;
        }


        public EdgeInsets From(
            float? left = null,
            float? top = null,
            float? right = null,
            float? bottom = null)
        {
            return new EdgeInsets(
                left ?? this.left,
                top ?? this.top,
                right ?? this.right,
                bottom ?? this.bottom);
        }

        /// <summary>
        /// Construct a new <see cref="EdgeInsets"/> by specifying a size per axis.
        /// 
        /// Total padding in each axis will be double the specified size.
        /// </summary>
        /// <returns>The axis.</returns>
        /// <param name="horizontal">Horizontal.</param>
        /// <param name="vertical">Vertical.</param>
        public static EdgeInsets Axis(float horizontal = 0, float vertical = 0)
            => new EdgeInsets(left: horizontal, right: horizontal, top: vertical, bottom: vertical);

        public static EdgeInsets Lerp(EdgeInsets a, EdgeInsets b, float f)
            => new EdgeInsets(
                left: MathUtils.Lerp(a.left, b.left, f),
                right: MathUtils.Lerp(a.right, b.right, f),
                top: MathUtils.Lerp(a.top, b.top, f),
                bottom: MathUtils.Lerp(a.bottom, b.bottom, f));

        public static EdgeInsets operator +(EdgeInsets a, EdgeInsets b)
            => new EdgeInsets(
                left: a.left + b.left,
                right: a.right + b.right,
                top: a.top + b.top,
                bottom: a.bottom + b.bottom);
    }

    public class Padding : LayoutWidget
    {
        public readonly Widget child;
        public readonly EdgeInsets edgeInsets;

        public Padding(Widget child, EdgeInsets edgeInsets, string key = null)
            : base(key)
        {
            this.child = child ?? throw new ArgumentNullException("Padding must have child.");
            this.edgeInsets = edgeInsets;
        }

        public override bool Equals(object obj)
        {
            var padding = obj as Padding;
            return padding != null &&
                   EqualityComparer<Widget>.Default.Equals(child, padding.child) &&
                   EqualityComparer<EdgeInsets>.Default.Equals(edgeInsets, padding.edgeInsets);
        }

        public override int GetHashCode()
        {
            var hashCode = -867541505;
            hashCode = hashCode * -1521134295 + EqualityComparer<Widget>.Default.GetHashCode(child);
            hashCode = hashCode * -1521134295 + EqualityComparer<EdgeInsets>.Default.GetHashCode(edgeInsets);
            return hashCode;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            var horizontal = edgeInsets.left + edgeInsets.right;
            var vertical = edgeInsets.top + edgeInsets.bottom;

            var constraints = context.constraints;
            var childConstraints = new BoxConstraints(
                minWidth: Math.Max(0, constraints.minWidth - horizontal),
                minHeight: Math.Max(0, constraints.minHeight - vertical),
                maxWidth: constraints.maxWidth - horizontal,
                maxHeight: constraints.maxHeight - vertical);

            var child = layout.LayoutChild(
                this.child, 
                childConstraints);

            child.offset = new Vector2(edgeInsets.left, edgeInsets.top);
            var childSize = child.size;

            return new Size(childSize.width + horizontal, childSize.height + vertical);
        }
    }
}
