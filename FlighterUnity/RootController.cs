using Flighter;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlighterUnity
{
    public class RootController : MonoBehaviour
    {
        TreeController tree;

        public event Action TornDown;

        public void SetRoot(TreeController treeController)
        {
            tree = treeController;
        }

        public void TearDown()
        {
            tree?.Dispose();
            tree = null;

            Destroy(gameObject);
            TornDown?.Invoke();
        }

        void Update()
        {
            tree.Draw();
        }
    }
}
