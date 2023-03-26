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
        //LINE_LINE = 8,
        //LINE_AROUND = 9,
        //AROUND_AROUND = 10
    }

    public enum BlockState { IDLE, SWAP, DROP, PANG }
}
