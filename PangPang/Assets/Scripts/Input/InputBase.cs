using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Utils
{
    public interface InputBase
    {
        bool isInputDown { get; }
        bool isInputUp { get; }
        Vector2 inputPosition { get; }
    }
}

