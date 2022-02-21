using System.Collections.Generic;
using UnityEngine;

using Vector2 = System.Numerics.Vector2;

using Flighter;

namespace FlighterUnity
{
    public class DirectWorldOverlay : WorldOverlayView
    {
        public DirectWorldOverlay(
            WorldOverlayBuilder builder,
            Transform transform,
            List<Vector3> referencePoints = null)
            : base(builder, transform, referencePoints) { }

        public override BoxConstraints GetConstraints(
            Vector2 refOffset,
            Size refSize,
            float refDistance,
            BoxConstraints parentConstraints)
            => BoxConstraints.Loose(refSize);

        public override Vector2 GetOffset(
            Vector2 refOffset,
            Size refSize,
            float refDistance,
            BoxConstraints parentConstraints,
            Size realizedSize)
            => refOffset;
    }
}
