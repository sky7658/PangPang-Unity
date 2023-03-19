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
    private SpriteRenderer mySpr;
    public Block_Type myType;
    public (int y, int x) myPos;
    public int dropCount { get; set; }

    private void Awake()
    {
        mySpr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
    }
    private void Update()
    {
        // StateUpdate();
    }

    public BlockState blockState;
    // private void StateUpdate()
    // {
    //     switch (blockState)
    //     {
    //         case BlockState.IDLE:
    //             break;
    //         case BlockState.SWAP:
    //             StartCoroutine(MoveTo(target, 0.2f));
    //             break;
    //         case BlockState.DROP:
    //             break;
    //         case BlockState.PANG:
    //             Pang();
    //             break;
    //     }
    // }
    public void ChangeState(BlockState _state)
    {
        if (blockState == _state) return;

        blockState = _state;
    }

    public void InitBlock(Block_Type _type, (int y, int x) _pos, int dropCount)
    {
        myPos = _pos;
        myType = _type;
        this.transform.localScale = new Vector3(1, 1, 1);
        this.dropCount = dropCount;
        transform.position = BaseInfo.SetBlockPos(_pos.y, _pos.x, dropCount);
        oriPos = transform.position;
    }

    public Vector2 target;
    public Vector2 oriPos;
}