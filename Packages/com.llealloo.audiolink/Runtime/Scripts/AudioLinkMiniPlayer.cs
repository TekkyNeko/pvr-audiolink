
//using System;
//using PVR.CCK.Worlds.Components;
//using PVR.PSharp;
//using UnityEngine;
//using UnityEngine.Video;

//namespace AudioLink
//{
//    [AddComponentMenu("AudioLink/AudioLink Mini Player")]
//    public class AudioLinkMiniPlayer : PSharpBehaviour
//    {
//        [Header("Options")]
//        [Tooltip("Optional default URL to play on world load")]
//        public string defaultUrl;

//        [Tooltip("Whether player controls are locked to master and instance owner by default")]
//        public bool defaultLocked = false;

//        public bool retryOnError = true;

//        [Tooltip("Write out video player events to VRChat log")]
//        public bool debugLogging = true;

//        [Tooltip("Automatically loop track when finished")]
//        public bool loop = false;

//        [Header("Internal")]
//        [Tooltip("Use this texture as an input to materials and other shader systems like LTCGI.")]
//        public CustomRenderTexture videoRenderTexture;
//        [Tooltip("AVPro video player component")]
//        public PVR_VideoProvider avProVideo;

//        float retryTimeout = 6;
//        float syncFrequency = 5;
//        float syncThreshold = 1;

//        [PSharpSynced(SyncType.Manual)]
//        string _syncUrl;
//        string _queuedUrl;

//        [PSharpSynced(SyncType.Manual)]
//        int _syncVideoNumber;
//        int _loadedVideoNumber;

//        /// <summary>
//        /// Synced via <see cref="_flags"/>
//        /// </summary>
//        [NonSerialized]
//        public bool _syncOwnerPlaying;

//        [PSharpSynced(SyncType.Manual)]
//        float _syncVideoStartNetworkTime;

//        /// <summary>
//        /// Synced via <see cref="_flags"/>
//        /// </summary>
//        bool _syncLocked = true;

//        [PSharpSynced(SyncType.Manual)]
//        byte _flags = 0b10;

//        [NonSerialized]
//        public int localPlayerState = PLAYER_STATE_STOPPED;
//        [NonSerialized]
//        //public VideoError localLastErrorCode;

//        PVR_VideoProvider _currentPlayer;

//        float _lastVideoPosition = 0;
//        float _videoTargetTime = 0;

//        bool _waitForSync;
//        float _lastSyncTime;
//        float _playStartTime = 0;

//        float _pendingLoadTime = 0;
//        float _pendingPlayTime = 0;
//        string _pendingPlayUrl;

//        // Realtime state

//        [NonSerialized]
//        public bool seekableSource;
//        [NonSerialized]
//        public float trackDuration;
//        [NonSerialized]
//        public float trackPosition;
//        [NonSerialized]
//        public bool locked;

//        // Constants

//        const int PLAYER_STATE_STOPPED = 0;
//        const int PLAYER_STATE_LOADING = 1;
//        const int PLAYER_STATE_PLAYING = 2;
//        const int PLAYER_STATE_ERROR = 3;

//        void Start()
//        {
//            //avProVideo.Loop = false;
//            avProVideo.videoPlayer.Stop();

//            _currentPlayer = avProVideo;

//            CopyIntoFlags();

//            if (PSharpNetworking.IsOwner(gameObject))
//            {
//                _syncLocked = defaultLocked;
//                locked = _syncLocked;
//                Sync("_flags");

//                _PlayVideo(defaultUrl);
//            }
//        }

//        /// <summary>
//        /// Copys all the associated bools into the flags byte
//        /// </summary>
//        private void CopyIntoFlags()
//        {
//            _flags = 0;
//            if (_syncOwnerPlaying)
//                _flags |= 1;
//            if (_syncLocked)
//                _flags |= 2;
//        }

//        /// <summary>
//        /// Copys all the associated bools out of the flags byte into the associated bools
//        /// </summary>
//        private void CopyOutOfFlags()
//        {
//            _syncOwnerPlaying = (_flags & 1) != 0;
//            _syncLocked = (_flags & 2) != 0;
//        }

//        public void _TriggerPlay()
//        {
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Trigger play");
//            if (localPlayerState == PLAYER_STATE_PLAYING || localPlayerState == PLAYER_STATE_LOADING)
//                return;

//            _PlayVideo(_syncUrl);
//        }

//        public void _TriggerStop()
//        {
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Trigger stop");
//            if (_syncLocked && !_CanTakeControl())
//                return;
//            if (!IsOwner)
//                PSharpNetworking.SetOwner(PSharpPlayer.LocalPlayer, gameObject);

