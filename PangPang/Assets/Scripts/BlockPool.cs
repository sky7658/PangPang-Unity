using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool instance;
    private void Awake()
    {
        instance = this;
        Initialize();
    }

    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private Transform[] parent = new Transform[2];
    private Queue<Block> blocksQueue = new Queue<Block>();
    private int maxBlocknum = 50;
    private void Initialize()
    {
        for (int i = 0; i < maxBlocknum; i++)
            blocksQueue.Enqueue(CreateBlock());
    }

    private Block CreateBlock()
    {
        var newBlock = Instantiate(blockPrefab).GetComponent<Block>();
        newBlock.transform.SetParent(parent[0]);
        newBlock.gameObject.SetActive(false);

        return newBlock;
    }

    public Block GetBlock()
    {
        if (blocksQueue.Count > 0)
        {
            var obj = blocksQueue.Dequeue();
            obj.transform.SetParent(parent[1]);
            obj.gameObject.SetActive(true);

            return obj;
        }
        else
        {
            var newObj = CreateBlock();
            newObj.transform.SetParent(parent[1]);
            newObj.gameObject.SetActive(true);

            return newObj;
        }
    }

    public void ReturnBlock(Block obj)
    {
        obj.transform.SetParent(parent[0]);
        obj.gameObject.SetActive(false);
        blocksQueue.Enqueue(obj);
    }
}
