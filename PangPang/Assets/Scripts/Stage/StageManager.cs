using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PangPang.Stage
{
    public class StageManager : MonoBehaviour
    {
        public float endtimer;

        [SerializeField] private Image timerImage;
        private Image timerBar;

        private void Awake()
        {
            timerBar = timerImage.GetComponent<Image>();
            BaseInfo.gameTime = 0f;
        }
            

        void Update()
        {
            BaseInfo.gameTime += Time.smoothDeltaTime;
            timerBar.fillAmount = (endtimer - BaseInfo.gameTime) / endtimer;
            if (endtimer - BaseInfo.gameTime < 0f) GameState.GameOver();
        }
    }

    public static class GameState
    {
        public static void GamePause()
        {
            Time.timeScale = 0f;
        }

        public static void GameContinue()
        {
            if (Time.timeScale > 0f) return;
            Time.timeScale = 1f;
        }

        public static void GameOver()
        {
            // 메인 메뉴로 이동
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
