using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Megamanager : NetworkBehaviour { 

    public static Megamanager MM;
    public ClassAbilities[] players;
    [SyncVar (hook = "OnDeadPlayersChanged")]
    public int deadPlayers;
    public List<Character> characters;
    public bool SceneHasChanged = false;
    [System.Serializable]
    public class RoomConnection {
        public Room r1;
        public Room r2;
        public DoorScript door;

        public void UnlockConnection() {
            if(door != null) door.UnlockDoor();
            if (r1 != null) r1.UnlockRoom();
            if (r2 != null) r2.UnlockRoom();
        }
    }
    public RoomConnection[] roomTree;
    private bool allPlayersInList = false;


	// Use this for initialization
	void Awake () {
        if (MM != null) {
            Destroy(this);
        } else {
            MM = this;
            DontDestroyOnLoad(this.gameObject);
        }
        players = FindObjectsOfType<ClassAbilities>();
    }
	
	// Update is called once per frame
	void Update () {

        if (!allPlayersInList) {
            players = FindObjectsOfType<ClassAbilities>();
            if (players.Length >= FindObjectOfType<NetworkLobbyManager>().minPlayers) {
                allPlayersInList = true;
                for(int i = 0; i < players.Length; i++) {
                    players[i].ID = i; 
                }
            }
        }
    }

    [ServerCallback]
    public void AddDeadPlayer(int value)
    {
        deadPlayers += value;
    }

    private void OnDeadPlayersChanged(int value)
    {
        deadPlayers = value;
        //If 3 players are dead and betrayer is alive
        if (deadPlayers >= players.Length - 1)
        {
            bool alliesDead = true;
            foreach (ClassAbilities player in players)
            {
                bool isBetrayer = player.GetComponent<PlayerCommands>().IsBetrayer;
                if (!isBetrayer)
                {
                    if (alliesDead)
                    {
                        alliesDead = !player.IsAlive;
                    }
                }
            }
            if(alliesDead) Defeated();
        }        
    }

    [ServerCallback]
    public void Defeated()
    {
        foreach(ClassAbilities player in players)
        {
            var commands = player.GetComponent<PlayerCommands>();
            if (commands != null)
            {
                if (commands.IsBetrayer)
                {
                    commands.Victory = true;
                }
                else
                {
                    commands.Defeat = true;
                }
            }
        }
    }
    
    [ServerCallback]
    public void Victory()
    {
        foreach (ClassAbilities player in players)
        {
            var commands = player.GetComponent<PlayerCommands>();
            if (commands != null)
            {
                if (commands.IsBetrayer)
                {
                    commands.Defeat = true;
                }
                else
                {
                    commands.Victory = true;
                }
            }
        }
    }

    [ServerCallback]
    public void UnlockConnection(int ID) {
        if (roomTree[ID] != null)
        {
            roomTree[ID].UnlockConnection();
            Debug.Log("Unlocking Connection " + ID);
            if(players.Length > 1) 
                RpcUnlockConnection(ID);
        }
    }

    [ClientRpc]
    private void RpcUnlockConnection(int ID)
    {
        if (roomTree[ID] != null)
        {
            roomTree[ID].UnlockConnection();
            Debug.Log("Unlocking Connection " + ID);
        }
    }

    void OnDestroy() {
        MM.SceneHasChanged = true;
        MM.players = FindObjectsOfType<ClassAbilities>();
        MM.roomTree = this.roomTree;
    }

    bool AllRoomsUnlocked() {
        return false;
    }

    public static Character FindClosestAttackable(Character toMe, int passNum) {
        SortedDictionary<float, Character> listOfThings = new SortedDictionary<float, Character>();
        foreach (Character g in GetAllCharacters()) {
            if (g != null && g != toMe) {
                float newValue = Vector3.Distance(toMe.transform.position, g.transform.position);
                if(!listOfThings.ContainsKey(newValue))
                listOfThings.Add(newValue,g);
            }
        }
       
        int passes = 1;
        foreach (Character g in listOfThings.Values) {
            if (passNum == passes)
                return g;
            passes++;
        }
        return null;
    }

    public static int AddCharacterToList(Character c) {
        if (MM.characters == null) { MM.characters = new List<Character>(); }
        MM.characters.Add(c);
        return MM.characters.Count - 1;
    }

    public static void RemoveCharacterFromList(Character c) {
        MM.characters.Remove(c);
    }

    public static Character[] GetAllCharacters() {
        return MM.characters.ToArray() ;
    }

    /// <summary>
    /// It was found that order is not garenteed in RaycastAll
    /// </summary>
    /// <returns>Sorted Array</returns>
    public static RaycastHit[] SortByDistance(RaycastHit[] list) {
        RaycastHit[] returnList = new RaycastHit[list.Length];
        for (int i = 0; i < list.Length; i++) {
            int smallerThanMe = 0;
            for (int j = 0; j < list.Length; j++) {
                if(list[i].distance > list[j].distance) {
                    smallerThanMe++;
                }
            }
            returnList[smallerThanMe] = list[i];
        }
        return returnList;
    }
}
