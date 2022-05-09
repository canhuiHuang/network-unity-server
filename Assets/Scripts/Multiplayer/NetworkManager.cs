using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public enum ServerToClientId: ushort {
    playerSpawned = 1,
}

public enum ClientToServerId: ushort {
    name = 1,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;

    public static NetworkManager Singleton {
        get => _singleton;
        private set {
            if ( _singleton == null )
                _singleton = value;
                else if ( _singleton != value) {
                    Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate");
                    Destroy(value);
                }
        }
    }

    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake() {
        Singleton = this;
    }

    private void Start() {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    private void FixedUpdate() {
        Server.Tick();
    }

    private void onApplicationQuit() {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e) {
        Debug.Log($"dced");
        Destroy(Player.list[e.Id].gameObject);
    }
}
