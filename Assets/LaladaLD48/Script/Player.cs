using System;
using System.Collections.Generic;
using LaladaLD48.Script.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaladaLD48.Script
{
    public class Player : Movable
    {
        public float moveXMin = 0.01f;
        public GameObject BloodPrefab;
        public GameObject BloodPrefabRight;
        
        public GameObject BloodPrefabOnHit;
        public GameObject BloodPrefabOnCi;
        
        public GameObject BloodPrefabMp;
        private BloodHitable _bloodHitable;
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Sleep = Animator.StringToHash("sleep");
        private static readonly int Dead = Animator.StringToHash("dead");
        public bool Stop = false;

        [SerializeField]
        private float _beHitColdTime = 1f;
        private float _beHitColdTimer = 0f;
        [SerializeField]
        private float _beHitSpeedY = 3;
        [SerializeField]
        private float _beHitSpeedX = 3;

        private int _mpCount = 0;

        public int MpAddCount = 1;
        public int MpCount => _mpCount;

        public bool isLastScene = false;
        private int _attackCount = 0;

        private static readonly int BeHit = Animator.StringToHash("beHit");
        public Animator Anim => _animator;

        public float HpRate => (_bloodHitable.Hp + 0.0f) / _bloodHitable.MaxHp;
        private float _mpTimer = 0.03f;
        private void Start()
        {
            _bloodHitable = GetComponent<BloodHitable>();
            MoveRight();
        }

        private void Update()
        {
            if(Stop) return;
            if (_mpTimer > 0) _mpTimer -= Time.deltaTime;
            if (_beHitColdTimer > 0) _beHitColdTimer -= Time.deltaTime;
            if(_beHitColdTimer<=0 && _animator.GetBool(BeHit)) _animator.SetBool(BeHit,false);
            var ml = Input.GetKey(KeyCode.LeftArrow);
            var mr = Input.GetKey(KeyCode.RightArrow);
            var move = (ml && !mr) || (!ml && mr);
            if (move)
            {
                if (mr) MoveRight();
                else MoveLeft();
            }
            else MoveStop();

            if (Input.GetKeyDown("z")) MoveJump();
            if (Input.GetKeyDown("x")) _attack();
        }

        private void _attack()
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) return;
            _mpCount = 0;
            _attackCount++;
            if(_animator) _animator.SetTrigger(Attack);
            Invoke("_makeNormalBlood",0.3f);
            _bloodHitable.Hp -= 1;
        }


        private void _makeNormalBlood()
        {
            _makeObj(DirRight?BloodPrefabRight:BloodPrefab);
        }
        
        private void _makeObj(GameObject prefab)
        {
            var obj = Instantiate(prefab);
            obj.transform.position = transform.position;
        }
        
        private void _makeOnHitBlood()
        {
            _makeObj(BloodPrefabOnHit);
        }
        
        private void _makeOnHitCi()
        {
            _makeObj(BloodPrefabOnCi);
        }


        private HashSet<string> tags = new HashSet<string>() {"Monster", "Ci","Door"};
        protected void OnCollisionEnter2D(Collision2D other)
        {
            base.OnCollisionEnter2D(other);
            _checkCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            base.OnCollisionStay2D(other);
            _checkCollision(other);
        }

        private void _checkCollision(Collision2D other)
        {
            var t = other.gameObject.tag;
            if (!tags.Contains(t)) return;

            if (_beHitColdTimer > 0) return;
            _beHitColdTimer = _beHitColdTime;
            if (other.gameObject.CompareTag("Monster"))
            {
                _onBeHit(other, 4);
                Invoke("_makeOnHitBlood", 0.3f);
            }
            else if (other.gameObject.CompareTag("Ci"))
            {
                _onBeHit(other, 1);
                Invoke("_makeOnHitCi", 0.1f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Door") && !Stop)
            {
                Stop = true;
                MoveStop();
                _beHitColdTimer = 100;
                _animator.SetTrigger(Sleep);
                Invoke("_nextScene", 2f);
            }
        }

        private void _onBeHit(Collision2D other,int dmg)
        {
            if(_animator) _animator.SetBool(BeHit,true);
            cantMove = true;
            var v =  transform.position - other.transform.position;
            v.x = Math.Sign(v.x) * _beHitSpeedX;
            v.y = 1 * _beHitSpeedY;
            rb2d.velocity = v;
            _bloodHitable.Hp -= dmg;
            Invoke("_resetMoveable",1f);
        }

        void _resetMoveable()
        {
            cantMove = false;
        }

        void _nextScene()
        {
            if(!isLastScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            else
            {
                if(_attackCount==0)SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+3);
                else if(_attackCount>=9)SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
                else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (_mpTimer > 0) return;
            _mpTimer = 0.03f;
            if (other.tag == "Light" && !Stop)
            {
                _mpCount+=MpAddCount;
                if (_mpCount >= 100)
                {
                    _mpCount = 0;
                    _makeObj(BloodPrefabMp);
                }
            }
        }

        public void SetDead()
        {
            Stop = true;
            Anim.SetTrigger(Dead);
            Anim.SetBool(BeHit,false);
        }
    }
}