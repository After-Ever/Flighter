using System;
using System.Collections.Generic;

using Flighter;
using Flighter.Core;
using UnityEngine;
using Component = Flighter.Component;

namespace FlighterUnity
{
    public interface IUnityFlighterComponent
    {
        void InflateGameObject(GameObject gameObject);
        void Clear();
    }

    public class ComponentProviderMaker
    {
        public static ComponentProvider Make()
            => new ComponentProvider(new Dictionary<Type, Type>
            {
                { typeof(TextComponent), typeof(UnityTextComponent) },
                { typeof(ColorComponent), typeof(UnityColorComponent) }
            });
    }
}
