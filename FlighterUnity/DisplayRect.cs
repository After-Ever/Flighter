using System;
using System.Collections.Generic;
using System.Text;

using Flighter;
using UnityEngine;
using Component = Flighter.Component;

namespace FlighterUnity
{
    public class DisplayRect : IDisplayRect
    {
        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public Size Size
        {
            get
            {
                var unitySize = transform.sizeDelta;
                return new Size(unitySize.x, unitySize.y);
            }
            set
            {
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.width);
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.height);
            }
        }

        public Point Offset
        {
            get
            {
                var unityOffset = transform.anchoredPosition;
                return new Point(unityOffset.x, -unityOffset.y);
            }
            set
            {
                var unityOffset = new Vector2(value.x, -value.y);
                transform.anchoredPosition = unityOffset;
            }
        }

        readonly GameObject gameObject;
        readonly RectTransform transform;

        public DisplayRect(RectTransform parent)
        {
            if (parent == null)
                throw new ArgumentNullException();

            gameObject = new GameObject();
            transform = gameObject.AddComponent<RectTransform>();
            transform.SetParent(parent, false);
            
            transform.anchorMax 
                = transform.anchorMin 
                = transform.pivot 
                = new Vector2(0, 1);

            transform.anchoredPosition = Vector2.zero;
        }

        public void AddComponent(Component component)
        {
            var c = component as IUnityFlighterComponent ?? throw new Exception("Invalid component.");
            c.InflateGameObject(gameObject);
        }

        public IDisplayRect CreateChild() => new DisplayRect(transform);

        public void RemoveComponent(Component component)
        {
            var c = component as IUnityFlighterComponent ?? throw new Exception("Invalid component.");
            c.Clear();
        }

        public void SetParent(IDisplayRect rect)
        {
            var parent = rect as DisplayRect ?? throw new Exception("Invalid parent rect. ");
            transform.SetParent(parent.transform);
        }

        public void TearDown()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
