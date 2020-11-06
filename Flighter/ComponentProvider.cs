using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    /// <summary>
    /// Base class for all display components.
    /// </summary>
    public abstract class Component {}

    /// <summary>
    /// Provider for platform based imprementations of components.
    /// </summary>
    public interface IComponentProvider
    {
        C CreateComponent<C>() where C : Component;
    }
}
