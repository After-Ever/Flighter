using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flighter;
using Flighter.Core;
using Flighter.Input;

public class Container : StatelessWidget
{
    public Widget child;
    public Color? backgroundColor;
    public EdgeInsets? padding;
    public EdgeInsets? flexPadding;
    public EdgeInsets? margin;
    public EdgeInsets? flexMargin;
    public Alignment? alignment;
    public BoxConstraints? boxConstraints;

    public Container(
        Widget child,
        Color? backgroundColor = null, 
        EdgeInsets? padding = null,
        EdgeInsets? flexPadding = null, 
        EdgeInsets? margin = null,
        EdgeInsets? flexMargin = null,
        Alignment? alignment = null, 
        BoxConstraints? boxConstraints = null)
    {
        this.child = child ?? throw new ArgumentNullException("Child must not be null.");
        this.backgroundColor = backgroundColor;
        this.padding = padding;
        this.flexPadding = flexPadding;
        this.margin = margin;
        this.flexMargin = flexMargin;
        this.alignment = alignment;
        this.boxConstraints = boxConstraints;
    }

    public override Widget Build(BuildContext context)
    {
        var w = child;

        if (alignment != null)
            w = new Align(w, alignment.Value);

        if (boxConstraints != null)
            w = new BoxConstrained(w, boxConstraints.Value);

        if (margin != null)
            w = new Padding(w, margin.Value);

        if (flexMargin != null)
            w = new FlexPadding(w, flexMargin.Value);

        if (backgroundColor != null)
            w = new Stack(new List<Widget>
            {
                new DeferSize(
                    new ColoredBox(backgroundColor.Value)),
                w
            });

        if (padding != null)
            w = new Padding(w, padding.Value);

        if (flexPadding != null)
            w = new FlexPadding(w, flexPadding.Value);

        return w;
    }
}