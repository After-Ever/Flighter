using System;
using System.Collections.Generic;
using System.Text;

using Flighter;
using UnityEngine;

using Component = Flighter.Component;
using FlighterVec = System.Numerics.Vector2;

namespace FlighterUnity
{
    public class DisplayRect : IDisplayRect
    {
        public const int FlighterLayer = 31;
        public const int FlighterLayerMask = 1 << FlighterLayer;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public Size Size
        {
            get
            {
                return (transform.sizeDelta.ToFlighter()).ToSize();
            }
            set
            {
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.width);
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.height);
            }
        }

        public FlighterVec Offset
        {
            get
            {
                var unityOffset = transform.anchoredPosition;
                return new FlighterVec(unityOffset.x, -unityOffset.y);
            }
            set
            {
                var pixelOffset = new Vector2(value.X, -value.Y);
                transform.anchoredPosition = pixelOffset;
            }
        }

        readonly GameObject gameObject;
        public readonly RectTransform transform;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale">Unity units per flighter unit.</param>
        public DisplayRect(RectTransform rect)
        {
            if (rect == null)
                throw new ArgumentNullException();

            gameObject = rect.gameObject;
            transform = rect;

            gameObject.layer = FlighterLayer;
        }

        public void AddComponent(Component component)
        {
            var c = component as IUnityFlighterComponent ?? throw new Exception("Invalid component.");
            c.InflateGameObject(gameObject);
        }

        public IDisplayRect CreateChild()
        {
            var g = new GameObject();
            var t = g.AddComponent<RectTransform>();
            t.SetParent(transform);
            t.pivot = t.anchorMax = t.anchorMin = new Vector2(0, 1);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = new Vector3(1, 1, 1);

            return new DisplayRect(t);
        }

        public void RemoveComponent(Component component)
        {
            var c = component as IUnityFlighterComponent ?? throw new Exception("Invalid component.");
            c.Clear();
        }

        public void SetParent(IDisplayRect rect)
        {
            var parent = 
                rect == null 
                ? null 
                : rect as DisplayRect ?? throw new Exception("Invalid parent rect.");
            transform.SetParent(parent?.transform, worldPositionStays: false);
        }

        public void SetSiblingIndex(int index)
        {
            transform.SetSiblingIndex(index);
        }

        public void TearDown()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
