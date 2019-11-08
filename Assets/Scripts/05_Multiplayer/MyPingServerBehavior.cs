using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;

public class MyPingServerBehavior : MonoBehaviour
{
    public UdpNetworkDriver m_driver;
    public NativeList<NetworkConnection> m_connections;

    private JobHandle m_updateHandle;

    public InputField m_serverOutput;

    // Start is called before the first frame update
    void Start()
    {
        ushort port = 9000;
        m_driver = new UdpNetworkDriver(new INetworkParameter[0]);

        var addr = NetworkEndPoint.AnyIpv4;
        addr.Port = port;
        if (m_driver.Bind(addr) != 0)
            Debug.Log("Failed to bind to port " + port);
        else
            m_driver.Listen();

        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    void OnDestroy()
    {
        m_updateHandle.Complete();
        m_driver.Dispose();
        m_connections.Dispose();
    }

    [BurstCompile]
    struct DriverUpdateJob : IJob
    {
        public UdpNetworkDriver driver;
        public NativeList<NetworkConnection> connections;

        public void Execute()
        {
            // Remove connections that have been destroyed from the list of active connections
            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].IsCreated)
                {
                    connections.RemoveAtSwapBack(i);
                    i--;
                }
            }

            // Accept all new connections
            while (true)
            {
                var con = driver.Accept();
                // "Nothing more to accept" is signaled by returning an invalid connection from accept
                if (!con.IsCreated)
                    break;
                connections.Add(con);
            }
        }
    }

    [BurstCompile]
#if ENABLE_IL2CPP
    struct PongJob : IJob
    {
        public UdpNetworkDriver.Concurrent driver;
        public NativeList<NetworkConnection> connections;

        public void Execute()
        {
            for (int i = 0; i < connections.Length; i++)
                connections[i] = ProcessSingleConnection(driver, connections[i]);
        }
    }
#else
    struct PongJob : IJobParallelForDefer
    {
        public UdpNetworkDriver.Concurrent driver;
        public NativeArray<NetworkConnection> connections;

        public void Execute(int i)
        {
            connections[i] = ProcessSingleConnection(driver, connections[i]);
        }
    }
#endif

    static NetworkConnection ProcessSingleConnection(UdpNetworkDriver.Concurrent driver, NetworkConnection connection)
    {
        DataStreamReader reader;
        NetworkEvent.Type cmd;

        // Pop all events for the connection
        while ((cmd = driver.PopEventForConnection(connection, out reader)) != NetworkEvent.Type.Empty)
        {
            // Reply a ping requests with a pong message
            if (cmd == NetworkEvent.Type.Data)
            {
                // A DataStreamReader.Context is required to keep track of current read position since DataStreamReader is immutable
                var readerCtx = default(DataStreamReader.Context);
                int id = reader.ReadInt(ref readerCtx);

                // create a temporary DataStreamWriter to keep the serialized pong message
                var pongData = new DataStreamWriter(4, Allocator.Temp);
                pongData.Write(id);

                // Send the pong message with the same id as the ping
                driver.Send(NetworkPipeline.Null, connection, pongData);
            }
            // When disconnected, connection return false to IsCreated so the next frames DriverUpdateJob will remove it
            else if (cmd == NetworkEvent.Type.Disconnect)
                return default(NetworkConnection);
        }

        return connection;
    }

    void LateUpdate()
    {
        m_updateHandle.Complete();
    }

    void FixedUpdate()
    {
        m_updateHandle.Complete();

        // If at least one client is connected, update the activity so the server does not shut down
        if (m_connections.Length > 0)
            DedicatedServerConfig.UpdateLastActivity();
        var updateJob = new DriverUpdateJob {
            driver = m_driver,
            connections = m_connections
        };
        var pongJob = new PongJob {
            driver = m_driver.ToConcurrent(),
#if ENABLE_IL2CPP
            // IJobParallelForDeferExtensions is not working correctly with IL2CPP
            connections = m_connections
#else
            connections = m_connections.AsDeferredJobArray()
#endif
        };

        m_updateHandle = m_driver.ScheduleUpdate();             // Update the driver first
        m_updateHandle = updateJob.Schedule(m_updateHandle);    // Acceipt new connections in DriverUpdateJob. It depends on the driver update job

        // PongJob is the last job in the chain and it depends on the driver update job as well
#if ENABLE_IL2CPP
        m_updateHandle = pongJob.Schedule(m_updateHandle);
#else
        m_updateHandle = pongJob.Schedule(m_connections, 1, m_updateHandle);
#endif
    }
}


