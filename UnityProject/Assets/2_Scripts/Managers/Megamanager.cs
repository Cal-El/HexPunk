using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Megamanager : MonoBehaviour {

    public static Megamanager MM;
    public ClassAbilities[] players;
    public List<Character> characters;
    [System.Serializable]
    public class RoomConnection {
        public Room r1;
        public Room r2;
        public DoorScript door;

        public void UnlockConnection() {
            if(door != null) door.open = true;
            if (r1 != null) r1.UnlockRoom();
            if (r2 != null) r2.UnlockRoom();
        }
    }
    public RoomConnection[] roomTree;

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
        players = FindObjectsOfType<ClassAbilities>();

        if (characters.Count == 0 && AllRoomsUnlocked()) Victory();

        int deadPlayers = 0;
        foreach (ClassAbilities player in players)
        {
            if (!player.IsAlive) deadPlayers++;
        }

        if (deadPlayers == players.Length) Defeated();

    }
    
    private void Defeated()
    {
        foreach(ClassAbilities player in players)
        {
            player.GetComponent<PlayerCommands>().Defeat = true;
        }
    }

    private void Victory()
    {
        foreach (ClassAbilities player in players)
        {
            player.GetComponent<PlayerCommands>().Victory = true;
        }
    }

    public void UnlockConnection(int ID) {
        roomTree[ID].UnlockConnection();
        Debug.Log("Unlocking Connection "+ID);
    }

    void OnDestroy() {
        MM.roomTree = this.roomTree;
    }

    bool AllRoomsUnlocked() {
        return false;
    }

    public static Character FindClosestAttackable(Character toMe, int passNum) {
        SortedDictionary<float, Character> listOfThings = new SortedDictionary<float, Character>();
        foreach (Character g in GetAllCharacters()) {
            if (g != toMe) {
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

    public static void AddCharacterToList(Character c) {
        if (MM.characters == null) MM.characters = new List<Character>();
        MM.characters.Add(c);
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
