using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager Instance = null;

    [Header ("- 오브젝트 설정")]
    [SerializeField] GameObject hpUI;
    [SerializeField] GameObject panicBarUI;
    [SerializeField] GameObject menuUI;

    private HUDManager() { }

    void Start()
    {
        if (Instance == null)
        {
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            GameObject.Destroy(this);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = menuUI.activeInHierarchy;

            menuUI.SetActive(!isActive);
            
            // 메뉴화면 열렸을 시, 게임 일시정지
            Time.timeScale = menuUI.activeInHierarchy ? 0 : 1;
        }
    }

    public static HUDManager GetInstance()
    {
        if (Instance == null)
        {
            Debug.Log("[WARNING] HUD Manager is null.");
        }
        return Instance;
    }

    public void RefreshHUD()
    {
        int hp = GameManager.GetInstance().getHP();
        float panic = GameManager.GetInstance().getPanic();

        for(int idx = 0; idx < hpUI.transform.childCount; ++idx)
        {
            bool isActive = idx < hp;

            hpUI.transform.GetChild(idx).gameObject.SetActive(isActive);
        }

        panicBarUI.GetComponent<Image>().fillAmount = panic;
    }

    public void OnResetClick()
    {

    }

    public void OnSettingsClick()
    {

    }
    
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
