using TMPro;
using UnityEngine;

public class EnemyStatusUI : MonoBehaviour
{
    public static EnemyStatusUI Instance { get; private set; }

    public GameObject panel;
    public TMP_Text hpText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Show(UnitStatus enemy)
    {
        if (enemy == null) return;

        if (panel != null)
            panel.SetActive(true);

        if (hpText != null)
            hpText.text = $"Àû HP: {enemy.currentHP} / {enemy.maxHP}";
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
