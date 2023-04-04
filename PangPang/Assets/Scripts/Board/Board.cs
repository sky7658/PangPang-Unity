using System.Collections.Generic;
using PangPang.Quest;

namespace PangPang.Board
{
    public class Board
    {
        // 방향
        int[] directionX = { 0, 1, 0, -1 };
        int[] directionY = { 1, 0, -1, 0 };
        public int boardMaxSize { get; private set; }

        private Block[,] m_Blocks;
        public Block[,] blocks { get { return m_Blocks; } }
        public Block[] hintBlock;

        public Board()
        {
            boardMaxSize = 7;
            m_Blocks = new Block[boardMaxSize, boardMaxSize];
            hintBlock = new Block[2];
        }
        public void InitBoard()
        {
            //while(true)
            //{
            //    BoardSetting();
            //    if (AIMatch()) break;
            //    BoardReturn();
            //}
            do
            {
                BoardReturn();
                BoardSetting();
            } while (!AIMatch());
            //BoardSetting();
        }
        private void BoardSetting()
        {
            //Test용 코드
            //m_Blocks[0, 0] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 0].InitBlock(Block_Type.RED, (0, 0), 0);
            //m_Blocks[0, 0].skill = BlockSkill.LINE;

            //m_Blocks[1, 1] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[1, 1].InitBlock(Block_Type.RED, (1, 1), 0);

            //m_Blocks[0, 2] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 2].InitBlock(Block_Type.RED, (0, 2), 0);

            //m_Blocks[0, 3] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 3].InitBlock(Block_Type.RED, (0, 3), 0);


            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                {
                    //if (m_Blocks[y, x] != null) continue; // Test 용 코드
                    int block_type = UnityEngine.Random.Range(0, boardMaxSize);
                    m_Blocks[y, x] = BlockPool.instance.GetBlock((Block_Type)block_type);
                    m_Blocks[y, x].InitBlock((Block_Type)block_type, (y, x), 0);
                }
            }
        }
        public void BoardReturn()
        {
            if (m_Blocks[0, 0] == null)
            {
                return;
            }

            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                {
                    BlockPool.instance.ReturnBlock(m_Blocks[y, x]);
                    m_Blocks[y, x] = null;
                }
            }
        }
        private bool EscapeRange(int posX, int posY)
        {
            if (posX < 0 || posY < 0 || posX > boardMaxSize - 1 || posY > boardMaxSize - 1) return true;
            return false;
        }

        // 현재 보드에서 매치가 가능한 블럭이 있는지 확인한다.
        public bool AIMatch()
        {
            List<Block> list = new List<Block>();

            if (IsMatch_All()) return false;   // 이미 매치된 블럭이 있다면 재 구성

            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int _x = x + directionX[i];
                        int _y = y + directionY[i];

                        if (EscapeRange(_x, _y)) continue;

                        SwapBlock((y, x), (_y, _x));

                        if (IsSpecialSwap((int)m_Blocks[y, x].skill + (int)m_Blocks[_y, _x].skill) || IsMatch_Part(m_Blocks[_y, _x], list)) // 게임이 가능한 상황
                        {
                            SwapBlock((_y, _x), (y, x));
                            SetMatchBlockReset();   // 매치 확인을 하여 바꿔주었던 값을 초기값으로 바꾸기
                            for(int j = 0; j < boardMaxSize; j++)
                            {
                                for (int k = 0; k < boardMaxSize; k++)
                                {
                                    m_Blocks[j, k].transform.position = BaseInfo.SetBlockPos(j, k, 0);
                                }
                            }

                            hintBlock[0] = m_Blocks[y, x];
                            hintBlock[1] = m_Blocks[_y, _x];

                            return true;
                        }

                        SwapBlock((_y, _x), (y, x));
                    }
                }
            }

            return false;   // 게임이 불가능한 상황
        }

        public bool IsMatch_Part(Block baseBlock, List<Block> matchedBlockList)
        {
            bool bFound = false;
            if (baseBlock == null || baseBlock.match != MatchType.NONE) return false;

            matchedBlockList.Add(baseBlock);

            // 행의 오른쪽
            for (int x = baseBlock.myPos.x + 1; x < boardMaxSize; x++)
            {
                if (baseBlock.myType.Equals(m_Blocks[baseBlock.myPos.y, x].myType)) matchedBlockList.Add(m_Blocks[baseBlock.myPos.y, x]);
                else break;
            }
            // 행의 왼쪽
            for (int x = baseBlock.myPos.x - 1; x >= 0; x--)
            {
                if (baseBlock.myType.Equals(m_Blocks[baseBlock.myPos.y, x].myType)) matchedBlockList.Add(m_Blocks[baseBlock.myPos.y, x]);
                else break;
            }

            if (matchedBlockList.Count >= 3)
            {
                SetMatchBlock(matchedBlockList, baseBlock.transform.position);
                bFound = true;
            }

            matchedBlockList.Clear();
            matchedBlockList.Add(baseBlock);

            // 열의 아래쪽
            for (int y = baseBlock.myPos.y + 1; y < boardMaxSize; y++)
            {
                if (baseBlock.myType.Equals(m_Blocks[y, baseBlock.myPos.x].myType)) matchedBlockList.Add(m_Blocks[y, baseBlock.myPos.x]);
                else break;
            }
            // 열의 위쪽
            for (int y = baseBlock.myPos.y - 1; y >= 0; y--)
            {
                if (baseBlock.myType.Equals(m_Blocks[y, baseBlock.myPos.x].myType)) matchedBlockList.Add(m_Blocks[y, baseBlock.myPos.x]);
                else break;
            }

            if (matchedBlockList.Count >= 3)
            {
                SetMatchBlock(matchedBlockList, baseBlock.transform.position);
                bFound = true;
            }

            matchedBlockList.Clear();

            return bFound;
        }

        public bool IsMatch_All()
        {
            int count = 0;
            List<Block> matchedBlocks = new List<Block>();

            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                {
                    if (IsMatch_Part(m_Blocks[y, x], matchedBlocks))
                        count++;
                }
            }
            return count > 0;
        }

        private void SetMatchBlockReset()
        {
            foreach (var block in m_Blocks)
                block.match = MatchType.NONE;
        }
        private void SetMatchBlock(List<Block> matchedBlock, UnityEngine.Vector2 moveTarget)
        {
            matchedBlock.ForEach(block => block.UpdateMatchType((MatchType)matchedBlock.Count));
            matchedBlock.ForEach(block => block.UpdateMoveTarget(moveTarget));  // 기준 블럭이 문제
        }

        public void SwapBlock((int y, int x)p1, (int y, int x)p2)
        {
            Block baseBlock = m_Blocks[p1.y, p1.x];
            Block targetBlock = m_Blocks[p2.y, p2.x];

            m_Blocks[p1.y, p1.x] = targetBlock;
            m_Blocks[p2.y, p2.x] = baseBlock;

            m_Blocks[p1.y, p1.x].myPos = p1;
            m_Blocks[p2.y, p2.x].myPos = p2;
        }

        public void DownBlock()
        {
            for (int x = 0; x < boardMaxSize; x++)
            {
                int emptyBlockCount = 0;
                for (int y = boardMaxSize - 1; y >= 0; y--)
                {
                    if (m_Blocks[y, x] == null)
                        emptyBlockCount++;
                    else if (emptyBlockCount > 0)
                    {
                        m_Blocks[y + emptyBlockCount, x] = m_Blocks[y, x];
                        m_Blocks[y + emptyBlockCount, x].dropCount = emptyBlockCount;
                        m_Blocks[y + emptyBlockCount, x].myPos = (y + emptyBlockCount, x);
                        m_Blocks[y, x] = null;
                    }
                }
                AddBlock(emptyBlockCount, x);
            }
        }

        private void AddBlock(int emptyBlockCount, int x)
        {
            for (int y = 0; y < emptyBlockCount; y++)
            {
                int block_type = UnityEngine.Random.Range(0, boardMaxSize);
                m_Blocks[y, x] = BlockPool.instance.GetBlock((Block_Type)block_type);
                m_Blocks[y, x].InitBlock((Block_Type)block_type, (y, x), emptyBlockCount);
            }
        }

        public void ComposeMatchBlock(MatchType matchType, Block standardBlock, List<Block> blockList)
        {
            Queue<Block> blocksQueue = new Queue<Block>();
            bool[,] visit = new bool[boardMaxSize, boardMaxSize];

            // 방문 블럭 확인
            for (int y = 0; y < boardMaxSize; y++)
                for (int x = 0; x < boardMaxSize; x++) visit[y, x] = false;

            blocksQueue.Enqueue(standardBlock);
            visit[standardBlock.myPos.y, standardBlock.myPos.x] = true;

            while (blocksQueue.Count > 0)
            {
                Block block = blocksQueue.Dequeue();

                for (int index = 0; index < 4; index++)
                {
                    int x = block.myPos.x + directionX[index];
                    int y = block.myPos.y + directionY[index];

                    if (EscapeRange(x, y) || m_Blocks[y, x] == null) continue; // 범위를 벗어나면
                    if (visit[y, x] || !standardBlock.myType.Equals(m_Blocks[y, x].myType) || m_Blocks[y, x].match == MatchType.NONE) continue; // 이미 방문한 블럭이면

                    // 매치 타입에 따른 애니메이션 수행
                    blockList.Add(m_Blocks[y, x]);
                    blocksQueue.Enqueue(m_Blocks[y, x]);
                    visit[y, x] = true;
                }
            }

            if (matchType > MatchType.FIVE) blockList.ForEach(block => block.UpdateMoveTarget(standardBlock.transform.position));
        }

        public bool IsSpecialSwap(int skillType)
        {
            if (skillType >= 5) return true;
            return false;
        }
    }
}