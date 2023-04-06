using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Board
{
    public class Hint
    {
        private float pastTime;
        public bool isHintTurnOn { get; private set; }
        public Hint()
        {
            InitHint();
        }

        public void InitHint()
        {
            pastTime = 0f;
            isHintTurnOn = false;
        }

        public void ResetHintInfo()
        {
            isHintTurnOn = false;
            pastTime = BaseInfo.gameTime;
        }
        public bool IsHint()
        {
            if (BaseInfo.gameTime - pastTime > BaseInfo.activeHintTime && !isHintTurnOn)
            {
                isHintTurnOn = true;
                return true;
            }
            return false;
        }
    }
}