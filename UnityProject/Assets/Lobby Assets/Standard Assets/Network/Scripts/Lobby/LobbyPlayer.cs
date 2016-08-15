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
<<<<<<< HEAD
    {        
        public Text playerNameText;
=======
    {
        static Color[] Colors = new Color[] { Color.blue, Color.red, Color.yellow, Color.green };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Image colorImage;
        public Image nameImage;
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;
        public Button conduitButton;
        public Button aethersmithButton;
        public Button calderaButton;
        public Button shardButton;
        [SyncVar]
        public int connectionId;
        [SyncVar(hook = "OnClassChanged")]
        public string selectedClass;
        public GameObject conduitPrefab;
        public GameObject aethersmithPrefab;
        public GameObject calderaPrefab;
        public GameObject shardPrefab;
<<<<<<< HEAD

        public Color OddRowColor = new Color(17f / 255.0f, 16f / 255.0f, 50f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(7f / 255.0f, 6.0f / 255.0f, 22.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(247f / 255.0f, 104f / 255.0f, 140f / 255.0f, 1.0f);
        static Color NotReadyColor = new Color(1, 1, 1, 0.5f);
        static Color ReadyColor = new Color(104f / 255.0f, 247f / 255.0f, 163f / 255.0f, 1.0f);
=======
        
        public Sprite[] playerNumberImages = new Sprite[4];

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);
        
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
        static Color UnselectedClass = new Color(160, 160, 160, 1);

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);
            
            nameImage.sprite = playerNumberImages[connectionId];

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
            
            conduitButton.interactable = false;
            aethersmithButton.interactable = false;
            calderaButton.interactable = false;
            shardButton.interactable = false;
            //Show previously highlighted
            UpdateSelectedClass(selectedClass);
<<<<<<< HEAD

            ChangeReadyButtonColor(NotReadyColor);

=======
            
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            CheckRemoveButton();
<<<<<<< HEAD

            ChangeReadyButtonColor(JoinColor);
            
            readyButton.interactable = true;
            
=======
            
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet

>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
            conduitButton.interactable = true;
            aethersmithButton.interactable = true;
            calderaButton.interactable = true;
            shardButton.interactable = true;

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
<<<<<<< HEAD
                ChangeReadyButtonColor(ReadyColor);
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);
=======
                ChangeReadyButtonColor(TransparentColor);
                
                readyButton.interactable = false;
            }
            else
            {
                readyButton.interactable = isLocalPlayer;
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }
<<<<<<< HEAD
=======

        ///===== callback from sync var
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
        
        //Takes the string sent to the server and updates the class buttons
        public void OnClassChanged(string newClass)
        {
            selectedClass = newClass;
            UpdateSelectedClass(newClass);
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnReadyClicked()
        {
<<<<<<< HEAD
            if (readyToBegin) SendNotReadyToBeginMessage();
            else SendReadyToBeginMessage();
=======
            SendReadyToBeginMessage();
>>>>>>> 0965adcf211c2bd7e6c7d7b45c167d8c293dbbfc
        }

        //Function for button that sends button string to the server
        public void OnClassSelected(Button button)
        {
            CmdClassChanged(button.gameObject.name);
        }

        [Command]
        void CmdClassChanged(string newClass)
        {
            selectedClass = newClass;
        }

        void UpdateSelectedClass(string buttonName)
        {
            conduitButton.GetComponent<Image>().color = UnselectedClass;
            aethersmithButton.GetComponent<Image>().color = UnselectedClass;
            calderaButton.GetComponent<Image>().color = UnselectedClass;
            shardButton.GetComponent<Image>().color = UnselectedClass;

            switch (buttonName)
            {
                case "ConduitTile":
                    conduitButton.GetComponent<Image>().color = Color.black;
                    break;
                case "AethersmithTile":
                    aethersmithButton.GetComponent<Image>().color = Color.black;
                    break;
                case "CalderaTile":
                    calderaButton.GetComponent<Image>().color = Color.black;
                    break;
                case "ShardTile":
                    shardButton.GetComponent<Image>().color = Color.black;
                    break;
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
