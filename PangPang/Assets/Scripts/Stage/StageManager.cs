using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PangPang.Stage
{
    public class StageManager : MonoBehaviour
    {
        public float endtimer;
        float timer;

        //
        [SerializeField] private Image timerImage;
        private Image timerBar;

        private void Awake()
        {
            timerBar = timerImage.GetComponent<Image>();
        }
            

        void Update()
        {
            timer += Time.smoothDeltaTime;
            timerBar.fillAmount = (endtimer - timer) / endtimer;
        }
    }

}
