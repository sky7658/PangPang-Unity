using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Board
{
    public class Score
    {
        public float curScore { get; private set; }
        public float curCombo { get; private set; }

        public Score()
        {
            curScore = 0;
            curCombo = 0;
        }

        public void ScoreUpdate(int addScore)
        {
            curScore += addScore + addScore * curCombo * 0.1f;
        }

        public void ComboUpdate()
        {
            curCombo += 1;
        }

        public void ResetCombo(float curTime, float resetTime = 5f)
        {
            if(curTime > resetTime) curCombo = 0;
        }
    }
}

