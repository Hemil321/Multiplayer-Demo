using UnityEngine;
using Agora.Rtc;
using Agora_RTC_Plugin.API_Example;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting.Antlr3.Runtime;

public class AgoraManager : MonoBehaviour
{
    public static AgoraManager Instance { get; private set; }

    [SerializeField] private string appID;
    [SerializeField] private GameObject canvas;

    private string tokenBase = "http://localhost:8080";
    private IRtcEngine RtcEngine;

    private string token = "";
    private string channelName = "Sample";

    private Player player;
    public CONNECTION_STATE_TYPE connectionState = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;
    public Dictionary<string, List<uint>> usersJoinedInAChannel;

    private void Awake()
    {
        Instance = this;
        usersJoinedInAChannel = new Dictionary<string, List<uint>>();
    }

    private void Start()
    {
        InitRtcEngine();
        SetBasicConfiguration();
    }

    private void InitRtcEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext();
        context.appId = appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        context.areaCode = AREA_CODE.AREA_CODE_GLOB;

        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    private void SetBasicConfiguration()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();

        //Setting up Video Configuration
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(640, 360);
        config.frameRate = 15;
        config.bitrate = 0;
        RtcEngine.SetVideoEncoderConfiguration(config);

        RtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    public void JoinChannel()
    {
        //string player1ChannelName = player.GetChannelName();
        //string player2ChannelName = player2.GetChannelName();

        ////If both the players have not joined a channel
        //if (player1ChannelName == "" && player2ChannelName == "")
        //{
        //    player.SetChannelName(GenerateChannelName());
        //}
        ////If both the players are already in a channel
        //else if (player1ChannelName != "" && player2ChannelName != "")
        //{
        //    return;
        //}
        ////If one of the players has joined a channel
        //else if(player2ChannelName != "")
        //{
        //    player.SetToken(player2.GetToken());
        //    player.SetChannelName(player2ChannelName);
        //}

        if (token.Length == 0)
        {
            StartCoroutine(HelperClass.FetchToken(tokenBase, channelName, 0, JoinOrRenewToken));
            return;
        }

        RtcEngine.JoinChannel(token, channelName, "", 0);
        RtcEngine.StartPreview();
        MakeVideoView(0);
    }

    public void LeaveChannel()
    {
        RtcEngine.StopPreview();
        DestroyVideoView(0);
        RtcEngine.LeaveChannel();
    }

    private string GenerateChannelName()
    {
        return GetRandomChannelName(10);
    }

    private void JoinOrRenewToken(string newToken)
    {
        token = newToken;
        if(connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED || connectionState == CONNECTION_STATE_TYPE.CONNECTION_STATE_FAILED)
        {
            JoinChannel();
        }
        else
        {

        }
    }

    private string GetRandomChannelName(int length)
    {
        string characters = "abcdefghijklmnopqrstuvwxyzABCDDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string randomChannelName = "";

        for(int i = 0; i < length; i++)
        {
            randomChannelName += characters[Random.Range(0, characters.Length)];
        }

        return randomChannelName;
    }

    private void DestroyVideoView(uint uid)
    {
        GameObject videoView = GameObject.Find(uid.ToString());
        if (videoView != null)
        {
            Destroy(videoView);
        }
    }

    #region Video View Rendering Logic
    private void MakeVideoView(uint uid, string channelId = "")
    {
        GameObject videoView = GameObject.Find(uid.ToString());
        if (videoView != null)
        {
            //Video view for this user id already exists
            return;
        }

        // create a video surface game object and assign it to the user
        VideoSurface videoSurface = MakeImageSurface(uid.ToString());
        if (videoSurface == null) return;

        // configure videoSurface
        if (uid == 0)
        {
            videoSurface.SetForUser(uid, channelId);
        }
        else
        {
            videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }
        

        videoSurface.OnTextureSizeModify += (int width, int height) =>
        {
            RectTransform transform = videoSurface.GetComponent<RectTransform>();
            if (transform)
            {
                //If render in RawImage. just set rawImage size.
                transform.sizeDelta = new Vector2(width / 2, height / 2);
                transform.localScale = Vector3.one;
            }
            else
            {
                //If render in MeshRenderer, just set localSize with MeshRenderer
                float scale = (float)height / (float)width;
                videoSurface.transform.localScale = new Vector3(-1, 1, scale);
            }
            Debug.LogError("OnTextureSizeModify: " + width + "  " + height);
        };

        videoSurface.SetEnable(true);
    }

    private VideoSurface MakeImageSurface(string goName)
    {
        GameObject gameObject = new GameObject();

        if (gameObject == null)
        {
            return null;
        }

        gameObject.name = goName;
        // to be renderered onto
        gameObject.AddComponent<RawImage>();
        // make the object draggable
        gameObject.AddComponent<UIElementDrag>();
        if (canvas != null)
        {
            //Add the video view as a child of the canvas
            gameObject.transform.parent = canvas.transform;
        }
        else
        {
            Debug.LogError("Canvas is null video view");
        }

        // set up transform
        gameObject.transform.Rotate(0f, 0.0f, 180.0f);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = new Vector3(2f, 3f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = gameObject.AddComponent<VideoSurface>();
        return videoSurface;
    }

    #endregion


    public IRtcEngine GetIrtcEngine()
    {
        return RtcEngine;
    }

    #region User Events
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private AgoraManager agoraManager;
        internal UserEventHandler(AgoraManager agoraManager)
        {
            this.agoraManager = agoraManager;
        }

        public override void OnError(int err, string msg)
        {
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.LogError("Joined channel");
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            //When a player leaves a channel, all the other views present in that channel will be destroyed
            foreach(uint uid in agoraManager.usersJoinedInAChannel[connection.channelId])
            {
                agoraManager.DestroyVideoView(uid);
            }
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {

        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            agoraManager.MakeVideoView(uid, connection.channelId);

            //When a remote user joins a channel, we will add the user in the users pool for that channel
            if(agoraManager.usersJoinedInAChannel.ContainsKey(connection.channelId))
            {
                agoraManager.usersJoinedInAChannel[connection.channelId].Add(uid);
            }
            else
            {
                agoraManager.usersJoinedInAChannel.Add(connection.channelId, new List<uint> { uid });
            }
            
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            //If a remote user goes offline(leaves the channel), we will remove the user's video view
            agoraManager.DestroyVideoView(uid);
        }

        public override void OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            agoraManager.connectionState = state;
        }
    }
    #endregion
}

