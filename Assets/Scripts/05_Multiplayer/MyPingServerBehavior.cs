using System.Net;
using UnityEngine;
#if UNITY_2018_3_OR_NEWER
using Unity.Networking.Transport;
using Unity.Collections;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using Unity.Jobs;
using UnityEngine.Assertions;
#endif

public class MyPingServerBehavior : MonoBehaviour
{
#if UNITY_2019_1_OR_NEWER
    public UdpCNetworkDriver m_driver;
    public NativeList<NetworkConnection> m_connections;
    private JobHandle ServerJobHandle;

    // Start is called before the first frame update
    void Start()
    {
        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        m_driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        ushort port = 9000;        
        if (m_driver.Bind(new IPEndPoint(IPAddress.Any, port)) != 0)
            Debug.Log("Failed to bind to port " + port);
        else
            m_driver.Listen();
    }

    void OnDestroy()
    {
        ServerJobHandle.Complete();
        m_driver.Dispose();
        m_connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        ServerJobHandle.Complete();

        var connectionJob = new ServerUpdateConnectionJob
        {
            driver = m_driver,
            connections = m_connections
        };

        var serverUpdateJob = new ServerUpdateJob
        {
            driver = m_driver.ToConcurrent(),
            connections = m_connections.ToDeferredJobArray()
        };

        ServerJobHandle = m_driver.ScheduleUpdate();
        ServerJobHandle = connectionJob.Schedule(ServerJobHandle);
#if UNITY_2018_3_OR_NEWER
        ServerJobHandle = serverUpdateJob.Schedule(m_connections.Length, 1, ServerJobHandle);
#else
        ServerJobHandle = serverUpdateJob.Schedule(m_connections, 1, ServerJobHandle);
#endif
    }
}

struct ServerUpdateJob : IJobParallelFor
{
    public UdpCNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

    public void Execute(int index)
    {
        if (!connections[index].IsCreated)
            Assert.IsTrue(true);

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connections[index], out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint number = stream.ReadUInt(ref readerCtx);
                Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                number += 2;

                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(number);
                    driver.Send(connections[index], writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client Disconnected from server.");
                connections[index] = default;
            }
        }
    }
}

struct ServerUpdateConnectionJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // Clean up connections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept New Connection
        NetworkConnection c;
        while ((c = driver.Accept()) != default)
        {
            connections.Add(c);
            Debug.Log("Accepted a new connection");
        }

        // Process recieved data
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
                continue;

            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    uint number = stream.ReadUInt(ref readerCtx);
                    Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    using (var writer = new DataStreamWriter(4, Allocator.Temp))
                    {
                        writer.Write(number);
                        driver.Send(connections[i], writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client Disconnected from server.");
                    connections[i] = default;
                }
            }
        }
    }
#endif
}
