
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel; // 인벤토리 Panel 참조

    private void Awake()
    {
        // 싱글톤 셋업
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지하고 싶으면 나중에 켜기
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ShowInventory(bool show)
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(show);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
