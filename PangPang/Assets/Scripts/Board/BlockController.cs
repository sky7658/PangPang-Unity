using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PangPang.Action;
using PangPang.Quest;

namespace PangPang.Board
{
    public class BlockController : MonoBehaviour
    {
        Board board;
        Score score;
        Hint hint;
        Utils.InputManager m_Input;
        public Score getScoreInfo { get { return score; } }

        void Start()
        {
            board = new Board();
            score = new Score();
            hint = new Hint();
            m_Input = new Utils.InputManager();

            board.InitBoard();
        }

        private Vector2 arrowVector, mousePos;
        private RaycastHit2D b1, b2;
        void MouseDrag()
        {
            if (m_Input.isTouchDown)
            {
                arrowVector = Vector2.zero;

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                arrowVector -= mousePos;

                b1 = Physics2D.Raycast(mousePos, transform.forward);
            }
            if (m_Input.isTouchUp)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                arrowVector += mousePos;

                arrowVector = ArrowCal(arrowVector);

                if (!b1) return;

                Vector2 check = new Vector2(b1.transform.position.x + (arrowVector.x * 1.3f), b1.transform.position.y + (arrowVector.y * 1.3f));
                b2 = Physics2D.Raycast(check, Vector2.zero);

                if (b2)
                {
                    StartCoroutine(ExecuteSwapAction(b1.transform.GetComponent<Block>(), arrowVector));
                }
            }
        }

        private Vector2 ArrowCal(Vector2 vc)
        {
            Vector2 newVc = Vector2.zero;
            if (Mathf.Abs(vc.x) < Mathf.Abs(vc.y))
            {
                newVc.x = 0;
                if (vc.y > 0) newVc.y = 1f;
                else newVc.y = -1f;
            }
            else
            {
                newVc.y = 0;
                if (vc.x > 0) newVc.x = 1f;
                else newVc.x = -1f;
            }
            return newVc;
        }

        // 블럭 스왑 애니메이션 명령
        private IEnumerator ExecuteSwapAction(Block curBlock, Vector2 swipeD)
        {
            Block targetBlock = board.blocks[curBlock.myPos.y + -(int)swipeD.y, curBlock.myPos.x + (int)swipeD.x];
            Block baseBlock = board.blocks[curBlock.myPos.y, curBlock.myPos.x];

            (int y, int x) baseXY = curBlock.myPos;
            (int y, int x) targetXY = (curBlock.myPos.y + -(int)swipeD.y, curBlock.myPos.x + (int)swipeD.x);

            Vector2 targetPos = targetBlock.transform.position;
            Vector2 basePos = baseBlock.transform.position;

            if (baseBlock.blockState != BlockState.IDLE || targetBlock.blockState != BlockState.IDLE) yield break;

            baseBlock.ChangeState(BlockState.SWAP);
            targetBlock.ChangeState(BlockState.SWAP);

            // 스왑 액션
            StartCoroutine(BlockAction.MoveToAction(baseBlock, targetPos, AnimationLength.BLOCK_SWAP));
            StartCoroutine(BlockAction.MoveToAction(targetBlock, basePos, AnimationLength.BLOCK_SWAP));

            yield return new WaitForSeconds(AnimationLength.BLOCK_SWAP);

            board.SwapBlock(baseXY, targetXY);

            List<Block> matchedBlockList = new List<Block>();

            bool targetMatch = board.IsMatch_Part(board.blocks[baseXY.y, baseXY.x], matchedBlockList); 
            bool baseMatch = board.IsMatch_Part(board.blocks[targetXY.y, targetXY.x], matchedBlockList);

            if (SpecialSwap(board.blocks[targetXY.y, targetXY.x], board.blocks[baseXY.y, baseXY.x]) || baseMatch || targetMatch)
            {
                if (hint.isHintTurnOn)
                {
                    ActiveHint(false);
                }
                // "PANG"해야하는 블럭이 없을때까지 보드 체크
                do
                {
                    StartCoroutine(BlockDropAction());
                    yield return new WaitForSeconds(AnimationLength.BLOCK_DROP + AnimationLength.BLOCK_PANG);
                } while (board.IsMatch_All());

                baseBlock.ChangeState(BlockState.IDLE);
                targetBlock.ChangeState(BlockState.IDLE);

                if (!board.AIMatch())
                {
                    board.InitBoard();
                }

                hint.ResetHintInfo();

                yield break;
            }

            StartCoroutine(BlockAction.MoveToAction(baseBlock, basePos, AnimationLength.BLOCK_SWAP));
            StartCoroutine(BlockAction.MoveToAction(targetBlock, targetPos, AnimationLength.BLOCK_SWAP));

            yield return new WaitForSeconds(AnimationLength.BLOCK_SWAP);

            board.SwapBlock(targetXY, baseXY);

            baseBlock.ChangeState(BlockState.IDLE);
            targetBlock.ChangeState(BlockState.IDLE);

            yield break;
        }

