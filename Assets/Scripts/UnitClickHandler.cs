
using UnityEngine;

public class UnitClickHandler : MonoBehaviour
{
    public UnitStatus unitStatus;
    public CommandUI commandUI;

    private void OnMouseDown()
    {
        Debug.Log("Unit clicked");  // 클릭 확인용

        if (commandUI != null)
        {
            commandUI.Show(unitStatus);
        }
        else
        {
            Debug.LogWarning("commandUI 가 비어 있습니다!");
        }
    }
}
