using System;
using LaladaLD48.Script.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LaladaLD48.Script
{
    public class GameManager : MonoBehaviour
    {
        public static Player globalPlayer;
        [SerializeField]
        private Player _player;

        public HpBar _HpBar;
        public HpBar _MpBar;
        public Text clickToResetText;

        private bool _gameOver = false;
        private bool _clickToReset = false;

        private int _mpCount;

        public Transform Monsters;
        public GameObject Door;

        private void Start()
        {
            globalPlayer = PP;
        }

        public Player PP
        {
            get
            {
                if (_player == null) _player = FindObjectOfType<Player>();
                return _player; 
            }
        }

        public float HpRate => PP.HpRate;

        private void Update()
        {
            Door.gameObject.SetActive(Monsters==null || Monsters.childCount == 0);
            _HpBar._nowValue = HpRate;
            _MpBar._nowValue = PP.MpCount / 100.0f;
            _MpBar.gameObject.SetActive(_MpBar._nowValue>0);
            if (PP.HpRate <= 0 && !_gameOver)
            {
                PP.SetDead();
                _gameOver = true;
                Invoke("_showClickToRest",2f);
            }

            if (_clickToReset)
            {
                if(Input.anyKeyDown) _resetScene();
            }
            
            
        }

        private void _showClickToRest()
        {
            clickToResetText.gameObject.SetActive(true);
            _clickToReset = true;
        }
        
        private void _resetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}