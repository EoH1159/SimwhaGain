using UnityEngine.EventSystems;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel; // 인벤토리 Panel 참조
    [SerializeField] private CommandUI commandUI;  //  인스펙터에서 넣어줄 것
    private bool reopenCommandAfterInventory;      // 나중에 다시 열어야 하는지 기억


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

        // 인벤토리가 열려 있고, 마우스 왼쪽 버튼을 눌렀을 때
        if (inventoryPanel != null && inventoryPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // 마우스가 어떤 UI 위에 있지 않다면 → 게임 화면(타일/빈 공간 등)을 클릭한 것
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ShowInventory(false); // 인벤토리 닫기
            }
        }
    }

    public void ShowInventory(bool show)
    {
        if (inventoryPanel == null) return;

        if (show)
        {
            // 인벤토리 열 때: 커맨드가 열려 있었다면 일단 닫고, 나중에 다시 열기 위해 기억해둔다
            if (commandUI != null && commandUI.IsOpen)
            {
                reopenCommandAfterInventory = true;
                commandUI.Hide();
            }
            else
            {
                reopenCommandAfterInventory = false;
            }
        }
        else
        {
            // 인벤토리 닫을 때: 아까 열려 있었던 커맨드를 다시 연다
            if (commandUI != null && reopenCommandAfterInventory)
            {
                commandUI.ShowCurrent();
            }
        }

        inventoryPanel.SetActive(show);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        // 무조건 ShowInventory를 거쳐가게 만들기
        ShowInventory(!inventoryPanel.activeSelf);
    }
}
