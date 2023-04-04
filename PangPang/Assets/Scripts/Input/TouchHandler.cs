using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Utils
{
    public class TouchHandler : InputBase
    {
        bool InputBase.isInputDown => Input.GetTouch(0).phase == TouchPhase.Began;
        bool InputBase.isInputUp => Input.GetTouch(0).phase == TouchPhase.Ended;
        Vector2 InputBase.inputPosition => Input.GetTouch(0).position;
    }

}
