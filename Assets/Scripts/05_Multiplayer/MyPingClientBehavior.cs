using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;

public class MyPingClientBehavior : MonoBehaviour
{
    struct PendingPing
    {
        public int id;
        public float time;
    }

    private UdpNetworkDriver m_driver;
    private NativeArray<NetworkConnection> m_connection;
    // pendingPings is an array of pings sent to the server which have not yet received a response.
    // Currently it only supports one ping in-flight
    private NativeArray<PendingPing> m_pendingPings;
    // The ping stats are two integers, time for last ping and number of pings
    private NativeArray<int> m_pingStats;
    // The EndPoint the ping client should ping, will be a non-created end point when ping should not run.
    private NetworkEndPoint m_serverEndPoint;

    private JobHandle m_updateHandle;

    private int m_currentPingCount = 0;     // To show a message when a ping is responded

    public InputField m_clientOutput;
    public Button m_pingButton;


    // Start is called before the first frame update
    void Start()
    {
        // Create a NetworkDriver for the client. We could bind to a specific address but in this case we rely on the
        // implicit bind since we do not need to bing to anything special
        m_driver = new UdpNetworkDriver(new INetworkParameter[0]);

        m_pendingPings = new NativeArray<PendingPing>(64, Allocator.Persistent);
        m_pingStats = new NativeArray<int>(2, Allocator.Persistent);
        m_connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent); // Only support one ping in-flight like stated above
        m_serverEndPoint = default;
    }

    void OnDestroy()
    {
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_driver.Dispose();
        m_pendingPings.Dispose();
        m_pingStats.Dispose();
        m_connection.Dispose();
    }

    [BurstCompile]
    struct PingJob : IJob
    {
        public UdpNetworkDriver driver;
        public NativeArray<NetworkConnection> connection;
        public NetworkEndPoint serverEndPoint;
        public NativeArray<PendingPing> pendingPings;
        public NativeArray<int> pingStats;
        public float fixedTime;

        public void Execute()
        {
            // If endpoint shows pings should be sent but do not have an active connection, create one
            if (serverEndPoint.IsValid && !connection[0].IsCreated)
                connection[0] = driver.Connect(serverEndPoint);

            // If endpoint shows no ping should be sent but there is an active connection, close it
            if (!serverEndPoint.IsValid && connection[0].IsCreated)
            {
                connection[0].Disconnect(driver);
                connection[0] = default;
            }

            DataStreamReader strm;
            NetworkEvent.Type cmd;
            // Process all events on the connection. If the connection is invalid it will return Empty immediately
            while ((cmd = connection[0].PopEvent(driver, out strm)) != NetworkEvent.Type.Empty)
            {
                // Once connected, start sending data to the server
                if (cmd == NetworkEvent.Type.Connect)
                {
                    // Update the number of sent pings
                    pingStats[0]++;
                    // Set the ping id to a sequence number for the new ping we are about to send
                    pendingPings[0] = new PendingPing
                    {
                        id = pingStats[0],
                        time = fixedTime
                    };
                    // Create a 4 byte data stream to store the ping sequence number in
                    var pingData = new DataStreamWriter(4, Allocator.Temp);
                    pingData.Write(pingStats[0]);
                    connection[0].Send(driver, pingData);
                }
                // Once the message is received, calculate the ping time and disconnect from the server
                else if (cmd == NetworkEvent.Type.Data)
                {
                    pingStats[1] = (int)((fixedTime - pendingPings[0].time) * 1000);
                    connection[0].Disconnect(driver);
                    connection[0] = default;
                }
                // Clear out connection if the server is disconnected
                else if (cmd == NetworkEvent.Type.Disconnect)
                    connection[0] = default;
            }
        }
    }

    private void LateUpdate()
    {
        // On fast clients each fixed update can have more than 4 frames, this call prevents warnings about TempJob allocation longer than 4 frames in those cases
        m_updateHandle.Complete();
    }

    private void FixedUpdate()
    {
        // Wait for the previous frames ping to complete before starting a new one, the Complete in LateUpdate is not enough since there are multiple FixedUpdate per frame on slow clients
        m_updateHandle.Complete();

        if (m_pingStats[0] != m_currentPingCount && m_pingStats[1] > 0)
        {
            m_currentPingCount = m_pingStats[0];
            ShowMessage("<color=green>Ping " + m_pingStats[0] + " receives reponse from Server. Time: " + m_pingStats[1] + "ms</color>");
        }

        // Update the ping statistics computed by the job scheduled previous frame since that is now guaranteed to have completed
        var pingJob = new PingJob
        {
            driver = m_driver,
            connection = m_connection,
            pendingPings = m_pendingPings,
            pingStats = m_pingStats,
            fixedTime = Time.fixedTime,
            serverEndPoint = m_serverEndPoint
        };

        m_updateHandle = m_driver.ScheduleUpdate();
        m_updateHandle = pingJob.Schedule(m_updateHandle);
    }

    private void Update()
    {
        m_pingButton.GetComponentInChildren<Text>().text = m_serverEndPoint.IsValid ? "Stop" : "Start";
    }

    public void OnTogglePing()
    {
        if (m_serverEndPoint.IsValid)
        {
            m_serverEndPoint = default;
            ShowMessage("Stop Ping.");
        }
        else
        {
            var endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 9000;
            m_serverEndPoint = endpoint;
            ShowMessage("Start Ping.");
        }
    }

    private void ShowMessage(string msg)
    {
        m_clientOutput.text += msg + "\n";
        Debug.Log(msg);
    }
}
