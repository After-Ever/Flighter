using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Vector2 = System.Numerics.Vector2;

using Flighter;
using System.Linq;

namespace FlighterUnity
{
    public class WorldOverlayDomain : LayoutWidget
    {
        readonly List<WorldOverlayView> children;
        readonly Vector2 offset;

        /// <summary>
        /// Allows displaying on screen UI anchored to in-world objects.
        /// </summary>
        /// <param name="children">List of child widgets. Children of type 
        /// <see cref="WorldOverlayView"/> will be linked to an in-world object.</param>
        /// <param name="offset">The absolute offset of this widget 
        /// relative to the top left of the screen.</param>
        public WorldOverlayDomain(List<WorldOverlayView> children, Vector2 offset = default)
        {
            this.children = children
                ?? throw new ArgumentNullException(nameof(children));
            this.offset = offset;
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
        {
            // TODO: Does this have to be the case?
            if (context.constraints.IsUnconstrained)
                throw new Exception($"{nameof(WorldOverlayDomain)} must be given a constrained" +
                    $"context.");

            foreach (var c in children)
            {
                var t = c.transform;
                Vector3 refCenter = Vector3.zero;
                var refPoints = new List<Vector2>();
                foreach (var p in c.referencePoints)
                {
                    var wp = t.localToWorldMatrix.MultiplyPoint(p);
                    var sp = Camera.main.WorldToScreenPoint(wp);
                    // Convert to top-left based coords.
                    sp.y = Screen.height - sp.y;
                    refPoints.Add(new Vector2(sp.x, sp.y) - offset);

                    refCenter += wp;
                }

                if (refPoints.Count == 0)
                    throw new Exception("Should have at least one point.");

                refCenter /= refPoints.Count;
                var refDist = Vector3.Distance(refCenter, Camera.main.transform.position);

                var refOffset = new Vector2(
                    refPoints.Min(p => p.X),
                    refPoints.Min(p => p.Y));
                var refSize = new Size(
                    refPoints.Max(p => p.X) - refOffset.X,
                    refPoints.Max(p => p.Y) - refOffset.Y);

                var childConstraints = c.GetConstraints(
                    refOffset, refSize, context.constraints);

                var child = c.builder(
                    context.WithNewConstraints(childConstraints),
                    refDist);
                var childLayout = layoutController.LayoutChild(
                    // The child is wrapped in a stateful widget which will
                    // rebuild whenever the transform, or camera, move.
                    new WorldOverlayViewUpdater(child, c.transform),
                    childConstraints);

                childLayout.offset = c.GetOffset(
                    refOffset, refSize, context.constraints, childLayout.size);
            }

            return context.constraints.MaxSize;
        }
    }
}