//            _StopVideo();
//        }

//        public void _TriggerLock()
//        {
//            if (!_IsAdmin())
//                return;
//            if (localPlayerState != PLAYER_STATE_PLAYING)
//                return;

//            if (!IsOwner)
//                PSharpNetworking.SetOwner(PSharpPlayer.LocalPlayer, gameObject);

//            _syncLocked = !_syncLocked;
//            locked = _syncLocked;
//            Sync("_flags");
//        }

//        public void _Resync()
//        {
//            _ForceResync();
//        }

//        public void _ChangeUrl(string url)
//        {
//            if (_syncLocked && !_CanTakeControl())
//                return;

//            _PlayVideo(url);

//            _queuedUrl = "";
//        }

//        public void _UpdateQueuedUrl(string url)
//        {
//            if (_syncLocked && !_CanTakeControl())
//                return;
//            if (!IsOwner)
//                PSharpNetworking.SetOwner(PSharpPlayer.LocalPlayer, gameObject);

//            _queuedUrl = url;
//        }

//        public void _SetTargetTime(float time)
//        {
//            if (_syncLocked && !_CanTakeControl())
//                return;
//            if (!IsOwner)
//                PSharpNetworking.SetOwner(PSharpPlayer.LocalPlayer, gameObject);

//            //_syncVideoStartNetworkTime = (float)PSharpNetworking.Server() - time;
//            SyncVideo();
//            Sync(nameof(_syncVideoStartNetworkTime));
//        }

//        void _PlayVideo(string url)
//        {
//            _pendingPlayTime = 0;
//            if (!_IsUrlValid(url))
//                return;

//            string message = "Play video " + url;
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] " + message);
//            bool isOwner = IsOwner;
//            if (!isOwner && !_CanTakeControl())
//                return;

//            if (!isOwner)
//                PSharpNetworking.SetOwner(PSharpPlayer.LocalPlayer, gameObject);

//            _syncUrl = url;
//            _syncVideoNumber += isOwner ? 1 : 2;
//            _loadedVideoNumber = _syncVideoNumber;
//            _syncOwnerPlaying = false;

//            _syncVideoStartNetworkTime = float.MaxValue;
//            //RequestSerialization();

//            _videoTargetTime = _ParseTimeFromUrl(url);

//            _StartVideoLoad();
//        }

//        public void _LoopVideo()
//        {
//            _PlayVideo(_syncUrl);
//        }

//        public void _PlayQueuedUrl()
//        {
//            _PlayVideo(_queuedUrl);
//            _queuedUrl = "";
//        }

//        bool _IsUrlValid(string url)
//        {
//            //if (!VRC.SDKBase.Utilities.IsValid(url))
//            //    return false;

//            string urlStr = url;
//            if (urlStr == null || urlStr == "")
//                return false;

//            return true;
//        }

//        // Time parsing code adapted from USharpVideo project by Merlin
//        float _ParseTimeFromUrl(string urlStr)
//        {
//            // Attempt to parse out a start time from YouTube links with t= or start=
//            if (!urlStr.Contains("youtube.com/watch") && !urlStr.Contains("youtu.be/"))
//                return 0;

//            int tIndex = urlStr.IndexOf("?t=");
//            if (tIndex == -1)
//                tIndex = urlStr.IndexOf("&t=");
//            if (tIndex == -1)
//                tIndex = urlStr.IndexOf("?start=");
//            if (tIndex == -1)
//                tIndex = urlStr.IndexOf("&start=");
//            if (tIndex == -1)
//                return 0;

//            char[] urlArr = urlStr.ToCharArray();
//            int numIdx = urlStr.IndexOf('=', tIndex) + 1;

//            string intStr = "";
//            while (numIdx < urlArr.Length)
//            {
//                char currentChar = urlArr[numIdx];
//                if (!char.IsNumber(currentChar))
//                    break;

//                intStr += currentChar;
//                ++numIdx;
//            }

//            if (intStr.Length == 0)
//                return 0;

//            int secondsCount = 0;
//            if (!int.TryParse(intStr, out secondsCount))
//                return 0;

//            return secondsCount;
//        }

//        void _StartVideoLoadDelay(float delay)
//        {
//            _pendingLoadTime = Time.time + delay;
//        }

//        void _StartVideoLoad()
//        {
//            _pendingLoadTime = 0;
//            if (_syncUrl == null || _syncUrl == "")
//                return;

//            string message = "Start video load " + _syncUrl;
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] " + message);
//            _UpdatePlayerState(PLAYER_STATE_LOADING);

//#if !UNITY_EDITOR
//            _currentPlayer.LoadURL(_syncUrl);
//#endif
//        }

