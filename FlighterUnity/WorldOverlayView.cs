using System;
using System.Collections.Generic;
using UnityEngine;

using Vector2 = System.Numerics.Vector2;

using Flighter;

namespace FlighterUnity
{
    public delegate Widget WorldOverlayBuilder(BuildContext context, float depth);

    public abstract class WorldOverlayView
    {
        public readonly WorldOverlayBuilder builder;

        public readonly Transform transform;
        public readonly List<Vector3> referencePoints;

        protected WorldOverlayView(
            WorldOverlayBuilder builder, 
            Transform transform,
            List<Vector3> referencePoints)
        {
            this.builder = builder
                ?? throw new ArgumentNullException(nameof(builder));
            this.transform = transform 
                ?? throw new ArgumentNullException(nameof(transform));
            this.referencePoints = referencePoints 
                ?? throw new ArgumentNullException(nameof(referencePoints));
        }

        /// <summary>
        /// Set the BoxConstraints for this widget, given the ref box
        /// and parent constraints.
        /// </summary>
        public abstract BoxConstraints GetConstraints(
            Vector2 refOffset,
            Size refSize,
            float refDistance,
            BoxConstraints parentConstraints);

        /// <summary>
        /// Set the offset of this widget given its realized size.
        /// </summary>
        /// <param name="refOffset">Offset of the ref box.</param>
        /// <param name="refSize">Size of the ref box.</param>
        /// <param name="parentConstraints">Incoming constraints of the parent.</param>
        /// <param name="realizedSize">Size of this widget.</param>
        public abstract Vector2 GetOffset(
            Vector2 refOffset,
            Size refSize,
            float refDistance,
            BoxConstraints parentConstraints,
            Size realizedSize);
    }
}
