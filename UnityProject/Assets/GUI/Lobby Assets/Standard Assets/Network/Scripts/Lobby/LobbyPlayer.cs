using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityStandardAssets.Network
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        public RectTransform myRectTransform;
        public Text playerNameText;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;
        public Button leftButton;
        public Button rightButton;
        public Text ipAddressText;
        [SyncVar]
        public string ipAddressString;

        [SyncVar]
        public int connectionId;

        [SyncVar(hook = "OnClassChanged")]
        public int currentClass = 0;
        [SyncVar]
        public int selectedClass; //unselected
        public GameObject[] classTiles = new GameObject[4];

        public Color OddRowColor = new Color(17f / 255.0f, 16f / 255.0f, 50f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(7f / 255.0f, 6.0f / 255.0f, 22.0f / 255.0f, 1.0f);
        public Sprite Conduit;
        public Sprite ConduitGrey;
        public Sprite Aethersmith;
        public Sprite AethersmithGrey;
        public Sprite Caldera;
        public Sprite CalderaGrey;
        public Sprite Shard;
        public Sprite ShardGrey;

        private Vector2 startPos = Vector2.zero;

        static Color JoinColor = new Color(247f / 255.0f, 104f / 255.0f, 140f / 255.0f, 1.0f);
        static Color NotReadyColor = new Color(1, 1, 1, 0.5f);
        static Color ReadyColor = new Color(104f / 255.0f, 247f / 255.0f, 163f / 255.0f, 1.0f);
        static Color OtherClass = new Color(1, 1, 1, 125f / 255f);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            ipAddressText = GameObject.Find("IpAddress").GetComponent<Text>();

            playerNameText.text = string.Format("Player {0}", connectionId + 1);

            var lobbyPlayers = FindObjectsOfType<LobbyPlayer>();

            if (connectionId != 0)
            {
                var host = lobbyPlayers.First(i => i.connectionId == 0);
                if (host != null) ipAddressText.text = host.ipAddressString;
            }

            //Check if the same connected player exists
            var playersWithSameId = lobbyPlayers.Where(i => i.connectionId == connectionId).ToList();
            if (playersWithSameId.Count > 1)
            {
                foreach(var player in playersWithSameId)
                {
                    if (player != this) Destroy(player.gameObject);
                }
            }

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            if(connectionId == 0) CmdSetIpAddress();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

            SetupLocalPlayer();
        }

        [Command]
        private void CmdSetIpAddress()
        {
            ipAddressString = string.Format("IP: {0}", LocalIPAddress());
            ipAddressText.text = ipAddressString;
        }

        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
            readyButton.gameObject.GetComponent<Image>().color = c;
        }

        void SetupOtherPlayer()
        {
            removePlayerButton.interactable = NetworkServer.active;
            //Show previously highlighted
            UpdateSelectedClass(currentClass);

            foreach (var tile in classTiles)
            {
                SetOTherPlayerClassImages(tile, false);
                tile.GetComponent<Image>().color = OtherClass;
                tile.GetComponentInChildren<Text>().color = OtherClass;
            }

            foreach (var player in FindObjectsOfType<LobbyPlayer>())
            {
                if (player != null && !player.isLocalPlayer)
                {
                    player.ChangeReadyButtonColor(player.selectedClass < 0 ? NotReadyColor : ReadyColor);
                }
            }

            readyButton.interactable = false;
            leftButton.interactable = false;
            rightButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            CmdSelectedClass(-1);
            CheckRemoveButton();

            foreach (var tile in classTiles)
            {
                SetOTherPlayerClassImages(tile, true);
                tile.GetComponent<Image>().color = Color.white;
                tile.GetComponentInChildren<Text>().color = Color.white;
            }

            ChangeReadyButtonColor(JoinColor);

            readyButton.interactable = true;
            leftButton.interactable = true;
            rightButton.interactable = true;


            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        void Update()
        {
            if (isLocalPlayer && !readyToBegin)
            {
                if (Input.GetButtonDown("Horizontal"))
                {
                    if (Input.GetAxisRaw("Horizontal") == 1)
                    {
                        OnClassIncreased();
                    }
                    if (Input.GetAxisRaw("Horizontal") == -1)
                    {
                        OnClassDescreased();
                    }
                }
            }

            if(myRectTransform != null) myRectTransform.anchoredPosition = new Vector2(connectionId * 480, 0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.gameObject.SetActive(connectionId == 0);
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(ReadyColor);
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);
            }
        }

        public void OnPlayerListChanged(int idx)
        {
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        //Takes the string sent to the server and updates the class buttons
        public void OnClassChanged(int newClass)
        {
            currentClass = newClass;

            UpdateSelectedClass(newClass);
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnReadyClicked()
        {
            if (readyToBegin)
            {
                CmdSelectedClass(-1); // unselected
                leftButton.interactable = true;
                rightButton.interactable = true;

                SendNotReadyToBeginMessage();
            }
            else
            {
                selectedClass = currentClass;
                CmdSelectedClass(currentClass);
                leftButton.interactable = false;
                rightButton.interactable = false;

                bool hasBeenSelected = false;
                foreach (var slot in LobbyManager.s_Singleton.lobbySlots)
                {
                    if (!hasBeenSelected && slot != null)
                    {
                        LobbyPlayer lp = slot.GetComponent<LobbyPlayer>();
                        if (lp != null)
                        {
                            if (lp.selectedClass != -1)
                            {
                                if (lp.connectionId != connectionId)
                                {
                                    hasBeenSelected = selectedClass == lp.selectedClass;
                                }
                            }
                        }
                    }
                }

                if (!hasBeenSelected)
                {
                    SendReadyToBeginMessage(); // Ready up
                }
                else
                {
                    selectedClass = -1;
                    CmdSelectedClass(-1); // Unselect class
                    leftButton.interactable = true;
                    rightButton.interactable = true;
                }
            }
        }

        [Command]
        public void CmdSelectedClass(int classNumber)
        {
            selectedClass = classNumber;
        }

        public void OnClassIncreased()
        {
            CmdClassChanged(true);
        }
        public void OnClassDescreased()
        {
            CmdClassChanged(false);
        }

        //Function for button that sends button string to the server

        [Command]
        void CmdClassChanged(bool increment)
        {
            if (increment)
            {
                if (currentClass < classTiles.Length - 1)
                {
                    currentClass++;
                }
                else
                {
                    currentClass = 0;
                }
            }
            else
            {
                if (currentClass > 0)
                {
                    currentClass--;
                }
                else
                {
                    currentClass = classTiles.Length - 1;
                }
            }
        }

        void UpdateSelectedClass(int currentClass)
        {
            foreach (var tile in classTiles)
            {
                tile.SetActive(false);
            }

            classTiles[currentClass].SetActive(true);
        }

        void SetOTherPlayerClassImages(GameObject tile, bool isLocalP)
        {
            var tileImg = tile.GetComponent<Image>();
            if (tile.name.Contains("Conduit"))
            {
                tileImg.sprite = isLocalP ? Conduit : ConduitGrey;
            }
            if (tile.name.Contains("Aethersmith"))
            {
                tileImg.sprite = isLocalP ? Aethersmith : AethersmithGrey;
            }
            if (tile.name.Contains("Caldera"))
            {
                tileImg.sprite = isLocalP ? Caldera : CalderaGrey;
            }
            if (tile.name.Contains("Shard"))
            {
                tileImg.sprite = isLocalP ? Shard : ShardGrey;
            }
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);

        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);
        }
    }
}
