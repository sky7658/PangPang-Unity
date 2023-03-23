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

        void Start()
        {
            board = new Board();
            board.InitBoard();
        }

        private Vector2 arrowVector, mousePos;
        private RaycastHit2D b1, b2;
        void MouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                arrowVector = Vector2.zero;

                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                arrowVector -= mousePos;

                b1 = Physics2D.Raycast(mousePos, transform.forward);
            }
            if (Input.GetMouseButtonUp(0))
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                arrowVector += mousePos;

                arrowVector = ArrowCal(arrowVector);

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

            Vector2 targetPos = targetBlock.transform.position;
            Vector2 basePos = baseBlock.transform.position;

            if (baseBlock.blockState != BlockState.IDLE || targetBlock.blockState != BlockState.IDLE) yield break;

            baseBlock.ChangeState(BlockState.SWAP);
            targetBlock.ChangeState(BlockState.SWAP);

            // 스왑 액션
            StartCoroutine(BlockAction.MoveToAction(baseBlock, targetPos, AnimationLength.BLOCK_SWAP));
            StartCoroutine(BlockAction.MoveToAction(targetBlock, basePos, AnimationLength.BLOCK_SWAP));

            yield return new WaitForSeconds(AnimationLength.BLOCK_SWAP);

            (int y, int x) baseXY = curBlock.myPos;
            (int y, int x) targetXY = (curBlock.myPos.y + -(int)swipeD.y, curBlock.myPos.x + (int)swipeD.x);

            board.blocks[targetXY.y, targetXY.x] = baseBlock;
            board.blocks[baseXY.y, baseXY.x] = targetBlock;

            List<Block> matchedBlockList = new List<Block>();

            board.blocks[baseXY.y, baseXY.x].myPos = baseXY;
            board.blocks[targetXY.y, targetXY.x].myPos = targetXY;

            bool targetMatch = board.IsMatch_Part(board.blocks[baseXY.y, baseXY.x], matchedBlockList); 
            bool baseMatch = board.IsMatch_Part(board.blocks[targetXY.y, targetXY.x], matchedBlockList);

            if (baseMatch || targetMatch || IsAroundBlockSwap(board.blocks[baseXY.y, baseXY.x], board.blocks[targetXY.y, targetXY.x]))
            {
                bool allMatch = true;

                while(allMatch)
                {
                    StartCoroutine(BlockDropAction());
                    yield return new WaitForSeconds(AnimationLength.BLOCK_DROP + AnimationLength.BLOCK_PANG);
                    allMatch = board.IsMatch_All_();
                }

                baseBlock.ChangeState(BlockState.IDLE);
                targetBlock.ChangeState(BlockState.IDLE);
                yield break;
            }

            StartCoroutine(BlockAction.MoveToAction(baseBlock, basePos, AnimationLength.BLOCK_SWAP));
            StartCoroutine(BlockAction.MoveToAction(targetBlock, targetPos, AnimationLength.BLOCK_SWAP));

            board.blocks[baseXY.y, baseXY.x].myPos = targetXY;
            board.blocks[targetXY.y, targetXY.x].myPos = baseXY;

            board.blocks[targetXY.y, targetXY.x] = targetBlock;
            board.blocks[baseXY.y, baseXY.x] = baseBlock;

            baseBlock.ChangeState(BlockState.IDLE);
            targetBlock.ChangeState(BlockState.IDLE);

            yield break;
        }

        private IEnumerator BlockDropAction(/*HashSet<(int y, int x)> matches*/)
        {
            //특수 블럭들 먼저 처리
            foreach (var block in board.blocks)
            {
                if(block == null) continue;

                if (block.match > MatchType.THREE && block.transform.position.Equals(block.specialMoveTarget))
                {
                    List<Block> specialBlockList = new List<Block>();
                    board.CreateSpecialBlock(block.match, block, specialBlockList);

                    foreach (var specialBlock in specialBlockList)
                    {
                        int sx = specialBlock.myPos.x;
                        int sy = specialBlock.myPos.y;
                        StartCoroutine(BlockAction.SpecialBlockAction(specialBlock, specialBlock.specialMoveTarget, AnimationLength.BLOCK_PANG));
                        board.blocks[sy, sx].match = MatchType.NONE;
                        board.blocks[sy, sx] = null;
                    }
                    block.UpdateBlockSkill();
                    block.mySpr.color = Color.black;
                    block.match = MatchType.NONE;
                }
            }

            // 특수 블럭 처리 이후 일반 블럭 처리
            foreach (var block in board.blocks)
            {
                if (block == null) continue;

                int x = block.myPos.x;
                int y = block.myPos.y;

                if (block.match.Equals(MatchType.THREE))
                {
                    BlockPangType(x, y);
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

        private void BlockPang(int x, int y)
        {
            if (board.blocks[y, x] == null) return;
            board.blocks[y, x].ChangeState(BlockState.PANG);
            StartCoroutine(BlockAction.BlockPangAction(board.blocks[y, x], 0.3f, 1.5f));
            board.blocks[y, x] = null;
        }

        private void BlockPangType(int x, int y)
        {
            if (board.blocks[y, x] == null) return;

            switch(board.blocks[y, x].skill)
            {
                case BlockSkill.NONE:
                    BlockPang(x, y);
                    break;
                case BlockSkill.LINE:
                    for (int col = x; col < board.boardMaxSize; col++)
                        BlockPang(col, y);
                    for (int col = x - 1; col >= 0; col--)
                        BlockPang(col, y);
                    for (int row = y + 1; row < board.boardMaxSize; row++)
                        BlockPang(x, row);
                    for (int row = y - 1; row >= 0; row--)
                        BlockPang(x, row);
                    break;
                case BlockSkill.AROUND:
                    int startX = x - 2;
                    int endX = x + 2;
                    int startY = y - 2;
                    int endY = y + 2;

                    if (startX < 0) startX = 0;
                    if (startY < 0) startY = 0;
                    if (endX >= board.boardMaxSize) endX = board.boardMaxSize - 1;
                    if (endY >= board.boardMaxSize) endY = board.boardMaxSize - 1;

                    for(int row = startY; row <= endY; row++)
                    {
                        for(int col = startX; col <= endX; col++)
                            BlockPang(col, row);
                    }

                    break;
            }
        }

        private bool IsAroundBlockSwap(Block block1, Block block2)
        {
            bool isAround = false;

            if(block1.skill == BlockSkill.AROUND)
            {
                isAround = true;
                BlockPang(block1.myPos.x, block1.myPos.y);
            }
            if(block2.skill == BlockSkill.AROUND)
            {
                isAround = true;
                BlockPangType(block2.myPos.x, block2.myPos.y);
            }

            return isAround;
        }

        void Update()
        {
            MouseDrag();
        }
    }
}

