using System.Net;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using Unity.Jobs;

public class MyPingClientBehavior : MonoBehaviour
{
    public UdpCNetworkDriver m_driver;
    public NativeArray<NetworkConnection> m_connection;
    public NativeArray<byte> m_isDone;
    public JobHandle ClientJobHandle;    

    // Start is called before the first frame update
    void Start()
    {
        ushort port = 9000;

        m_driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        m_isDone = new NativeArray<byte>(1, Allocator.Persistent);

        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        m_connection[0] = m_driver.Connect(endpoint);
    }

    void OnDestroy()
    {
        ClientJobHandle.Complete();

        m_driver.Dispose();
        m_connection.Dispose();
        m_isDone.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        ClientJobHandle.Complete();

        var job = new ClientUpdateJob
        {
            driver = m_driver,
            connection = m_connection,
            isDone = m_isDone
        };

        ClientJobHandle = m_driver.ScheduleUpdate();
        ClientJobHandle = job.Schedule(ClientJobHandle);
    }
}

struct ClientUpdateJob: IJob
{
    public UdpCNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> isDone;

    public void Execute()
    {
        if (!connection[0].IsCreated)
        {
            if (isDone[0] != 1)
                Debug.Log("Something went wrong during connection.");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection[0].PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("we are now connected to the server.");

                var value = 1;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    connection[0].Send(driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);
                Debug.Log("Got the value = " + value + " back from the server");
                isDone[0] = 1;
                connection[0].Disconnect(driver);
                connection[0] = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client is disconnected from the server");
                connection[0] = default;
            }
        }
    }
}
