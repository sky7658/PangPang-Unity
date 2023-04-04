using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Board
{
    public class Score
    {
        public float curScore { get; private set; }
        public float curCombo { get; private set; }
        float pastTime;
        float resetTime;

        public Score()
        {
            curScore = 0;
            curCombo = 0;
            pastTime = 0f;
            resetTime = BaseInfo.comboResetTime;
        }

        public void ScoreUpdate(int addScore)
        {
            curScore += addScore + addScore * curCombo * 0.1f;
        }

        public void ComboUpdate()
        {
            curCombo += 1;
            pastTime = BaseInfo.gameTime;
        }

        public void ResetCombo()
        {
            if(curCombo < 1)
            {
                pastTime = BaseInfo.gameTime;
                return;
            }

            if (BaseInfo.gameTime - pastTime > resetTime)
            {
                pastTime = BaseInfo.gameTime;
                curCombo = 0;
            }
        }
    }
}

