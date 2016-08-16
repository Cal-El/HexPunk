using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityStandardAssets.Network
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {        
        public Text playerNameText;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;
        public Button leftButton;
        public Button rightButton;
        [SyncVar]
        public int connectionId;
        [SyncVar(hook = "OnClassChanged")]
        public int currentClass = 0;
        [SyncVar]
        public int selectedClass; //unselected
        public GameObject[] classTiles = new GameObject[4];
        public GameObject conduitPrefab;
        public GameObject aethersmithPrefab;
        public GameObject calderaPrefab;
        public GameObject shardPrefab;

        public Color OddRowColor = new Color(17f / 255.0f, 16f / 255.0f, 50f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(7f / 255.0f, 6.0f / 255.0f, 22.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(247f / 255.0f, 104f / 255.0f, 140f / 255.0f, 1.0f);
        static Color NotReadyColor = new Color(1, 1, 1, 0.5f);
        static Color ReadyColor = new Color(104f / 255.0f, 247f / 255.0f, 163f / 255.0f, 1.0f);
        static Color UnselectedClass = new Color(160, 160, 160, 1);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            playerNameText.text = string.Format("Player {0}", connectionId + 1);

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

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            removePlayerButton.interactable = NetworkServer.active;
            //Show previously highlighted
            UpdateSelectedClass(currentClass);

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.interactable = false;
            leftButton.interactable = false;
            rightButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            CmdSelectedClass(-1);
            CheckRemoveButton();

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
                foreach(var slot in LobbyManager.s_Singleton.lobbySlots)
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
            foreach(var tile in classTiles)
            {
                tile.SetActive(false);
            }

            classTiles[currentClass].SetActive(true);

            //conduitButton.GetComponent<Image>().color = UnselectedClass;
            //aethersmithButton.GetComponent<Image>().color = UnselectedClass;
            //calderaButton.GetComponent<Image>().color = UnselectedClass;
            //shardButton.GetComponent<Image>().color = UnselectedClass;

            //switch (currentClass)
            //{
            //    case 0:
            //        conduitButton.GetComponent<Image>().color = Color.black;
            //        break;
            //    case 1:
            //        aethersmithButton.GetComponent<Image>().color = Color.black;
            //        break;
            //    case 2:
            //        calderaButton.GetComponent<Image>().color = Color.black;
            //        break;
            //    case 3:
            //        shardButton.GetComponent<Image>().color = Color.black;
            //        break;
            //}
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
