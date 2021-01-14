using System;
using System.Collections.Generic;

namespace Flighter
{
    /// <summary>
    /// Base class for all display components.
    /// </summary>
    public abstract class Component { }

    /// <summary>
    /// Provider for platform based imprementations of components.
    /// </summary>
    public class ComponentProvider
    {
        readonly Dictionary<Type, Type> componentImplementations;

        public ComponentProvider(Dictionary<Type, Type> componentImplementations)
        {
            this.componentImplementations = componentImplementations ?? throw new ArgumentNullException();
        }

        public C CreateComponent<C>() where C : Component
        {
            // TODO: What if they request a more generic type? We should return any type that "is" the requested type. 
            componentImplementations.TryGetValue(typeof(C), out Type imp);

            return imp
                ?.GetConstructor(new Type[0])
                ?.Invoke(null) as C 
                ?? throw new CannotCreateComponentException(typeof(C));
        }
    }

    public class CannotCreateComponentException : Exception
    {
        public CannotCreateComponentException(Type type)
            : base("Unable to create Component of type " + type.ToString()) {}
    }
}
