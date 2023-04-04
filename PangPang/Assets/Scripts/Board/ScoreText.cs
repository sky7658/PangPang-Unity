using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PangPang.Board
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField]
        private GameObject refObj;
        private BlockController scoreInfo;
        private Text text;
        void Start()
        {
            text = GetComponent<Text>();
            scoreInfo = refObj.GetComponent<BlockController>();
        }

        void Update()
        {
            text.text = "Score : " + scoreInfo.getScoreInfo.curScore.ToString() + "\nCombo : " + scoreInfo.getScoreInfo.curCombo.ToString();
        }
    }

}
