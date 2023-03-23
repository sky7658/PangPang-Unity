using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Board
{
    [System.Serializable]
    public class BlockInfo
    {
        public Queue<Block> blockQueue = new Queue<Block>();
        public GameObject blockPrefab;
        public Transform parent;
        public Block_Type _type;
        public int poolCount;
    }

    public class BlockPool : MonoBehaviour
    {
        public static BlockPool instance;
        private void Awake()
        {
            instance = this;
            Initialize();
        }

        [SerializeField]
        private List<BlockInfo> blockInfos = new List<BlockInfo>();

        private void Initialize()
        {
            foreach (var blockInfo in blockInfos)
                InsertQueue(blockInfo);
        }

        private void InsertQueue(BlockInfo blockInfo)
        {
            for (int i = 0; i < blockInfo.poolCount; i++)
            {
                var newBlock = Instantiate(blockInfo.blockPrefab).GetComponent<Block>();

                newBlock.gameObject.SetActive(false);
                if (blockInfo.parent != null)
                    newBlock.transform.SetParent(blockInfo.parent);
                else
                    newBlock.transform.SetParent(transform);

                blockInfo.blockQueue.Enqueue(newBlock);
            }
        }

        public Block GetBlock(Block_Type type)
        {
            foreach (var blockInfo in blockInfos)
            {
                if (blockInfo._type == type)
                {
                    if (blockInfo.blockQueue.Count > 0)
                    {
                        var block = blockInfo.blockQueue.Dequeue();
                        block.gameObject.SetActive(true);
                        return block;
                    }
                    else
                    {
                        var newBlock = Instantiate(blockInfo.blockPrefab).GetComponent<Block>();
                        newBlock.gameObject.SetActive(true);
                        if (blockInfo.parent != null)
                            newBlock.transform.SetParent(blockInfo.parent);
                        else
                            newBlock.transform.SetParent(transform);
                        return newBlock;
                    }
                }
            }

            Debug.Log("오브젝트를 찾을 수 없습니다.");
            return null;
        }

        public void ReturnBlock(Block block)
        {
            foreach (var blockInfo in blockInfos)
            {
                if (blockInfo._type == block.myType)
                {
                    block.gameObject.SetActive(false);
                    blockInfo.blockQueue.Enqueue(block);
                }
            }
        }
    }

}