        // "PANG"을 한 이후 드롭을 하는 액션
        private IEnumerator BlockDropAction(/*HashSet<(int y, int x)> matches*/)
        {
            //특수 블럭들 먼저 처리
            int blockMatchType = (int)MatchTypeMethod.MaxType();

            while(blockMatchType > 3)
            {
                foreach (var block in board.blocks)
                {
                    if (block == null) continue;

                    if ((MatchType)blockMatchType > MatchType.FIVE && block.match == (MatchType)blockMatchType)
                    {
                        ProcessMatchBlock(block);
                    }
                    else if ((MatchType)blockMatchType > MatchType.THREE && block.match == (MatchType)blockMatchType && block.transform.position.Equals(block.specialMoveTarget))
                    {
                        ProcessMatchBlock(block);
                    }
                }
                blockMatchType--;
            }

            // 특수 블럭 처리 이후 일반 블럭 처리
            foreach (var block in board.blocks)
            {
                if (block == null) continue;

                int x = block.myPos.x;
                int y = block.myPos.y;

                List<Block> blockList = new List<Block>()
                {
                    block
                };

                if (block.match.Equals(MatchType.THREE))
                {
                    score.ComboUpdate();
                    board.ComposeMatchBlock(block.match, block, blockList);
                    foreach(var matchBlock in blockList)
                    {
                        BlockPangType(matchBlock.myPos.x, matchBlock.myPos.y, null);
                    }
                    // 콤보 업데이트
                }
            }

            yield return new WaitForSeconds(AnimationLength.BLOCK_PANG);

            board.DownBlock();

            foreach (var block in board.blocks)
            {
                if (block.dropCount > 0)
                {
                    Vector2 to = BaseInfo.SetBlockPos(block.myPos.y, block.myPos.x, 0);
                    StartCoroutine(BlockAction.MoveToAction(block, to, AnimationLength.BLOCK_DROP));
                }
            }
        }

        private void ProcessMatchBlock(Block block)
        {
            if (block == null) return;

            List<Block> specialBlockList = new List<Block>();

            board.ComposeMatchBlock(block.match, block, specialBlockList);

            // 콤보 업데이트
            score.ComboUpdate();

            List<Block> exceptBlockList = new List<Block>(specialBlockList)
            {
                block // 예외 블럭은 본인 자신도 포함시켜주기
            };

            foreach (var specialBlock in specialBlockList)
            {
                score.ScoreUpdate(BaseInfo.baseScore);

                int sx = specialBlock.myPos.x;
                int sy = specialBlock.myPos.y;
                if (specialBlock.skill >= BlockSkill.LINE) BlockPangType(sx, sy, exceptBlockList);
                StartCoroutine(BlockAction.SpecialBlockAction(specialBlock, specialBlock.specialMoveTarget, AnimationLength.BLOCK_PANG));
                board.blocks[sy, sx].match = MatchType.NONE;
                board.blocks[sy, sx] = null;
            }

            score.ScoreUpdate(BaseInfo.baseScore);
            block.UpdateBlockSkill();
            block.mySpr.color = Color.black;
            block.match = MatchType.NONE;
        }

        private bool IsExceptBlock(int x, int y, List<Block> exceptBlockList) // 현재 블럭이 예외 처리 블럭인지 확인
        {
            if (exceptBlockList == null) return false;
            foreach (var exceptBlock in exceptBlockList)
                if (exceptBlock.myPos.Equals((y, x))) return true;
            return false;
        }

