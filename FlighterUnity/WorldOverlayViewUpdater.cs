using System;
using System.Collections.Generic;
using UnityEngine;

using Flighter;
using Flighter.Core;

namespace FlighterUnity
{
    internal class WorldOverlayViewUpdater : StatefulWidget
    {
        public readonly Widget child;

        public readonly Transform transform;

        public WorldOverlayViewUpdater(
            Widget child,
            Transform transform)
        {
            this.child = child
                ?? throw new ArgumentNullException(nameof(child));
            this.transform = transform
                ?? throw new ArgumentNullException(nameof(transform));
        }

        public override State CreateState()
            => new WorldOverlayViewUpdaterState();
    }

    internal class WorldOverlayViewUpdaterState : State<WorldOverlayViewUpdater>
    {
        TickSource lastTickSource;

        Vector3 lastPosition;
        Vector3 lastScale;
        Quaternion lastRotation;

        Vector3 lastCamPosition;
        Quaternion lastCamRotation;
        float lastCamFov;
        // TODO There could be other things which change where the target is
        //  relative to the camera. Eg, Camera projection mode.

        Camera cam => Camera.main;

        public override void Init()
        {
            UpdateStoredTransform();

            lastTickSource = GetTickSource();
            lastTickSource += OnTick;
        }

        public override void WidgetChanged()
        {
            var tickSource = GetTickSource();

            if (lastTickSource != tickSource)
            {
                lastTickSource -= OnTick;
                tickSource += OnTick;
                lastTickSource = tickSource;
            }
        }

        public override Widget Build(BuildContext context)
            => widget.child;
        void OnTick(float time, float delta)
        {
            if (lastPosition != widget.transform.position
                || lastScale != widget.transform.localScale
                || lastRotation != widget.transform.rotation
                || lastCamPosition != cam.transform.position
                || lastCamRotation != cam.transform.rotation
                || lastCamFov != cam.fieldOfView)
            {
                SetState(UpdateStoredTransform);
            }
        }

        TickSource GetTickSource()
            => TickSource.Of(context)
                ?? throw new Exception($"{nameof(WorldOverlayView)} must inherit from " +
                $"{nameof(TickSource)}!");

        void UpdateStoredTransform()
        {
            lastPosition = widget.transform.position;
            lastScale = widget.transform.localScale;
            lastRotation = widget.transform.rotation;

            lastCamPosition = cam.transform.position;
            lastCamRotation = cam.transform.rotation;
            lastCamFov = cam.fieldOfView;
        }
    }
}
