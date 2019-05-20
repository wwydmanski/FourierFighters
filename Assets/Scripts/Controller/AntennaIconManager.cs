using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    class AntennaIconManager
    {
        private GameObject BaseIcon;

        public AntennaIconManager(GameObject baseIcon)
        {
            BaseIcon = baseIcon;
        }

        public void UpdateIcon(int powerLeft)
        {
            Texture2D wave = new Texture2D(100, 100);
            wave.SetPixels(0, 0, 100, 100, new[] {Color.red});
            wave.Apply();
        }
    }
}
