using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Utils
{
    public class MouseHandler : InputBase
    {
        bool InputBase.isInputDown => Input.GetButtonDown("Fire1");
        bool InputBase.isInputUp => Input.GetButtonUp("Fire1");
        Vector2 InputBase.inputPosition => Input.mousePosition;
    }

}

