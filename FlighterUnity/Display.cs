using Flighter;
using Flighter.Input;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
            
            return InstrumentWidget(widget, rect, screenInputProvider);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="size">The size of the display in world units.</param>
        /// <param name="transform">The transform on which this display will be mounted.
        /// The display will extend down and to the right.</param>
        /// <param name="pixelsPerUnit">How many pixels per world unit.</param>
        /// <returns></returns>
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
            InputProvider inputProvider = null)
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
                inputProvider);

            handle.WasTornDown += () =>
            {
                UnityEngine.Object.Destroy(parentObj);
            };

            return handle;
        }

        static DisplayRect screenRect;
        static InputProvider screenInputProvider;
        static readonly ComponentProvider componentProvider = ComponentProviderMaker.Make();

        static DisplayHandle InstrumentWidget(
            Widget widget, 
            DisplayRect rect, 
            InputProvider inputProvider = null)
        {
            var size = rect.Size;
            var constraints = BoxConstraints.Tight(size);

            var root = RootWidget.MakeRootWidgetNode(
                widget,
                new BuildContext(constraints),
                rect,
                componentProvider);


            var rootController = rect.transform.gameObject.AddComponent<RootController>();
            rootController.SetRoot(root);

            if (inputProvider != null)
            {
                inputProvider.AddRoot(root.Item1);
                rootController.TornDown += () => inputProvider.RemoveRoot(root.Item1);
            }


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

            screenInputProvider = obj.AddComponent<InputProvider>();
            screenInputProvider.SetPoller(new InputPoller());

            screenRect = new DisplayRect(rect);
        }

        static (RectTransform, InputProvider) CreateRootWorldObject(float pixelPerUnit, Size size)
        {
            var rect = BaseRect(new Size(size.width * pixelPerUnit, size.height * pixelPerUnit));
            var obj = rect.gameObject;

            rect.localScale = new Vector3(1 / pixelPerUnit, 1 / pixelPerUnit, 1);

            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            // TODO: Is the canvas scaler necessary?
            var scalar = obj.AddComponent<CanvasScaler>();
            scalar.referencePixelsPerUnit = 1;
            scalar.dynamicPixelsPerUnit = 1;

            var inputProvider = obj.AddComponent<InputProvider>();
            inputProvider.SetPoller(new InputPoller(rect, pixelPerUnit));

            return (rect, inputProvider);
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
