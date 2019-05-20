using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controller
{
    class AntennaIconManager : MonoBehaviour
    {
        private Image[] _bars;

        public void SetBars(Image[] Bars)
        {
            _bars = Bars;
        }

        public void UpdateIcon(int powerLeft)
        {
            for (int i = 0; i < powerLeft+1; i++) _bars[i].enabled = true;
            for (int i = powerLeft+1; i < _bars.Length; i++) _bars[i].enabled = false;
        }
    }
}
