using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard
{
    private int boardMaxSize = 7;
    public int GetBoardMaxSize() { return boardMaxSize; }
    private int[,] board;
    public int[,] GetBoard() { return board; }
    BoardUpdate boardUpdate = new BoardUpdate();

    public void RepairBoard() // 보드를 전체적으로 재정비 합니다.
    {
        if (board == null)
        {
            board = new int[boardMaxSize, boardMaxSize];
            boardUpdate.InitBoard(board);
        }
        bool complete = false;
        while (!complete)
        {
            if (boardUpdate.UnableGame(board)) boardUpdate.InitBoard(board);
            else complete = true;
        }
    }

    public bool SwapTarget((int y, int x) targetA, (int y, int x) targetB) // 블록을 바꾸고 "PANG" 인지 확인해줍니다.
    {
        if (boardUpdate.EscapeRange(targetB.x, targetB.y, boardMaxSize)) return false;

        var swap = board[targetA.y, targetA.x];

        board[targetA.y, targetA.x] = board[targetB.y, targetB.x];
        board[targetB.y, targetB.x] = swap;

        var matches = boardUpdate.CompleteSearchBlocks(board);

        if (matches.Count > 0)
        {
            foreach ((int y, int x) pos in matches)
                board[pos.y, pos.x] = -1;

            boardUpdate.DownBlock(board);
            boardUpdate.AddBlock(board);
            return true;
        }

        board[targetB.y, targetB.x] = board[targetA.y, targetA.x];
        board[targetA.y, targetA.x] = swap;

        return false;
    }
}