
using UnityEngine;

public class UnitClickHandler : MonoBehaviour
{
    public UnitStatus unitStatus;
    public CommandUI commandUI;

    private void OnMouseDown()
    {
        Debug.Log("Player Unit clicked!");

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
