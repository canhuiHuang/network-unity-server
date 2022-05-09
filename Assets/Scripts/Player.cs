using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id {get; private set;}
    public string Username {get; private set;}

    private void OnDestroy() {
        Debug.Log($"dced onDestroy");
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username) {

        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }


        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f,1f,0f), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username)? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username)? $"Guest {id}" : username;

        player.SendSpawned();
        list.Add(id, player);
    }

    #region Messages
    private void SendSpawned() {
        Message spawnData = AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned));
        NetworkManager.Singleton.Server.SendToAll(spawnData);
    }

    private void SendSpawned(ushort toClientId) {
        Message spawnData = AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned));
        NetworkManager.Singleton.Server.Send(spawnData, toClientId);
    }

    private Message AddSpawnData(Message message) {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message) {
        Spawn(fromClientId, message.GetString());
    }
    #endregion
}
