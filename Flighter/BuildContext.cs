using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public struct BuildContext
    {
        public readonly BoxConstraints constraints;

        readonly Dictionary<Type, InheritedWidget> inheritedWidgets;

        public BuildContext(BoxConstraints constraints)
        {
            this.constraints = constraints;

            inheritedWidgets = new Dictionary<Type, InheritedWidget>();
        }

        BuildContext(BoxConstraints constraints, Dictionary<Type, InheritedWidget> inheritedWidgets)
        {
            this.constraints = constraints;
            this.inheritedWidgets = inheritedWidgets;
        }

        /// <summary>
        /// Return the nearest <see cref="InheritedWidget"/> with type <typeparamref name="T"/>,
        /// or null if there is no such widget.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInheritedWidgetOfExactType<T>() where T : InheritedWidget
        {
            if (inheritedWidgets.TryGetValue(typeof(T), out var val))
                return val as T;
            return null;
        }

        internal BuildContext AddInheritedWidget(InheritedWidget widget, Type type)
        {
            var with = new Dictionary<Type, InheritedWidget>(inheritedWidgets);
            with[type] = widget;

            return new BuildContext(constraints, with);
        }

        public BuildContext WithNewConstraints(BoxConstraints constraints)
        {
            return new BuildContext(constraints, inheritedWidgets);
        }
    }
}
