using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public enum Reality
{
    MOFATheTraining = 0
}

public class App : MonoBehaviour, INetworkRunnerCallbacks
{
    public static App Instance { get { return _instance; } }

    private static App _instance;

    [SerializeField] private RealityManager[] _realityManagerPrefabs;

    public bool IsMaster => _runner != null && _runner.IsServer;

    public NetworkRunner Runner => _runner;

    public Reality Reality
    {
        get => _reality;
        set
        {
            _reality = value;
        }
    }

    public RealityManager RealityManager => _realityManager;

    public string CurrentSessionCode
    {
        get
        {
            if (_runner != null && _runner.SessionInfo != null)
            {
                return _runner.SessionInfo.Name;
            }
            else
            {
                return null;
            }
        }
    }

    private NetworkRunner _runner;

    private Reality _reality;

    private RealityManager _realityManager;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Connect()
    {
        if (_runner == null)
        {
            GameObject go = new("Runner");
            go.transform.SetParent(transform);

            _runner = go.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
        }
    }

    public void Disconnect()
    {
        if (_runner != null)
        {
            _runner.Shutdown();
        }
    }

    public async void CreateReality(Reality reality)
    {
        _reality = reality;
        Connect();
        _runner.ProvideInput = true;
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = Utils.GetRandomSessionCode(),
            SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok)
        {
            Debug.Log("Failed to create reality");
        }
    }

    public async void JoinReality(string sessionCode)
    {
        Connect();
        _runner.ProvideInput = true;
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = sessionCode,
            DisableClientSessionCreation = true
        });

        if (!result.Ok)
        {
            Debug.Log($"Failed to join reality with session code {sessionCode}");
        }
    }

    public void SetRealityManager(RealityManager realityManager)
    {
        _realityManager = realityManager;
        _realityManager.transform.SetParent(_runner.transform);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[App] OnPlayerJoined {player}");
        if (IsMaster)
        {
            if (_realityManager == null)
            {
                runner.Spawn(_realityManagerPrefabs[(int)_reality], Vector3.zero, Quaternion.identity);

                string arSceneName = _reality.ToString() + "AR";
                _runner.SetActiveScene(arSceneName);
            }
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
}
