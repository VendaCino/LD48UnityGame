using System;
using UnityEngine;

namespace LaladaLD48.Script.Common
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        protected Animator _animator;
        public float moveSpeed = 1f;
        public float jumpSpeed = 1f;
        public bool cantMove = false;
        private bool _canJump = true;
        private float _originScaleX = 1;
        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Jump = Animator.StringToHash("jump");
        public bool DirRight => this.transform.localScale.x*_originScaleX < 0;
        public bool OnGround => _canJump;

        private float _canJumpTimer = 0f;

        private void Awake()
        {
            _originScaleX = transform.localScale.x;
            _animator = GetComponent<Animator>();
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            
            // Debug.Log($"[{name}]:{other.gameObject.tag} is {other.relativeVelocity}");
            if (other.gameObject.tag == "Ground")
            {
                if (other.relativeVelocity.y > 0.001) _canJump = true;
                if(_animator) _animator.SetBool(Jump,false);
            }
        }
        
        protected void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.tag == "Ground")
            {
                if (!_canJump)
                {
                    if (_canJumpTimer > 0) _canJumpTimer -= Time.deltaTime;
                    if (_canJumpTimer <= 0)
                    {
                        _canJumpTimer = 5f;
                        _canJump = true;
                    }
                }
                if (other.relativeVelocity.y > 0.001) _canJump = true;
            }
        }

        public void MoveLeft()
        {
            // if(sr) sr.flipX = false;
            if(_animator) _animator.SetBool(Walk,true);
            var ss = transform.localScale;
            ss.x = _originScaleX * 1;
            transform.localScale = ss;
            _move(-1);
        }
        
        public void MoveRight()
        {
            // if(sr) sr.flipX = true;
            if(_animator) _animator.SetBool(Walk,true);
            var ss = transform.localScale;
            ss.x = _originScaleX * -1;
            transform.localScale = ss;
            _move(1);
        }
        
        public void MoveStop()
        {
            if(_animator) _animator.SetBool(Walk,false);
            _move(0);
        }
        
        public void MoveJump()
        {
            if(!_canJump) return;
            _canJump = false;
            if(_animator) _animator.SetBool(Jump,true);
            var v2 = rb2d.velocity;
            v2.y = jumpSpeed;
            if (!cantMove) rb2d.velocity = v2;
        }
        
        public void MoveTowardAndAutoJump(Vector2 dir)
        {
            if(dir.y>0.1f) MoveJump();
            if(dir.x<-0.01f) MoveLeft();
            else if(dir.x>0.01f) MoveRight();
            else MoveStop();
            if(!_canJump) return;
        }
        
        public void MoveToAndAutoJump(Vector2 target)
        {
            MoveTowardAndAutoJump(target-this.rb2d.position);
        }

        private void _move(float x)
        {
            var v2 = rb2d.velocity;
            v2.x = x*moveSpeed;
            if (!cantMove) rb2d.velocity = v2;
        }
        
        
        
        
        protected Rigidbody2D rb2d
        {
            get
            {
                if (_rigidbody2D == null) _rigidbody2D = GetComponent<Rigidbody2D>();
                return _rigidbody2D; 
            }
        }

        protected SpriteRenderer sr
        {
            get
            {
                if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
                return _spriteRenderer; 
            }
        }
    }
}
