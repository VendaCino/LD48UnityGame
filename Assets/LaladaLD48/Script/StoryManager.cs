using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaladaLD48.Script
{
    public class StoryManager : MonoBehaviour
    {

        public bool isBadEnd = false;
        public bool isLastEnd = false;
        
        public void OnEnd()
        {
            if(isLastEnd) SceneManager.LoadScene("menu2");
            else if (isBadEnd) SceneManager.LoadScene("scene3-1");
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }

        public void ScreenChange()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
        public void StartGame()
        {
            SceneManager.LoadScene("scene1-1");
        }

    }
}