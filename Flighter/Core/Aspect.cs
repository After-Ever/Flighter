using System;
namespace Flighter.Core
{
    /// <summary>
    /// Force the child widget into a particular aspect ratio.
    /// Must have at least one axis constrained.
    /// </summary>
    public class Aspect : StatelessWidget
    {
        /// <summary>
        /// The ratio between width and height (w/h) this Widget will be
        /// forced into.
        /// </summary>
        public readonly float aspect;
        public readonly Widget child;

        public Aspect(float aspect, Widget child)
        {
            this.aspect = aspect;
            this.child = child 
                ?? throw new ArgumentNullException(nameof(child));
        }

        public override Widget Build(BuildContext context)
        {
            var c = context.constraints;

            if (float.IsInfinity(c.maxWidth) && float.IsInfinity(c.maxHeight))
                throw new Exception("At least one axis must be bounded.");

            var min = Math.Min(c.maxWidth / aspect, c.maxHeight);
            var box = BoxConstraints.Tight(min * aspect, min);

            return new BoxConstrained(child, box);
        }
    }
}
