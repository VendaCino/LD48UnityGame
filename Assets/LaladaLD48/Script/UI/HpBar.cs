using System;
using UnityEngine;

namespace LaladaLD48.Script.UI
{
    public class HpBar : MonoBehaviour
    {
        public RectTransform innerBar;
        public float _nowValue=1f;
        [SerializeField]
        private float _changeSpeed=1f;
        private void Update()
        {
            var ss = innerBar.localScale;
            var value = _nowValue;
            if (value < 0) value = 0;
            if (value > 1) value = 1;
            var dx = value - ss.x;
            if (Math.Abs(dx) < _changeSpeed * Time.deltaTime) ss.x = value;
            else ss.x = ss.x + _changeSpeed * Time.deltaTime * Math.Sign(dx);
            innerBar.localScale = ss;
        }
    }
}