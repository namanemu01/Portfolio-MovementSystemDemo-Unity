using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MovementStstem
{
    public class   QuitPanel : MonoBehaviour
    {
        [Header("UI组件")]
        public GameObject confirmPanel;  // 确认面板
        public Button quitButton;        // 退出按钮
        public Button confirmButton;     // 确认按钮
        public Button cancelButton;      // 取消按钮

        // Start is called before the first frame update
        void Start()
        {
            // 隐藏确认面板
            confirmPanel.SetActive(false);

            // 绑定事件 监听
            //如果点击离开按钮，就打开面板
            quitButton.onClick.AddListener(ShowConfirmPanel);
            //点击确认
            confirmButton.onClick.AddListener(QuitGame);
            //点击取消
            cancelButton.onClick.AddListener(HideConfirmPanel);
        }

        // Update is called once per frame
        void Update()
        {
            //另一种方式是 按esc按下之后
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //如果当前面板是打开状态 
                if (confirmPanel.activeSelf)
                {
                    //就隐藏面板
                    HideConfirmPanel();
                }
                else
                {
                    //显示面板
                    ShowConfirmPanel();
                }
            }
        }
        /// <summary>
        /// 显示面板的时候 暂停游戏
        /// </summary>
        void ShowConfirmPanel()
        {
            confirmPanel.SetActive(true);
            Time.timeScale = 0f;  // 暂停游戏
        }
        /// <summary>
        /// 隐藏面板的时候 继续游戏
        /// </summary>
        void HideConfirmPanel()
        {
            confirmPanel.SetActive(false);
            Time.timeScale = 1f;  // 恢复游戏
        }

        void QuitGame()
        {
            Debug.Log("QuitGame");

            Application.Quit();
        }
    }
}
