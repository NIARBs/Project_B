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
}
