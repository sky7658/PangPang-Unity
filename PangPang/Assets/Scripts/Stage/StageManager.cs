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
        }
            

        void Update()
        {
            BaseInfo.gameTime += Time.smoothDeltaTime;
            timerBar.fillAmount = (endtimer - BaseInfo.gameTime) / endtimer;
        }
    }

}