//        public void _StopVideo()
//        {
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Stop video");

//            //if (seekableSource)
//            //    _lastVideoPosition = _currentPlayer.GetTime();

//            _UpdatePlayerState(PLAYER_STATE_STOPPED);

//            _currentPlayer.videoPlayer.Stop();
//            _videoTargetTime = 0;
//            _pendingPlayTime = 0;
//            _pendingLoadTime = 0;
//            _playStartTime = 0;

//            if (IsOwner)
//            {
//                _syncVideoStartNetworkTime = 0;
//                _syncOwnerPlaying = false;
//                _syncUrl = "";
//                Sync("_syncVideoStartNetworkTime");
//                Sync("_syncOwnerPlaying");
//                Sync("_syncUrl");
//            }
//        }

//        public void OnVideoReady()
//        {
//            float duration = _currentPlayer.videoPlayer.frameCount / _currentPlayer.videoPlayer.frameRate;
//            string message = "Video ready, duration: " + duration + ", position: ";
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] " + message);

//            // If a seekable video is loaded it should have a positive duration.  Otherwise we assume it's a non-seekable stream
//            seekableSource = !float.IsInfinity(duration) && !float.IsNaN(duration) && duration > 1;

//            // If player is owner: play video
//            // If Player is remote:
//            //   - If owner playing state is already synced, play video
//            //   - Otherwise, wait until owner playing state is synced and play later in update()
//            //   TODO: Streamline by always doing this in update instead?

//            if (IsOwner)
//                _currentPlayer.videoPlayer.Play();
//            else
//            {
//                // TODO: Stream bypass owner
//                if (_syncOwnerPlaying)
//                    _currentPlayer.videoPlayer.Play();
//                else
//                    _waitForSync = true;
//            }
//        }

//        public void OnVideoStart()
//        {
//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Video start");

//            if (IsOwner)
//            {
//                _UpdatePlayerState(PLAYER_STATE_PLAYING);
//                _playStartTime = Time.time;

//                //_syncVideoStartNetworkTime = (float)Networking.GetServerTimeInSeconds() - _videoTargetTime;
//                _syncOwnerPlaying = true;
//                //RequestSerialization();

//                //_currentPlayer.SetTime(_videoTargetTime);
//            }
//            else
//            {
//                if (!_syncOwnerPlaying)
//                {
//                    // TODO: Owner bypass
//                    _currentPlayer.videoPlayer.Pause();
//                    _waitForSync = true;
//                }
//                else
//                {
//                    _UpdatePlayerState(PLAYER_STATE_PLAYING);
//                    _playStartTime = Time.time;

//                    SyncVideo();
//                }
//            }
//        }

//        public void OnVideoEnd()
//        {
//            if (!seekableSource && Time.time - _playStartTime < 1)
//            {
//                Debug.Log("[AudioLink] Video end encountered at start of stream, ignoring");
//                return;
//            }

//            _UpdatePlayerState(PLAYER_STATE_STOPPED);
//            seekableSource = false;

//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Video end");
//            _lastVideoPosition = 0;

//            if (IsOwner)
//            {
//                if (_IsUrlValid(_queuedUrl))
//                    SendNetworkedEvent(NetworkEventTarget.Local, "_PlayQueuedUrl");
//                else if (loop)
//                    SendNetworkedEvent(NetworkEventTarget.Local, "_LoopVideo");
//                else
//                {
//                    _syncVideoStartNetworkTime = 0;
//                    _syncOwnerPlaying = false;
//                    Sync("_syncVideoStartNetworkTime");
//                    Sync("_syncOwnerPlaying");
//                }
//            }
//        }

//        //public void OnVideoError(VideoError videoError)
//        //{
//        //    _currentPlayer.Stop();

//        //    string message = "Video stream failed: " + _syncUrl;
//        //    if (debugLogging)
//        //        Debug.Log("[AudioLink:MiniPlayer] " + message);
//        //    string message1 = "Error code: " + videoError;
//        //    if (debugLogging)
//        //        Debug.Log("[AudioLink:MiniPlayer] " + message1);

//        //    _UpdatePlayerState(PLAYER_STATE_ERROR);
//        //    localLastErrorCode = videoError;

//        //    if (Networking.IsOwner(gameObject))
//        //    {
//        //        if (retryOnError)
//        //        {
//        //            _StartVideoLoadDelay(retryTimeout);
//        //        }
//        //        else
//        //        {
//        //            _syncVideoStartNetworkTime = 0;
//        //            _videoTargetTime = 0;
//        //            _syncOwnerPlaying = false;
//        //            RequestSerialization();
//        //        }
//        //    }
//        //    else
//        //    {
//        //        _StartVideoLoadDelay(retryTimeout);
//        //    }
//        //}

