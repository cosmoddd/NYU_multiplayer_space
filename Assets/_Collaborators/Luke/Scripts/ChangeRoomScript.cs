using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Mirror;

public class ChangeRoomScript : NetworkBehaviour
{
    public GameObject customizerUI;
    public GameObject interactText;
    bool bInChangeRoom;

    [Header("In Options Mode")]
    public BoolVariable inOptionsMode;

    [Header("In Options Mode")]
    public BoolVariable inChatMode;

    private void Update()
    {
        if (bInChangeRoom && Input.GetKeyDown(KeyCode.F))
        {
            ToggleChangeRoomUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!isLocalPlayer) return;

        if(other.gameObject.CompareTag("ChangeRoom"))
        {
            bInChangeRoom = true;
            ShowInteractText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
      if (!isLocalPlayer) return;
      
        if (other.gameObject.CompareTag("ChangeRoom"))
        {
            bInChangeRoom = false;
            HideInteractText();
        }
    }

    public void ToggleChangeRoomUI()
    {
        if (!customizerUI.GetComponent<CustomizerUITabs>().customizePanel.activeSelf && inOptionsMode.Value == false && inChatMode.Value == false)
        {
            customizerUI.GetComponent<CustomizerUITabs>().ShowCustomizePanel();
            customizerUI.GetComponent<CustomizerUITabs>().SkipLogin();
            HideInteractText();
            inOptionsMode.SetValue(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            customizerUI.GetComponent<CustomizerUITabs>().HideCustomizePanel();
            ShowInteractText();
            inOptionsMode.SetValue(false);
        }
    }

    public void HideInteractText()
    {

        interactText.SetActive(false);
    }

    public void ShowInteractText()
    {
        interactText.SetActive(true);
    }
}
