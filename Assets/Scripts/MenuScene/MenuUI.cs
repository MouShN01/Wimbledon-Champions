using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;
    public TMP_InputField hostPortInputField;
    public GameObject menu;
    public GameObject connMenu;
    public GameObject hostMenu;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManagerWim>();
    }

    public void OnConnectButtonClicked()
    {
        string ipAddress = ipInputField.text;
        string portText = portInputField.text;

        if (int.TryParse(portText, out int port))
        {
            networkManager.networkAddress = ipAddress;
            networkManager.GetComponent<KcpTransport>().port = (ushort)port;
            
            networkManager.StartClient();
        }
        else
        {
            Debug.LogError("Invalid port number");
        }
    }
    
    public void OnCreateHostButtonClicked()
    {
        string portText = hostPortInputField.text;

        if (int.TryParse(portText, out int port))
        {
            networkManager.GetComponent<KcpTransport>().port = (ushort)port;
            
            networkManager.StartHost();
        }
        else
        {
            Debug.LogError("Invalid port number");
        }
    }

    public void OnConnectMenuButtonClicked()
    {
        menu.SetActive(false);
        connMenu.SetActive(true);
    }
    
    public void OnHostConnectMenuButtonClicked()
    {
        menu.SetActive(false);
        hostMenu.SetActive(true);
    }
    
    public void OnHostBackButtonClick()
    {
        menu.SetActive(true);
        hostMenu.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        menu.SetActive(true);
        connMenu.SetActive(false);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}
