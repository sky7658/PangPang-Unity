using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PangPang.ButtonEvent
{
    public class OptionButton : ButtonEvent
    {
        public bool setOptionCase;
        private Button optionBtn;

        private void Awake()
        {
            optionBtn = this.GetComponent<Button>();
            optionBtn.onClick.AddListener(() => ActiveWindow(setOptionCase));
            optionWindow = GameObject.Find("SoundManager").transform.GetChild(2).transform.GetChild(0).gameObject;
        }
    }
}

