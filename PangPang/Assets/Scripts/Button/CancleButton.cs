using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PangPang.ButtonEvent
{
    public class CancleButton : ButtonEvent
    {
        private Button cancleBtn;

        private void Awake()
        {
            cancleBtn = this.GetComponent<Button>();
            cancleBtn.onClick.AddListener(UnActiveWindow);
        }
    }

}
