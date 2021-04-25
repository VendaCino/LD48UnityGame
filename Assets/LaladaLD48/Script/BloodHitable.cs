using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaladaLD48.Script
{
    public class BloodHitable : MonoBehaviour
    {
        public int MaxHp = 3;
        public int Hp = 3;
        public bool deadOnHpZero = true;
        private HashSet<GameObject> _hasHit = new HashSet<GameObject>();
        protected Animator _animator;
        private static readonly int BeHit = Animator.StringToHash("beHit");

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if(_hasHit.Contains(other)) return;
            _hasHit.Add(other);
            Debug.Log($"[{name}]: Particle is {other.gameObject.tag} ");
            if (other.gameObject.tag == "Blood")
            {
                var dd = other.gameObject.GetComponent<BloodDamageable>();
                var dHp = dd != null ? dd.dHp : 1;
                if(dHp>0) if(_animator!=null) _animator.SetTrigger(BeHit); 
                Hp -= dHp;
            }

            if (Hp > MaxHp) Hp = MaxHp;
            if (Hp <= 0)
            {
                if(deadOnHpZero) Destroy(this.gameObject);
                else SendMessage("OnBloodDead", other);
            }
            
        }
    }
}