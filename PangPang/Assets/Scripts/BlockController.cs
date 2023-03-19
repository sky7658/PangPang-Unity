using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 블럭 스왑 애니메이션
    private IEnumerator MoveToAction(Block baseBlock, Vector2 to, float duration)
    {
        Vector2 startPos = baseBlock.transform.position;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.smoothDeltaTime;
            baseBlock.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);
            yield return null;
        }

        baseBlock.transform.position = to;

        yield break;
    }

    // 블럭 스왑 애니메이션 명령
    private IEnumerator ExecuteSwapAction(Block curBlock, Vector2 swipeD)
    {
        Block targetBlock = board.blocks[curBlock.myPos.y + -(int)swipeD.y, curBlock.myPos.x + (int)swipeD.x];
        Block baseBlock = board.blocks[curBlock.myPos.y, curBlock.myPos.x];

        Vector2 targetPos = targetBlock.transform.position;
        Vector2 basePos = baseBlock.transform.position;

        StartCoroutine(MoveToAction(baseBlock, targetPos, AnimationLength.BLOCK_SWAP));
        StartCoroutine(MoveToAction(targetBlock, basePos, AnimationLength.BLOCK_SWAP));

        yield return new WaitForSeconds(AnimationLength.BLOCK_SWAP);

        (int y, int x) baseXY = curBlock.myPos;
        (int y, int x) targetXY = (curBlock.myPos.y + -(int)swipeD.y, curBlock.myPos.x + (int)swipeD.x);

        board.blocks[targetXY.y, targetXY.x] = baseBlock;
        board.blocks[baseXY.y, baseXY.x] = targetBlock;

        HashSet<(int y, int x)> matches = board.IsMatch_All();

        if (matches.Count > 0)
        {
            board.blocks[baseXY.y, baseXY.x].myPos = baseXY;
            board.blocks[targetXY.y, targetXY.x].myPos = targetXY;

            while (true)
            {
                if (matches.Count == 0) break;
                StartCoroutine(BlockDropAction(matches));
                yield return new WaitForSeconds(AnimationLength.BLOCK_DROP);
                matches = board.IsMatch_All();
            }

            yield break;
        }

        StartCoroutine(MoveToAction(baseBlock, basePos, AnimationLength.BLOCK_SWAP));
        StartCoroutine(MoveToAction(targetBlock, targetPos, AnimationLength.BLOCK_SWAP));

        board.blocks[targetXY.y, targetXY.x] = targetBlock;
        board.blocks[baseXY.y, baseXY.x] = baseBlock;

        yield break;
    }

    // 매치 시 블럭이 없어지는 애니메이션
    private IEnumerator BlockPangAction(Block pangBlock, float toScale, float speed)
    {
        Transform pangBlockT = pangBlock.transform;

        float factor;
        while(pangBlockT.localScale.x > toScale)
        {
            factor = Time.deltaTime * speed;
            pangBlockT.localScale = new Vector3(pangBlockT.localScale.x - factor, pangBlockT.localScale.y - factor, pangBlockT.localScale.z);
            yield return null;
        }

        BlockPool.instance.ReturnBlock(pangBlock);

        yield break;
    }

    private IEnumerator BlockDropAction(HashSet<(int y, int x)> matches)
    {
        foreach (var match in matches)
        {
            StartCoroutine(BlockPangAction(board.blocks[match.y, match.x], 0.3f, 3f));
            board.blocks[match.y, match.x] = null;
        }
        yield return new WaitForSeconds(AnimationLength.BLOCK_PANG);

        board.DownBlock();

        foreach (var block in board.blocks)
        {
            if (block.dropCount > 0)
            {

                Vector2 to = BaseInfo.SetBlockPos(block.myPos.y, block.myPos.x, 0);
                StartCoroutine(MoveToAction(block, to, AnimationLength.BLOCK_DROP));
            }
        }
    }

    void Update()
    {
        MouseDrag();
    }
}