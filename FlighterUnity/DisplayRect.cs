﻿using System;
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
                return (transform.sizeDelta.ToPoint() * scale).ToSize();
            }
            set
            {
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.width / scale);
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.height / scale);
            }
        }

        public Point Offset
        {
            get
            {
                var unityOffset = transform.anchoredPosition * scale;
                return new Point(unityOffset.x, -unityOffset.y);
            }
            set
            {
                var unityOffset = new Vector2(value.x, -value.y);
                transform.anchoredPosition = unityOffset / scale;
            }
        }

        readonly GameObject gameObject;
        public readonly RectTransform transform;
        /// <summary>
        /// Unity units per flighter unit.
        /// </summary>
        public readonly float scale;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="scale">Unity units per flighter unit.</param>
        public DisplayRect(RectTransform parent, float scale = 1)
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

            if (scale == 0)
                throw new Exception("Scale cannot be zero.");
            this.scale = scale;
        }

        public void AddComponent(Component component)
        {
            var c = component as IUnityFlighterComponent ?? throw new Exception("Invalid component.");
            c.InflateGameObject(gameObject);
        }

        public IDisplayRect CreateChild() => new DisplayRect(transform, scale);

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
            transform.SetParent(parent?.transform);
        }

        public void TearDown()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