//        public bool _IsAdmin()
//        {
//            PSharpPlayer player = PSharpPlayer.LocalPlayer;
//            if (player.IsNull)
//                return false;

//            return player.IsMaster || player.IsInstanceCreator;
//        }

//        public bool _CanTakeControl()
//        {
//            PSharpPlayer player = PSharpPlayer.LocalPlayer;
//            if (player.IsNull)
//                return false;

//            return player.IsMaster || player.IsInstanceCreator || !_syncLocked;
//        }
        
//        public override void OnPreSerialization()
//        {
//            CopyIntoFlags();
//        }

//        public override void OnDeserialization()
//        {
//            if (IsOwner)
//                return;
            
//            CopyOutOfFlags();

//            if (debugLogging)
//            {
//                Debug.Log($"[AudioLink:MiniPlayer] Deserialize: video #{_syncVideoNumber}");
//            }

//            locked = _syncLocked;

//            if (_syncVideoNumber == _loadedVideoNumber)
//            {
//                if (localPlayerState == PLAYER_STATE_PLAYING && !_syncOwnerPlaying)
//                {
//                    SendNetworkedEvent(NetworkEventTarget.Local, "_StopVideo");
//                }
//                return;
//            }

//            // There was some code here to bypass load owner sync bla bla

//            _loadedVideoNumber = _syncVideoNumber;

//            if (debugLogging)
//                Debug.Log("[AudioLink:MiniPlayer] Starting video load from sync");

//            _StartVideoLoad();
//        }

//        public override void OnPostSerialization(SerializationResult result)
//        {
//            if (!result.success)
//            {
//                if (debugLogging)
//                    Debug.Log("[AudioLink:MiniPlayer] Failed to sync");
//            }
//        }

//        void Update()
//        {
//            bool isOwner = IsOwner;
//            float time = Time.time;

//            if (_pendingPlayTime > 0 && time > _pendingPlayTime)
//                _PlayVideo(_pendingPlayUrl);
//            if (_pendingLoadTime > 0 && Time.time > _pendingLoadTime)
//                _StartVideoLoad();

//            if (seekableSource && localPlayerState == PLAYER_STATE_PLAYING)
//            {
//                //trackDuration = _currentPlayer.GetDuration();
//                //trackPosition = _currentPlayer.GetTime();
//            }

//            // Video is playing: periodically sync with owner
//            if (isOwner || !_waitForSync)
//            {
//                SyncVideoIfTime();
//                return;
//            }

//            // Video is not playing, but still waiting for go-ahead from owner
//            if (!_syncOwnerPlaying)
//                return;

//            // Got go-ahead from owner, start playing video
//            _UpdatePlayerState(PLAYER_STATE_PLAYING);

//            _waitForSync = false;
//            _currentPlayer.videoPlayer.Play();

//            SyncVideo();
//        }

//        void SyncVideoIfTime()
//        {
//            if (Time.realtimeSinceStartup - _lastSyncTime > syncFrequency)
//            {
//                _lastSyncTime = Time.realtimeSinceStartup;
//                SyncVideo();
//            }
//        }

//        void SyncVideo()
//        {
//            if (seekableSource)
//            {
//                float offsetTime = Mathf.Clamp((float)_syncVideoStartNetworkTime, 0f, 0);
//                if (Mathf.Abs(_currentPlayer.GetTime() - offsetTime) > syncThreshold)
//                    _currentPlayer.SetTime(offsetTime);
//            }
//        }

//        public void _ForceResync()
//        {
//            bool isOwner = Networking.IsOwner(gameObject);
//            if (isOwner)
//            {
//                if (seekableSource)
//                {
//                    float startTime = _videoTargetTime;
//                    if (_currentPlayer.IsPlaying)
//                        startTime = _currentPlayer.GetTime();

//                    _StartVideoLoad();
//                    _videoTargetTime = startTime;
//                }
//                return;
//            }

//            _currentPlayer.Stop();
//            if (_syncOwnerPlaying)
//                _StartVideoLoad();
//        }

//        void _UpdatePlayerState(int state)
//        {
//            localPlayerState = state;

//            if (VRC.SDKBase.Utilities.IsValid(videoRenderTexture))
//            {
//                if (state == PLAYER_STATE_PLAYING)
//                    videoRenderTexture.updateMode = CustomRenderTextureUpdateMode.Realtime;
//                else
//                    videoRenderTexture.updateMode = CustomRenderTextureUpdateMode.OnDemand;
//            }
//        }
//    }
//}
