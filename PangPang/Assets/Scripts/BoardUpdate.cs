using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUpdate
{
    // 전체 보드 초기화
    public void InitBoard(int[,] board)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                int block_num = Random.Range(0, board.GetLength(0));
                board[y, x] = block_num;
            }
        }
    }

    // 빈 블럭 채우기
    public void AddBlock(int[,] board)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] == -1)
                {
                    int block_num = Random.Range(0, board.GetLength(0));
                    board[y, x] = block_num;
                }
            }
        }
    }

    // 보드 범위를 벗어났는지 확인
    public bool EscapeRange(int posX, int posY, int boardMaxSize)
    {
        if (posX < 0 || posY < 0 || posX > boardMaxSize - 1 || posY > boardMaxSize - 1) return true;
        return false;
    }

    int[] directionX = { 0, 1, 0, -1 };
    int[] directionY = { 1, 0, -1, 0 };
    public bool UnableGame(int[,] board)
    {
        // 깊은 복사
        int[,] _board;

        if (CompleteSearchBlocks(board).Count > 0) { return true; }

        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                for (int i = 0; i < 4; i++)
                {
                    _board = (int[,])board.Clone();
                    int swap;
                    int _x = x + directionX[i];
                    int _y = y + directionY[i];

                    if (EscapeRange(_x, _y, board.GetLength(0))) continue;

                    swap = _board[y, x];
                    _board[y, x] = _board[_y, _x];
                    _board[_y, _x] = swap;

                    if (CompleteSearchBlocks(_board).Count > 0)
                        return false;
                }
            }
        }
        return true;
    }

    public HashSet<(int y, int x)> CompleteSearchBlocks(int[,] _board) // 매치 블럭 완전 탐색
    {
        HashSet<(int y, int x)> matches = new HashSet<(int y, int x)>();

        // 매치 블럭 position 저장
        for (int y = 0; y < _board.GetLength(0); y++)
        {
            int blockCountRow = 1; //행 카운터
            int blockCountCol = 1; //열 카운터
            for (int x = 1; x < _board.GetLength(1); x++)
            {
                // 행 탐색
                if (_board[y, x] == _board[y, x - 1]) blockCountRow++;
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
                if (_board[x, y] == _board[x - 1, y]) blockCountCol++;
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
                if (x == _board.GetLength(1) - 1)
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

    // 블럭을 내려줍니다.
    public void DownBlock(int[,] board)
    {
        for (int x = 0; x < board.GetLength(1); x++)
        {
            int emptyBlockCount = 0;
            for (int y = board.GetLength(0) - 1; y >= 0; y--)
            {
                if (board[y, x] == -1)
                    emptyBlockCount++;
                else if (emptyBlockCount > 0)
                {
                    board[y + emptyBlockCount, x] = board[y, x];
                    board[y, x] = -1;
                }
            }
        }
    }
}