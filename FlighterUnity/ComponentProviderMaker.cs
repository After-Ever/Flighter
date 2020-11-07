using System;
using System.Collections.Generic;

using Flighter;
using Flighter.Core;

namespace FlighterUnity
{
    public class ComponentProviderMaker
    {
        static ComponentProvider Make()
            => new ComponentProvider(new Dictionary<Type, Type>
            {
                { typeof(TextComponent), typeof(UnityTextComponent) },
                { typeof(ColorComponent), typeof(UnityColorComponent) }
            });
    }
}
