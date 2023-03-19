using System.Collections.Generic;

public class Board
{
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
    }
    private void BoardSetting()
    {
        for (int y = 0; y < boardMaxSize; y++)
        {
            for (int x = 0; x < boardMaxSize; x++)
            {
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

    int[] directionX = { 0, 1, 0, -1 };
    int[] directionY = { 1, 0, -1, 0 };
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
        matchedBlockList.Add(baseBlock);

        // 행의 오른쪽
        for(int x = baseBlock.myPos.x + 1; x < boardMaxSize; x++)
        {
            if (m_Blocks[baseBlock.myPos.y, x].myType.Equals(baseBlock.myType)) matchedBlockList.Add(m_Blocks[baseBlock.myPos.y, x]);
            else break;
        }
        for (int x = baseBlock.myPos.x - 1; x >= 0; x--)
        {
            if (m_Blocks[baseBlock.myPos.y, x].myType.Equals(baseBlock.myType)) matchedBlockList.Add(m_Blocks[baseBlock.myPos.y, x]);
            else break;
        }

        return false;
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

    private void SetMatchBlock(List<Block> matchedBlock)
    {
        foreach(Block block in matchedBlock)
        {
            block.ChangeState(BlockState.PANG);
        }
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
        for(int y = 0; y < emptyBlockCount; y++)
        {
            int block_type = UnityEngine.Random.Range(0, boardMaxSize);
            m_Blocks[y, x] = BlockPool.instance.GetBlock((Block_Type)block_type);
            m_Blocks[y, x].InitBlock((Block_Type)block_type, (y, x), emptyBlockCount);
        }
    }
}