        // 위치에 있는 블록 "PANG"
        private void BlockPang(int x, int y)
        {
            if (board.blocks[y, x] == null) return;

            score.ScoreUpdate(BaseInfo.baseScore);

            board.blocks[y, x].ChangeState(BlockState.PANG);
            board.blocks[y, x].Explosion(x, y);
            StartCoroutine(BlockAction.BlockPangAction(board.blocks[y, x], 0.3f, 1.5f));
            board.blocks[y, x] = null;
        }

        private void LinePang(int x, int y, List<Block> exceptBlockList) // 기준 블럭의 위치와 예외 블럭 리스트
        {
            // LINE 블럭 기점으로 가로 세로 줄 PANG
            if (board.blocks[y, x] != null && !IsExceptBlock(x, y, exceptBlockList)) BlockPang(x, y);

            for (int col = x + 1; col < board.boardMaxSize; col++)
                if (!IsExceptBlock(col, y, exceptBlockList)) BlockPangType(col, y, exceptBlockList);
            for (int col = x - 1; col >= 0; col--)
                if (!IsExceptBlock(col, y, exceptBlockList)) BlockPangType(col, y, exceptBlockList);
            for (int row = y + 1; row < board.boardMaxSize; row++)
                if (!IsExceptBlock(x, row, exceptBlockList)) BlockPangType(x, row, exceptBlockList);
            for (int row = y - 1; row >= 0; row--)
                if (!IsExceptBlock(x, row, exceptBlockList)) BlockPangType(x, row, exceptBlockList);
        }

        private void AroundPang(int x, int y, int interval, List<Block> exceptBlockList)
        {
            // AROUND 블럭 기점으로 마름모 모양으로 PANG
            int startX = x - interval;
            int endX = x + interval;
            int startY = y - interval;
            int endY = y + interval;

            if (board.blocks[y, x] != null && !IsExceptBlock(x, y, exceptBlockList)) BlockPang(x, y);

            int count = interval;
            for (int row = startY; row <= endY; row++)
            {
                if (row >= 0 && row < board.boardMaxSize)
                {
                    for (int col = startX + count; col <= endX - count; col++)
                    {
                        if (col < 0 || col >= board.boardMaxSize || (x == col && y == row) || IsExceptBlock(col, row, exceptBlockList)) continue;
                        BlockPangType(col, row, exceptBlockList);
                    }
                }
                if (row < startY + interval) count--;
                else count++;
            }
        }

        // 블록 스킬에 따른 "PANG" 범위
        private void BlockPangType(int x, int y, List<Block> exceptBlockList)
        {
            if (board.blocks[y, x] == null) return;

            switch (board.blocks[y, x].skill)
            {
                case BlockSkill.NONE:
                    // 자기 자신만 PANG
                    BlockPang(x, y);
                    break;
                case BlockSkill.LINE:
                    LinePang(x, y, exceptBlockList);
                    break;
                case BlockSkill.AROUND:
                    AroundPang(x, y, 2, exceptBlockList);
                    break;
            }
        }

        private bool SpecialSwap(Block baseBlock, Block targetBlock)
        {
            int type = (int)baseBlock.skill + (int)targetBlock.skill;

            if (!board.IsSpecialSwap(type)) return false;

            // 콤보 업데이트
            score.ComboUpdate();

            if (type == 5)
            {
                BlockPangType(baseBlock.myPos.x, baseBlock.myPos.y, null);
                BlockPangType(targetBlock.myPos.x, targetBlock.myPos.y, null);
            }
            else if (type == 8)
            {
                LinePang(baseBlock.myPos.x, baseBlock.myPos.y, null);
                LinePang(targetBlock.myPos.x, targetBlock.myPos.y, null);
            }
            else if (type == 10)
            {
                AroundPang(baseBlock.myPos.x, baseBlock.myPos.y, 3, null);
            }

            return true;
        }

        private void ActiveHint(bool turn)
        {
            if (turn)
            {
                board.hintBlock[0].StartHintAction();
                board.hintBlock[1].StartHintAction();
            }
            else
            {
                board.hintBlock[0].StopHintAction();
                board.hintBlock[1].StopHintAction();
            }
        }

        void Update()
        {
            MouseDrag();

            if (hint.IsHint()) ActiveHint(true);

            score.ResetCombo();
        }
    }
}

