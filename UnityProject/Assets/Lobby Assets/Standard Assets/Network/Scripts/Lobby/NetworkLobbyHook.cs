using UnityEngine;
using UnityStandardAssets.Network;
using UnityEngine.Networking;
using System.Collections;

public class NetworkLobbyHook : LobbyHook
{
    public GameObject conduitPrefab;
    public GameObject aethersmithPrefab;
    public GameObject calderaPrefab;
    public GameObject shardPrefab;

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();

        switch (lobby.selectedClass)
        {
            case "ConduitTile":
                gamePlayer = conduitPrefab;
                break;
            case "AethersmithTile":
                gamePlayer = aethersmithPrefab;
                break;
            case "CalderaTile":
                gamePlayer = calderaPrefab;
                break;
            case "ShardTile":
                gamePlayer = shardPrefab;
                break;
        }

        //spaceship.name = lobby.name;
        //spaceship.color = lobby.playerColor;
        //spaceship.score = 0;
        //spaceship.lifeCount = 3;
    }
}
