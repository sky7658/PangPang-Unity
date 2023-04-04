using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private GameObject activeWindow;
    [SerializeField] private GameObject optionWindow;
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ActiveWindow()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        switch(name)
        {
            case "Score":
                if(activeWindow != null)
                    activeWindow.SetActive(true);
                break;
            case "OptionButton":
                if(optionWindow != null)
                    optionWindow.SetActive(true);
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
