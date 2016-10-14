using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;


namespace UnityStandardAssets.Network
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;


        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public GameObject gameMainMenu;
        public LobbyTopPanel topPanel;

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        [Tooltip("0-Menu\n1-Conduit\n2-Aethersmith\n3-Caldera\n4-Shard")]
        [SerializeField]
        private Texture2D[] cursors;

        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            DontDestroyOnLoad(gameObject);
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (_isMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenuPanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                gameMainMenu.SetActive(false);

                ChangeTo(null);

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                if (!gameMainMenu.activeSelf) gameMainMenu.SetActive(true);
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                _isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
        }

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }
                 
        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
                this.matchMaker.DestroyMatch((NetworkID)_currentMatchID, OnMatchDestroyed);
                _disconnectServer = true;
            }
            else
            {
                StopHost();
            }

            
            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(mainMenuPanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }




        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
        }

        public override void OnMatchCreate(UnityEngine.Networking.Match.CreateMatchResponse matchInfo)
        {
            base.OnMatchCreate(matchInfo);

            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        public void OnMatchDestroyed(BasicResponse resp)
        {
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);
            newPlayer.connectionId = conn.connectionId;


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            foreach(var slot in lobbySlots)
            {
                LobbyPlayer lobbyPlayer = slot.GetComponent<LobbyPlayer>();
                if (lobbyPlayer.connectionId == conn.connectionId)
                {
                    GameObject prefab = null;
                    Vector3 spawnPoint = Vector3.zero;
                    Quaternion spawnRot = Quaternion.Euler(0, 90, 0);
                    Cursor.SetCursor(cursors[lobbyPlayer.selectedClass+1], Vector2.one* cursors[lobbyPlayer.selectedClass + 1].width, CursorMode.Auto);
                    switch (lobbyPlayer.selectedClass)
                    {
                        case 0:
                            spawnPoint = FindSpawnPoint(lobbyPlayer.conduitPrefab);
                            prefab = Instantiate(lobbyPlayer.conduitPrefab, spawnPoint, spawnRot) as GameObject;
                            break;
                        case 1:
                            spawnPoint = FindSpawnPoint(lobbyPlayer.aethersmithPrefab);
                            prefab = Instantiate(lobbyPlayer.aethersmithPrefab, spawnPoint, spawnRot) as GameObject;
                            break;
                        case 2:
                            spawnPoint = FindSpawnPoint(lobbyPlayer.calderaPrefab);
                            prefab = Instantiate(lobbyPlayer.calderaPrefab, spawnPoint, spawnRot) as GameObject;
                            break;
                        case 3:
                            spawnPoint = FindSpawnPoint(lobbyPlayer.shardPrefab);
                            prefab = Instantiate(lobbyPlayer.shardPrefab, spawnPoint, spawnRot) as GameObject;
                            break;
                    }
                    if (prefab != null)
                    {
                        playerObjects.Add(conn.connectionId, prefab);
                        return prefab;
                    }
                }
            }

            return base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
        }

        public override void ServerChangeScene(string sceneName)
        {
            base.ServerChangeScene(sceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            GameObject obj = playerObjects[client.connection.connectionId];

            Vector3 spawnPoint = FindSpawnPoint(obj);
            if (spawnPoint != null)
            {
                obj.transform.position = spawnPoint;
                obj.transform.rotation = Quaternion.Euler(0, 90, 0);
                obj.GetComponent<NetworkSyncPosition>().RpcServerSetPosition(spawnPoint);
                obj.GetComponent<NetworkSyncRotation>().RpcSetStartRot();
            }
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                Debug.Log(obj);
                if (obj.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    var spawnPoint = FindSpawnPoint(obj);
                    obj.GetComponent<NetworkSyncPosition>().CmdClientSetServerPos(spawnPoint);
                    obj.GetComponent<NetworkSyncRotation>().CmdSetStartRot();
                }
            }
        }

        public static Vector3 FindSpawnPoint(GameObject obj)
        {
            Vector3 spawnPoint = Vector3.zero;

            if (obj.name.Contains("Conduit"))
            {
                spawnPoint = GameObject.Find("ConduitSpawn").transform.position;
            }
            else if (obj.name.Contains("Aethersmith"))
            {
                spawnPoint = GameObject.Find("AethersmithSpawn").transform.position;
            }
            else if (obj.name.Contains("Caldera"))
            {
                spawnPoint = GameObject.Find("CalderaSpawn").transform.position;
            }
            else if (obj.name.Contains("Shard"))
            {
                spawnPoint = GameObject.Find("ShardSpawn").transform.position;
            }

            return spawnPoint;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
            StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            ServerChangeScene(playScene);
        }

        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            Megamanager manager = GetComponent<Megamanager>();

            if (!manager.isActiveAndEnabled)
            {
                manager.enabled = true;
            }

            base.OnLobbyServerSceneChanged(sceneName);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
            infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }
    }
}
