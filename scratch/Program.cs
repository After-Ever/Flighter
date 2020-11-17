using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Flighter.Input;

namespace scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                Console.WriteLine(
                    "{ UnityKeyCode." + keyCode.ToString()
                    + ", FlighterKeyCode." + keyCode.ToString()
                    + " },"
                    );
            }

            Console.ReadKey();
        }
    }
}
