using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance = null;

    [Header("UI Elements")]
    public GameObject m_lobbyMenu;
    public InputField m_gameNameInputField;
    public GameObject m_inGameMenu;
    public Text m_networkInfoText;

    [Header("Other Settings")]
    public uint m_maxPlayer = 4;

    private bool m_isHost = false;

    private void Awake()
    {
        if (instance != this)
        {
            if (instance != null)
                instance.SelfDestruction();
            instance = this;
        }

        //OnClickMatchmakeHost();
    }

    void Start()
    {
        SetupLobbyMenu();
    }

    //------- UI Interaction functions ----------------
    public void OnClickMatchmakeHost()
    {
        StartMatchMaker();
        matchMaker.CreateMatch(
            m_gameNameInputField.text,
            m_maxPlayer,
            true,
            "", "", "", 0, 0,
            OnMatchCreate);
    }

    public void OnClickMatchmakeClient()
    {
        StartMatchMaker();
        matchMaker.ListMatches(0, 10, "", false, 0, 0, OnMatchList);
    }

    public void SelfDestruction()
    {
        if (m_isHost)
            matchMaker.DestroyMatch(matchInfo.networkId, 0, OnDestroyMatch);
        else
            matchMaker.DropConnection(matchInfo.networkId, NodeID.Invalid, 0, base.OnDropConnection);
        StopMatchMaker();
        Destroy(this);
        Shutdown();
    }

    private void SetupLobbyMenu()
    {
        m_lobbyMenu.SetActive(true);
        m_inGameMenu.SetActive(false);
    }

    private void SetupIngameMenu()
    {
        m_lobbyMenu.SetActive(false);
        m_inGameMenu.SetActive(true);

        m_networkInfoText.text = matchName + "\n"
            + matchInfo.address + "\n"
            + (m_isHost ? "Hosting" : "Connected");
    }

    //------ Overriden Network Manager functions --------
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        if (success)
        {
            Debug.Log("Match: " + matchInfo.networkId + " is created successfully.");
            m_isHost = true;
            SetupIngameMenu();
        }
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        Debug.Log("Match list is called");
        if (success)
        {
            Debug.Log("Retrieve match list successfully.");
            foreach (MatchInfoSnapshot mi in matchList)
            {
                if (mi.currentSize < mi.maxSize)
                {
                    matchMaker.JoinMatch(mi.networkId, "", "", "", 0, 0, OnMatchJoined);
                    break;
                }
            }
        }
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchJoined(success, extendedInfo, matchInfo);
        if (success)
        {
            Debug.Log("Join match: " + matchInfo.networkId + " successfully.");
            m_isHost = false;
            SetupIngameMenu();
        }
    }

    public override void OnDropConnection(bool success, string extendedInfo)
    {
        base.OnDropConnection(success, extendedInfo);
        if (success)
        {
            Debug.Log("Connection dropped.");
            SetupLobbyMenu();
        }
    }
}
