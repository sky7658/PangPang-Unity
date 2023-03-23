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
        Block[,] m_Blocks;
        public Block[,] blocks { get { return m_Blocks; } }

        public Board()
        {
            boardMaxSize = 7;
            m_Blocks = new Block[boardMaxSize, boardMaxSize];
        }
        public void InitBoard()
        {
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
            //m_Blocks[2, 2] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[2, 2].InitBlock(Block_Type.RED, (2, 2), 0);

            //m_Blocks[3, 2] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[3, 2].InitBlock(Block_Type.RED, (3, 2), 0);

            //m_Blocks[4, 3] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[4, 3].InitBlock(Block_Type.RED, (4, 3), 0);

            //m_Blocks[4, 1] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[4, 1].InitBlock(Block_Type.RED, (4, 1), 0);

            //m_Blocks[5, 2] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[5, 2].InitBlock(Block_Type.RED, (5, 2), 0);

            // Test 용 코드
            //m_Blocks[0, 0] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 0].InitBlock(Block_Type.RED, (0, 0), 0);

            //m_Blocks[0, 2] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 2].InitBlock(Block_Type.RED, (0, 2), 0);

            //m_Blocks[0, 3] = BlockPool.instance.GetBlock(Block_Type.RED);
            //m_Blocks[0, 3].InitBlock(Block_Type.RED, (0, 3), 0);

            //m_Blocks[0, 1] = BlockPool.instance.GetBlock(Block_Type.Orange);
            //m_Blocks[0, 1].InitBlock(Block_Type.Orange, (0, 1), 0);

            //m_Blocks[1, 0] = BlockPool.instance.GetBlock(Block_Type.Orange);
            //m_Blocks[1, 0].InitBlock(Block_Type.Orange, (1, 0), 0);

            //m_Blocks[2, 0] = BlockPool.instance.GetBlock(Block_Type.Orange);
            //m_Blocks[2, 0].InitBlock(Block_Type.Orange, (2, 0), 0);

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
        private void BoardReturn()
        {
            if (m_Blocks[0, 0] == null) return;

            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                    BlockPool.instance.ReturnBlock(m_Blocks[y, x]);
            }
        }
        private bool EscapeRange(int posX, int posY)
        {
            if (posX < 0 || posY < 0 || posX > boardMaxSize - 1 || posY > boardMaxSize - 1) return true;
            return false;
        }

        // 현재 보드에서 매치가 가능한 블럭이 있는지 확인한다.
        private bool AIMatch()
        {
            // 기존 보드 복사
            Block[,] copy_blocks = (Block[,])m_Blocks.Clone();

            if (IsMatch_All().Count > 0) return false;   // 이미 매치된 블럭이 있다면 재 구성

            for (int y = 0; y < boardMaxSize; y++)
            {
                for (int x = 0; x < boardMaxSize; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        // 기존 블럭 다시 넣기
                        m_Blocks = (Block[,])copy_blocks.Clone();

                        int _x = x + directionX[i];
                        int _y = y + directionY[i];

                        if (EscapeRange(_x, _y)) continue;

                        Block baseBlock = m_Blocks[y, x];
                        Block targetBlock = m_Blocks[_y, _x];

                        m_Blocks[y, x] = targetBlock;
                        m_Blocks[_y, _x] = baseBlock;

                        if (IsMatch_All().Count > 0) // 게임이 가능한 상황이면 기존 블록을 다시 넣어 결과값 리턴
                        {
                            m_Blocks = (Block[,])copy_blocks.Clone();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsMatch_Part(Block baseBlock, List<Block> matchedBlockList)
        {
            bool bFound = false;
            if (baseBlock.match != MatchType.NONE) return false;

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

        public bool IsMatch_All_()
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

        // 전체 블럭 중 매치가 되어있는 블럭들을 반환해준다.
        public HashSet<(int, int)> IsMatch_All()
        {
            HashSet<(int y, int x)> matches = new HashSet<(int y, int x)>();

            for (int y = 0; y < boardMaxSize; y++)
            {
                int blockCountRow = 1;
                int blockCountCol = 1;
                for (int x = 1; x < boardMaxSize; x++)
                {
                    if (m_Blocks[y, x].myType == m_Blocks[y, x - 1].myType) blockCountRow++;
                    else
                    {
                        if (blockCountRow >= 3)
                        {
                            for (int index = x - blockCountRow; index < x; index++)
                                matches.Add((y, index));
                        }
                        blockCountRow = 1;
                    }

                    // 열 탐색
                    if (m_Blocks[x, y].myType == m_Blocks[x - 1, y].myType) blockCountCol++;
                    else
                    {
                        if (blockCountCol >= 3)
                        {
                            for (int index = x - blockCountCol; index < x; index++)
                                matches.Add((index, y));
                        }
                        blockCountCol = 1;
                    }

                    // 행과 열
                    if (x == boardMaxSize - 1)
                    {
                        if (blockCountRow >= 3)
                        {
                            for (int index = x - blockCountRow + 1; index <= x; index++)
                                matches.Add((y, index));
                        }
                        if (blockCountCol >= 3)
                        {
                            for (int index = x - blockCountCol + 1; index <= x; index++)
                                matches.Add((index, y));
                        }
                    }
                }
            }
            return matches;
        }

        private void SetMatchBlock(List<Block> matchedBlock, UnityEngine.Vector2 moveTarget)
        {
            matchedBlock.ForEach(block => block.UpdateMatchType((MatchType)matchedBlock.Count));
            matchedBlock.ForEach(block => block.UpdateMoveTarget(moveTarget));
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

        public void CreateSpecialBlock(MatchType matchType, Block standardBlock, List<Block> specialBlockList)
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

                    if (EscapeRange(x, y)) continue; // 범위를 벗어나면
                    if (visit[y, x] || !standardBlock.myType.Equals(m_Blocks[y, x].myType) || m_Blocks[y, x].match == MatchType.NONE) continue; // 이미 방문한 블럭이면

                    // 매치 타입에 따른 애니메이션 수행
                    specialBlockList.Add(m_Blocks[y, x]);
                    blocksQueue.Enqueue(m_Blocks[y, x]);
                    visit[y, x] = true;
                }
            }
        }
    }
}