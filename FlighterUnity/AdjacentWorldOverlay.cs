using System;
using System.Collections.Generic;
using UnityEngine;

using Vector2 = System.Numerics.Vector2;

using Flighter;

namespace FlighterUnity
{
    public class AdjacentWorldOverlay : WorldOverlayView
    {
        readonly Direction direction;
        // TODO Allow changing the meaning of start and end with Horizontal and Vertical direction.
        readonly bool boundStart;
        readonly bool boundEnd;
        readonly float align;

        public AdjacentWorldOverlay(
            WorldOverlayBuilder builder,
            Transform transform,
            Direction direction,
            bool boundStart = true,
            bool boundEnd = true,
            float align = 0,
            List<Vector3> referencePoints = null)
            : base(builder, transform, referencePoints)
        {
            this.direction = direction;
            this.boundStart = boundStart;
            this.boundEnd = boundEnd;
            this.align = align;
        }

        public override BoxConstraints GetConstraints(
            Vector2 refOffset, 
            Size refSize,
            BoxConstraints parentConstraints)
        {
            if (direction == Direction.Left || direction == Direction.Right)
            {
                var start = boundStart ? refOffset.Y : 0;
                var end = boundEnd ? refOffset.Y + refSize.height : parentConstraints.maxHeight;
                var height = end - start;
                var width = direction == Direction.Left
                    ? refOffset.X
                    : parentConstraints.maxWidth - refOffset.X - refSize.width;

                return BoxConstraints.Loose(width, height);
            }
            else
            {
                var start = boundStart ? refOffset.X : 0;
                var end = boundEnd ? refOffset.X + refSize.width : parentConstraints.maxWidth;
                var width = end - start;
                var height = direction == Direction.Up
                    ? refOffset.Y
                    : parentConstraints.maxHeight - refOffset.Y - refSize.height;

                return BoxConstraints.Loose(width, height);
            }
        }

        public override Vector2 GetOffset(
            Vector2 refOffset,
            Size refSize,
            BoxConstraints parentConstraints,
            Size realizedSize)
        {
            if (direction == Direction.Left || direction == Direction.Right)
            {
                var y = (refSize.height - realizedSize.height) * align + refOffset.Y;

                // TODO: This is the same as in GetConstraints. Should consolidate...
                var start = boundStart ? refOffset.Y : 0;
                var end = boundEnd ? refOffset.Y + refSize.height : parentConstraints.maxHeight;

                // If the size is greater than the bounds, we don't adjust the offset.
                if (realizedSize.height <= end - start)
                {
                    if (y < start)
                        y = start;
                    else if (y + realizedSize.height > end)
                        y = end - realizedSize.height;
                }

                var x = direction == Direction.Left
                    ? refOffset.X - realizedSize.width
                    : refOffset.X + refSize.width;

                return new Vector2(x, y);
            }
            else
            {
                var x = (refSize.width - realizedSize.width) * align + refOffset.X;

                // TODO: This is the same as in GetConstraints. Should consolidate...
                var start = boundStart ? refOffset.X : 0;
                var end = boundEnd ? refOffset.X + refSize.width : parentConstraints.maxWidth;

                // If the size is greater than the bounds, we don't adjust the offset.
                if (realizedSize.width <= end - start)
                {
                    if (x < start)
                        x = start;
                    else if (x + realizedSize.width > end)
                        x = end - realizedSize.width;
                }

                var y = direction == Direction.Up
                    ? refOffset.Y - realizedSize.height
                    : refOffset.Y + refSize.height;

                return new Vector2(x, y);
            }
        }
    }
}
