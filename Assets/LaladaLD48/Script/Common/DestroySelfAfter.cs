using System;
using UnityEngine;

namespace LaladaLD48.Script.Common
{
    public class DestroySelfAfter : MonoBehaviour
    {
        public float time;
        private float _timer=0;

        private void Update()
        {
            _timer += Time.deltaTime;
            if(_timer>time) Destroy(this.gameObject);
        }
    }
}