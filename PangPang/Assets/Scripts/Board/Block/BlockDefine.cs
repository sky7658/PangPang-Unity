using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Board
{
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

    public enum BlockSkill
    {
        NONE,
        LINE = 4,
        AROUND = 5,
    }

    public enum BlockState { IDLE, SWAP, DROP, PANG }
}
