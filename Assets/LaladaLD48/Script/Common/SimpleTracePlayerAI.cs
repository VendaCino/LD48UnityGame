using System;
using UnityEngine;

namespace LaladaLD48.Script.Common
{
    [RequireComponent(typeof(Movable))]
    public class SimpleTracePlayerAI : MonoBehaviour
    {
        [SerializeField]
        private Player _player;

        private Movable _movable;
        
        public bool normalMove = true;
        public float moveWidth = 4f;
        public bool moveCloseToPlayer = true;
        public float seeArea = 2f;
        public float homeArea = 10f;
        public bool jumpWithPlayer = true;
        public float jumpArea = 1f;

        private Vector3 _originPosition;
        private Vector3 _normalMoveTarget;
        private bool _turn = false;
        
        public Player Target
        {
            get
            {
                if (_player == null) _player = FindObjectOfType<Player>();
                if (_player == null) _player = GameManager.globalPlayer;
                return _player; 
            }
        }
        
        public Movable me
        {
            get
            {
                if (_movable == null) _movable = GetComponent<Movable>();
                return _movable; 
            }
        }

        private void Start()
        {
            _originPosition = transform.position;
            _updateMoveTarget();
        }

        private void _updateMoveTarget()
        {
            _turn = !_turn;
            _normalMoveTarget = _originPosition + (_turn ? Vector3.left : Vector3.right) * moveWidth;
        }

        private void Update()
        {
            var dis = _disWithPlayer();
            var dir = _dirWithPlayer();
            if (dis < jumpArea && jumpWithPlayer && dir.y > 0.01f) 
                me.MoveJump();
            
            bool leaveHomeToFar = Vector2.Distance(_originPosition, this.transform.position) > homeArea;
            if (dis < seeArea && moveCloseToPlayer && !leaveHomeToFar) 
                me.MoveTowardAndAutoJump(dir);
            else
            {
                if(normalMove) me.MoveToAndAutoJump(_normalMoveTarget);    
            }
            
            bool done = Vector2.Distance(_normalMoveTarget, this.transform.position) < 0.1f;
            if (done) _updateMoveTarget();
        }

        private float _disWithPlayer()
        {
            return Vector2.Distance(Target.transform.position, this.transform.position);
        }
        
        private Vector2 _dirWithPlayer()
        {
            return Target.transform.position - transform.position;
        }
    }
}