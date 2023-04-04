using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PangPang.Utils
{
    public class InputManager
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InputBase m_InputHandler = new TouchHandler();
#else 
        InputBase m_InputHandler = new MouseHandler();
#endif

        public bool isTouchDown => m_InputHandler.isInputDown;
        public bool isTouchUp => m_InputHandler.isInputUp;
        public Vector2 touchPosition => m_InputHandler.inputPosition;
    }

}
