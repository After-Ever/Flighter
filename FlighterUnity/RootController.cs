using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class RootController : MonoBehaviour
    {
        Root root;

        public void SetRoot(Root root)
        {
            this.root = root;
        }

        public void TearDown()
        {
            root?.Dispose();
            root = null;

            Destroy(gameObject);
        }

        void Update()
        {
            root?.Update();
        }
    }
}
