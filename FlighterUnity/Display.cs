using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Input = Flighter.Input.Input;

namespace FlighterUnity
{
    public class DisplayHandle
    {
        RootController rootController;

        public event Action WasTornDown;

        public DisplayHandle(RootController rootController)
        {
            this.rootController = rootController 
                ?? throw new ArgumentNullException("Must provide a rootController.");
        }

        /// <summary>
        /// Tearn down this display.
        /// </summary>
        public void TearDown()
        {
            if (rootController == null)
                throw new Exception("This display has already been torn down.");

            rootController.TearDown();
            rootController = null;

            WasTornDown?.Invoke();
        }
    }

    public static class Display
    {
        /// <summary>
        /// Instrument <paramref name="widget"/> on the screen.
        /// Multiple calls to this will stack the widgets on one another, in call order
        /// (recent ontop of older).
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public static DisplayHandle OnScreen(Widget widget)
        {
            InitScreenRect();

            var rect = screenRect.CreateChild() as DisplayRect;
            rect.Size = screenRect.Size;
            
            return InstrumentWidget(widget, rect, screenInput);
        }

        public static DisplayHandle InWorld(
            Widget widget, 
            Size size,
            float pixelsPerUnit,
            Vector3 topLeftPosition,
            Quaternion rotation)
        {
            var obj = new GameObject("FlighterDisplayRoot");
            obj.transform.position = topLeftPosition;
            obj.transform.rotation = rotation;

            return OnTransform(widget, size, obj.transform, pixelsPerUnit);
        }

        public static DisplayHandle OnTransform(
            Widget widget, 
            Size size, 
            Transform transform, 
            float pixelsPerUnit)
        {
            (var rect, var input) = CreateRootWorldObject(pixelsPerUnit, size);
            rect.SetParent(transform);
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;

            return InstrumentWidget(widget, new DisplayRect(rect, pixelsPerUnit), input);
        }

        public static DisplayHandle ToRenderTexture(
            Widget widget, 
            RenderTexture target, 
            Color backgroundColor = new Color(),
            Input input = null)
        {
            var camObj = new GameObject("FlighterRenderCam");
            var cam = camObj.AddComponent<Camera>();
            cam.targetTexture = target;
            cam.cullingMask = DisplayRect.FlighterLayerMask;
            cam.backgroundColor = backgroundColor;

            var rect = BaseRect(new Size(target.width, target.height));
            var obj = rect.gameObject;

            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;

            var parentObj = new GameObject("FlighterRenderTextureDisplay");
            var pt = parentObj.transform;

            camObj.transform.SetParent(pt);
            rect.SetParent(pt);

            var handle = InstrumentWidget(
                widget, 
                new DisplayRect(rect), 
                input ?? Flighter.Input.NoInput.Input);

            handle.WasTornDown += () =>
            {
                UnityEngine.Object.Destroy(parentObj);
            };

            return handle;
        }

        static DisplayRect screenRect;
        static Input screenInput;
        static readonly ComponentProvider componentProvider = ComponentProviderMaker.Make();

        static DisplayHandle InstrumentWidget(Widget widget, DisplayRect rect, Input input)
        {
            var size = rect.Size;
            var constraints = BoxConstraints.Tight(size);

            var root = RootWidget.MakeRootWidgetNode(
                widget,
                new BuildContext(constraints),
                rect,
                componentProvider,
                input);

            var rootController = rect.transform.gameObject.AddComponent<RootController>();
            rootController.SetRoot(root);

            return new DisplayHandle(rootController);
        }

        static void InitScreenRect()
        {
            if (screenRect != null)
                return;

            var obj = new GameObject("FlighterScreenDisplayRoot");
            var rect = obj.AddComponent<RectTransform>();

            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var inputProvider = obj.AddComponent<InputProvider>();
            inputProvider.SetPoller(new InputPoller());

            screenRect = new DisplayRect(rect);
            screenInput = inputProvider.GetInput();
        }

        static (RectTransform, Input) CreateRootWorldObject(float pixelPerUnit, Size size)
        {
            var rect = BaseRect(size);
            var obj = rect.gameObject;

            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scalar = obj.AddComponent<CanvasScaler>();
            scalar.referencePixelsPerUnit = pixelPerUnit;
            scalar.dynamicPixelsPerUnit = pixelPerUnit;

            var inputProvider = obj.AddComponent<InputProvider>();
            inputProvider.SetPoller(new InputPoller(rect, pixelPerUnit));

            return (rect, inputProvider.GetInput());
        }

        static RectTransform BaseRect(Size size)
        {
            var obj = new GameObject();
            var rect = obj.AddComponent<RectTransform>();
            rect.pivot
                = rect.anchorMax
                = rect.anchorMin
                = new Vector2(0, 1);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.height);

            return rect;
        }
    }
}
