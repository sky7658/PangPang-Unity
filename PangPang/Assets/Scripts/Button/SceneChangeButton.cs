using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PangPang.ButtonEvent
{
    public class SceneChangeButton : ButtonEvent
    {
        [SerializeField] private int sceneIndex;
        private Button sceneChangeBtn;
        private void Awake()
        {
            sceneChangeBtn = this.GetComponent<Button>();
            sceneChangeBtn.onClick.AddListener(() => ChangeScene(sceneIndex));
            sceneChangeBtn.onClick.AddListener(UnActiveWindow);
        }
    }

}
