using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PangPang.Quest;

namespace PangPang.Board
{
    public class Block : MonoBehaviour
    {
        public SpriteRenderer mySpr;
        public Block_Type myType;
        public BlockSkill skill;
        public BlockState blockState;
        public Vector2 specialMoveTarget;
        public (int y, int x) myPos;
        public int dropCount { get; set; }  // Drop È½¼ö

        public MatchType match;

        private void Awake()
        {
            mySpr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        public void UpdateMatchType(MatchType matchType)
        {
            if (match == MatchType.NONE)
                match = matchType;
            else match = match.Add(matchType);
        }
        public void UpdateMoveTarget(Vector2 moveTarget)
        {
            specialMoveTarget = moveTarget;
        }
        public void UpdateBlockSkill()
        {
            switch(match)
            {
                case MatchType.NONE:
                    skill = BlockSkill.NONE;
                    break;
                case MatchType.FOUR:
                    skill = BlockSkill.LINE;
                    break;
                case MatchType.FIVE:
                    skill = BlockSkill.AROUND;
                    break;
                default:
                    skill = BlockSkill.AROUND;
                    break;
            }
        }

        public void ChangeState(BlockState _state)
        {
            if (blockState == _state) return;
            blockState = _state;
        }

        public void InitBlock(Block_Type _type, (int y, int x) _pos, int dropCount)
        {
            myPos = _pos;
            myType = _type;
            mySpr.color = Color.white;


            match = MatchType.NONE;
            UpdateBlockSkill();
            ChangeState(BlockState.IDLE);

            this.transform.localScale = new Vector3(1, 1, 1);
            this.dropCount = dropCount;

            transform.position = BaseInfo.SetBlockPos(_pos.y, _pos.x, dropCount);
        }
    }
}