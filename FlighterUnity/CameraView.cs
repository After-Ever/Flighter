using System;

using Flighter;
using UnityEngine;

namespace FlighterUnity
{
    public class CameraView : DisplayWidget
    {
        public readonly Camera camera;

        public CameraView(Camera camera)
        {
            this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }

        public override DisplayBox CreateElement()
            => new _Element();

        public override Size Layout(BuildContext context, ILayoutController layoutController)
            => context.constraints.MaxSize;

        class _Element : DisplayBox
        {
            public override string Name => "Camera View";

            RawImageComponent component;

            Camera cam;
            RenderTexture texture;

            protected override void _Init()
            {
                component = new RawImageComponent();
                DisplayRect.AddComponent(component);
            }

            protected override void _TearDown()
            {
                TearDownCam();
            }

            protected override void _Update()
            {
                var c = GetWidget<CameraView>().camera;

                var curWidth = (int)DisplayRect.Size.width;
                var curHeight = (int)DisplayRect.Size.height;

                if (c != cam || texture == null)
                    InitCam(c, curWidth, curHeight);
                else if (curWidth != c.pixelWidth || curHeight != c.pixelHeight)
                {
                    texture.Release();
                    texture.width = curWidth;
                    texture.height = curHeight;
                    texture.Create();
                    cam.ResetAspect();
                }
            }

            void InitCam(Camera c, int w, int h)
            {
                cam = c;
                texture?.Release();
                texture = new RenderTexture(w, h, 0);
                texture.Create();
                cam.targetTexture = texture;
                cam.enabled = true;

                component.Texture = texture;
            }

            void TearDownCam()
            {
                if (cam == null)
                    return;

                cam.targetTexture = null;
                cam.enabled = false;

                // TODO Does it need to be destroyed?
                texture?.Release();
                texture = null;

                component.Texture = null;
            }
        }
    }
}