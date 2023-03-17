using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public Block[,] blocks;
    GameBoard gameBoard;

    void Start()
    {
        gameBoard = new GameBoard();

        gameBoard.RepairBoard();
        blocks = new Block[gameBoard.GetBoardMaxSize(), gameBoard.GetBoardMaxSize()];
        AllBlocksDraw();
    }

    void AllBlocksDraw()
    {
        for (int y = 0; y < blocks.GetLength(0); y++)
        {
            for (int x = 0; x < blocks.GetLength(1); x++)
            {
                if (blocks[y, x] != null)
                    BlockPool.instance.ReturnBlock(blocks[y, x]);
                blocks[y, x] = BlockPool.instance.GetBlock();
                blocks[y, x].InitBlock((Block_Type)gameBoard.GetBoard()[y, x], (y, x));
            }
        }
    }

    private Vector2 arrowVector, mousePos;
    (int, int) hitpos;
    private RaycastHit2D b1, b2;
    void MouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            arrowVector = Vector2.zero;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            arrowVector -= mousePos;

            b1 = Physics2D.Raycast(mousePos, transform.forward);
            hitpos = ((int)b1.transform.position.y, (int)b1.transform.position.x);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            arrowVector += mousePos;

            arrowVector = ArrowCal(arrowVector);

            Vector2 check = new Vector2(b1.transform.position.x + arrowVector.x, b1.transform.position.y + arrowVector.y);
            b2 = Physics2D.Raycast(check, Vector2.zero);

            if (b2)
                OrderSwap(b1.transform.GetComponent<Block>(), b2.transform.GetComponent<Block>());
        }
    }
    Vector2 ArrowCal(Vector2 vc)
    {
        Vector2 newVc = Vector2.zero;
        if (Mathf.Abs(vc.x) < Mathf.Abs(vc.y))
        {
            newVc.x = 0;
            if (vc.y > 0) newVc.y = 0.5f;
            else newVc.y = -0.5f;
        }
        else
        {
            newVc.y = 0;
            if (vc.x > 0) newVc.x = 0.5f;
            else newVc.x = -0.5f;
        }
        return newVc;
    }

    void OrderSwap(Block _b1, Block _b2)
    {
        var swapPos = _b1.myPos;

        _b1.target = _b2.transform.position;
        _b1.blockState = BlockState.SWAP;

        _b2.target = _b1.transform.position;
        _b2.blockState = BlockState.SWAP;
    }

    void Update()
    {
        MouseDrag();
    }
}