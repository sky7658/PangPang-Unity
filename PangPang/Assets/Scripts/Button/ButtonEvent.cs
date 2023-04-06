using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace PangPang.ButtonEvent
{
    public class ButtonEvent : MonoBehaviour
    {
        //Option
        protected GameObject optionWindow;

        public void ChangeScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void ActiveWindow(bool set)
        {
            string name = EventSystem.current.currentSelectedGameObject.name;
            switch (name)
            {
                //case "Score":
                //    if(activeWindow != null)
                //        activeWindow.SetActive(true);
                //    break;
                case "OptionButton":
                    if (optionWindow != null)
                    {
                        optionWindow.SetActive(true);
                        optionWindow.transform.GetChild(3).gameObject.SetActive(set);
                        optionWindow.transform.GetChild(4).gameObject.SetActive(set);
                        optionWindow.transform.GetChild(5).gameObject.SetActive(set);
                    }
                    break;
            }
        }

        public void UnActiveWindow()
        {
            var window = EventSystem.current.currentSelectedGameObject.transform.parent;

            if (window == null) return;

            window.gameObject.SetActive(false);
        }
    }

}