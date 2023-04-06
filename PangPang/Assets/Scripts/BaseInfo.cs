using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationLength 
{
    public static float BLOCK_SWAP = 0.2f;
    public static float BLOCK_PANG = 0.2f;
    public static float BLOCK_DROP = 0.2f;
}

public static class BaseInfo
{
    private static float interval = 3.9f;

    public static int baseScore = 100;

    public static float activeHintTime = 5f;
    public static float comboResetTime = 3f;

    public static float gameTime { get; set; }

    public static Vector2 SetBlockPos(int y, int x, int dropCount)
    {
        return new Vector2(interval / 3 * x - interval, -y * interval / 3 + interval + dropCount * (interval / 3));
    }
}
