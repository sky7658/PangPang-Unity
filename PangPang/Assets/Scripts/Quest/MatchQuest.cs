using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Quest
{
    public enum MatchType
    {
        NONE = 0,
        THREE = 3,
        FOUR = 4,
        FIVE = 5,
        THREE_THREE = 6,
        THREE_FOUR = 7
    }

    public static class MatchTypeMethod
    {
        public static MatchType Add(this MatchType curType, MatchType addType)
        {
            int result = ((int)curType + (int)addType) - 1;

            if (result >= 7) return MatchType.THREE_FOUR;
            return (MatchType)result;
        }
    }

}