using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flighter.Core;
using UnityEngine;

namespace skratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var textAlignEnum = Enum.GetValues(typeof(TextAlign)).GetEnumerator();
            var textAnchorEnum = Enum.GetValues(typeof(TextAnchor)).GetEnumerator();

            while (textAlignEnum.MoveNext() && textAnchorEnum.MoveNext())
            {
                Console.WriteLine("{ TextAnchor." 
                    + textAnchorEnum.Current.ToString() 
                    + ", TextAlign." 
                    + textAlignEnum.Current.ToString() + " },");
            }

            Console.ReadKey();
        }
    }
}
