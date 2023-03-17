using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Block_Type
{
    RED,
    Orange,
    Yellow,
    Green,
    Blue,
    Indigo,
    Purple,
    Bomb
}

public enum BlockState { IDLE, SWAP, DROP, PANG }

public class Block : MonoBehaviour
{
    private Rigidbody2D myRig;
    private SpriteRenderer mySpr;
    public Block_Type myType;
    public (int y, int x) myPos;

    private void Awake()
    {
        myRig = GetComponent<Rigidbody2D>();
        mySpr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
    }
    private void Update()
    {
        StateUpdate();
    }

    public BlockState blockState;
    private void StateUpdate()
    {
        switch (blockState)
        {
            case BlockState.IDLE:
                break;
            case BlockState.SWAP:
                StartCoroutine(MoveTo(target, 0.2f));
                break;
            case BlockState.DROP:
                break;
            case BlockState.PANG:
                Pang();
                break;
        }
    }
    public void ChangeState(BlockState _state)
    {
        if (blockState == _state) return;

        blockState = _state;
    }

    public void Pang()
    {

    }

    float interval = 1.5f;
    public void InitBlock(Block_Type _type, (int y, int x) _pos)
    {
        switch (_type)
        {
            case Block_Type.RED:
                mySpr.color = Color.red;
                break;
            case Block_Type.Orange:
                mySpr.color = Color.black; // 수정
                break;
            case Block_Type.Yellow:
                mySpr.color = Color.yellow;
                break;
            case Block_Type.Green:
                mySpr.color = Color.green;
                break;
            case Block_Type.Blue:
                mySpr.color = Color.blue;
                break;
            case Block_Type.Indigo:
                mySpr.color = Color.gray; // 수정
                break;
            case Block_Type.Purple:
                mySpr.color = Color.magenta; // 수정
                break;
        }

        myPos = _pos;
        myType = _type;
        transform.position = new Vector2((interval / 3 * _pos.x) - interval, (-_pos.y * interval / 3) + interval);
        oriPos = transform.position;
    }

    public Vector2 target;
    public Vector2 oriPos;
    private IEnumerator MoveTo(Vector2 to, float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.smoothDeltaTime;
            this.transform.position = Vector2.Lerp(transform.position, to, elapsed / duration);

            yield return null;
        }

        blockState = BlockState.IDLE;
        this.transform.position = to;
        Debug.Log($"{myType} : 이동 완료!");

        yield break;
    }
}