using System.Threading;
using LemonFrameSync.Logger;
using LemonFramework.ECS;
using LemonFramework.ECS.Actors;
using LemonFramework.ECS.Components;
using LemonFramework.ECS.Entitys;
using LemonFramework.ECS.Replay;
using LemonFramework.ECS.Systems;
using UnityEngine;

namespace LemonFramework.Demo
{
    /// <summary>
    /// 录像功能演示
    /// 支持录制、保存、播放、跳帧、加速播放
    /// </summary>
    public class ReplayDemo : MonoBehaviour
    {
        [Header("ECS设置")]
        [Tooltip("创建的实体数量")] public int entityCount = 5;
        [Tooltip("实体预制体")] public GameObject entityPrefab;
        
        [Header("录像设置")]
        [Tooltip("录像文件保存路径")] public string replayFileName = "gameplay_replay.dat";
        
        private World _ecsWorld;
        private Thread _ecsThread;
        private TimeData _timeData;
        private bool _isRunning = true;
        
        private EntityActorManager _actorManager;
        private Entity _controlledEntity;
        
        // 录像系统
        private ReplayRecorder _recorder;
        private ReplayPlayer _player;
        
        // UI样式
        private GUIStyle _titleStyle;
        private GUIStyle _normalStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _smallButtonStyle;
        
        // 状态标记
        private enum DemoState
        {
            Recording,      // 录制中
            Playing,        // 播放中
            Paused,         // 暂停
            Idle            // 空闲
        }
        private DemoState _currentState = DemoState.Idle;
        
        void Start()
        {
            LogManager.Instance.Initialize();
            InitializeStyles();
            
            // 注册组件类型
            ComponentManager.RegisterComponentType<EcsTransform>();
            
            // 创建Actor管理器
            _actorManager = gameObject.AddComponent<EntityActorManager>();
            
            // 初始化ECS
            InitializeEcs();
            
            // 创建实体
            CreateEntities();
            
            // 初始化录像系统
            _recorder = new ReplayRecorder(_ecsWorld);
            _player = new ReplayPlayer(_ecsWorld);
            
            // 启动ECS线程
            StartEcsThread();
            
            Debug.Log("=== 录像功能演示已启动 ===");
            Debug.Log("使用WASD控制红色立方体");
            Debug.Log("使用右侧按钮控制录像功能");
        }
        
        void InitializeStyles()
        {
            _titleStyle = new GUIStyle
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = Color.yellow }
            };
            
            _normalStyle = new GUIStyle
            {
                fontSize = 14,
                normal = new GUIStyleState { textColor = Color.white }
            };
            
            _buttonStyle = new GUIStyle("button")
            {
                fontSize = 14,
                fixedHeight = 40
            };
            
            _smallButtonStyle = new GUIStyle("button")
            {
                fontSize = 12,
                fixedHeight = 30
            };
        }
        
        void InitializeEcs()
        {
            _ecsWorld = new World("ReplayWorld");
            _timeData = new TimeData(60);
            
            _actorManager.SetWorld(_ecsWorld);
            
            // 添加移动系统
            _ecsWorld.AddSystem(new CircleMovementSystem());
        }
        
        void CreateEntities()
        {
            if (entityPrefab == null)
            {
                entityPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                entityPrefab.transform.localScale = Vector3.one * 0.5f;
            }
            
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(entityCount));
            float spacing = 2f;
            
            for (int i = 0; i < entityCount; i++)
            {
                int x = i % gridSize;
                int z = i / gridSize;
                
                Entity entity = _ecsWorld.CreateEntity();
                
                Vector3 startPos = new Vector3(
                    (x - gridSize / 2f) * spacing,
                    0,
                    (z - gridSize / 2f) * spacing
                );
                
                _ecsWorld.AddComponent(entity, new EcsTransform(startPos, Quaternion.identity, Vector3.one * 0.5f));
                
                GameObject go = Instantiate(entityPrefab, startPos, Quaternion.identity);
                go.name = $"Entity_{entity.Id}";
                
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mat = new Material(Shader.Find("Standard"));
                    
                    if (i == 0)
                    {
                        mat.color = Color.red;
                        _controlledEntity = entity;
                    }
                    else
                    {
                        mat.color = new Color(
                            Random.value * 0.5f + 0.5f,
                            Random.value * 0.5f + 0.5f,
                            Random.value * 0.5f + 0.5f
                        );
                    }
                    
                    renderer.material = mat;
                }
                
                _actorManager.RegisterEntity(go, entity);
            }
        }
        
        void StartEcsThread()
        {
            _ecsThread = new Thread(EcsUpdateLoop)
            {
                IsBackground = true,
                Name = "ECS-Replay-Thread"
            };
            _ecsThread.Start();
        }
        
        void EcsUpdateLoop()
        {
            while (_isRunning)
            {
                // 只在录制或空闲状态下更新ECS（播放时不更新，由Player控制）
                if (_currentState == DemoState.Recording || _currentState == DemoState.Idle)
                {
                    _ecsWorld.Update(_timeData);
                    
                    // 如果正在录制，记录当前帧
                    if (_currentState == DemoState.Recording && _recorder.IsRecording)
                    {
                        _recorder.RecordFrame();
                    }
                    
                    _timeData.Frame++;
                }
                
                Thread.Sleep((int)_timeData.MilliSeconds);
            }
        }
        
        void Update()
        {
            // 只在非播放状态下允许玩家输入
            if (_currentState != DemoState.Playing)
            {
                HandlePlayerInput();
            }
            
            // 播放状态下更新播放器
            if (_currentState == DemoState.Playing && _player.IsPlaying)
            {
                _player.Update(Time.deltaTime);
            }
        }
        
        void HandlePlayerInput()
        {
            if (_controlledEntity.Id == 0) return;
            
            Vector3 movement = Vector3.zero;
            float speed = 5f * Time.deltaTime;
            
            if (Input.GetKey(KeyCode.W)) movement.y += speed;
            if (Input.GetKey(KeyCode.S)) movement.y -= speed;
            if (Input.GetKey(KeyCode.A)) movement.x -= speed;
            if (Input.GetKey(KeyCode.D)) movement.x += speed;
            
            if (movement != Vector3.zero)
            {
                if (_ecsWorld.ComponentManager.TryGetComponent(_controlledEntity, out EcsTransform transform))
                {
                    transform.Position += movement;
                    _ecsWorld.ComponentManager.SetComponent(_controlledEntity, transform);
                }
            }
        }
        
        void OnGUI()
        {
            // 左侧信息面板
            DrawInfoPanel();
            
            // 右侧控制面板
            DrawControlPanel();
            
            // 底部播放控制条
            if (_currentState == DemoState.Playing || _currentState == DemoState.Paused)
            {
                DrawPlaybackControls();
            }
        }
        
        void DrawInfoPanel()
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 300));
            
            GUILayout.Label("=== 录像功能演示 ===", _titleStyle);
            GUILayout.Space(10);
            
            GUILayout.Label($"当前帧: {_timeData.Frame}", _normalStyle);
            GUILayout.Label($"Unity FPS: {(int)(1f / Time.deltaTime)}", _normalStyle);
            GUILayout.Label($"状态: {GetStateText()}", _normalStyle);
            
            GUILayout.Space(10);
            
            if (_currentState == DemoState.Recording)
            {
                GUILayout.Label("=== 录制信息 ===", _titleStyle);
                GUILayout.Label($"已录制帧数: {_recorder.RecordedFrames}", _normalStyle);
                float duration = _recorder.RecordedFrames / 60f;
                GUILayout.Label($"录制时长: {duration:F2} 秒", _normalStyle);
            }
            else if (_currentState == DemoState.Playing || _currentState == DemoState.Paused)
            {
                GUILayout.Label("=== 播放信息 ===", _titleStyle);
                GUILayout.Label($"当前帧: {_player.CurrentFrameIndex}/{_player.TotalFrames}", _normalStyle);
                GUILayout.Label($"播放时间: {_player.CurrentTime:F2}/{_player.TotalDuration:F2} 秒", _normalStyle);
                GUILayout.Label($"播放速度: {_player.PlaybackSpeed}x", _normalStyle);
                GUILayout.Label($"进度: {(_player.Progress * 100):F1}%", _normalStyle);
            }
            
            GUILayout.Space(10);
            GUILayout.Label("控制:", _titleStyle);
            GUILayout.Label("WASD - 移动红色立方体", _normalStyle);
            
            GUILayout.EndArea();
        }
        
        void DrawControlPanel()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, 600));
            
            GUILayout.Label("=== 录制控制 ===", _titleStyle);
            GUILayout.Space(10);
            
            if (_currentState == DemoState.Idle || _currentState == DemoState.Paused)
            {
                if (GUILayout.Button("开始录制", _buttonStyle))
                {
                    _recorder.StartRecording($"录像_{System.DateTime.Now:HHmmss}", 60);
                    _currentState = DemoState.Recording;
                }
            }
            else if (_currentState == DemoState.Recording)
            {
                if (GUILayout.Button("停止录制", _buttonStyle))
                {
                    var replay = _recorder.StopRecording();
                    _currentState = DemoState.Idle;
                }
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("取消录制", _buttonStyle))
                {
                    _recorder.CancelRecording();
                    _currentState = DemoState.Idle;
                }
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== 文件操作 ===", _titleStyle);
            GUILayout.Space(10);
            
            if (GUILayout.Button("保存录像", _buttonStyle))
            {
                SaveReplayToFile();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("加载录像", _buttonStyle))
            {
                LoadReplayFromFile();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== 播放控制 ===", _titleStyle);
            GUILayout.Space(10);
            
            if (_currentState == DemoState.Idle || _currentState == DemoState.Paused)
            {
                if (GUILayout.Button("播放", _buttonStyle))
                {
                    _player.Play();
                    _currentState = DemoState.Playing;
                }
            }
            else if (_currentState == DemoState.Playing)
            {
                if (GUILayout.Button("暂停", _buttonStyle))
                {
                    _player.Pause();
                    _currentState = DemoState.Paused;
                }
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("停止", _buttonStyle))
            {
                _player.Stop();
                _currentState = DemoState.Idle;
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== 播放速度 ===", _titleStyle);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.25x", _smallButtonStyle))
                _player.SetPlaybackSpeed(0.25f);
            if (GUILayout.Button("0.5x", _smallButtonStyle))
                _player.SetPlaybackSpeed(0.5f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1x", _smallButtonStyle))
                _player.SetPlaybackSpeed(1.0f);
            if (GUILayout.Button("2x", _smallButtonStyle))
                _player.SetPlaybackSpeed(2.0f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("4x", _smallButtonStyle))
                _player.SetPlaybackSpeed(4.0f);
            if (GUILayout.Button("8x", _smallButtonStyle))
                _player.SetPlaybackSpeed(8.0f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== 帧控制 ===", _titleStyle);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("◄◄ 前10帧", _smallButtonStyle))
            {
                _player.SeekToFrameIndex(_player.CurrentFrameIndex - 10);
            }
            if (GUILayout.Button("后10帧 ►►", _smallButtonStyle))
            {
                _player.SeekToFrameIndex(_player.CurrentFrameIndex + 10);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("◄ 上一帧", _smallButtonStyle))
            {
                _player.StepBackward();
            }
            if (GUILayout.Button("下一帧 ►", _smallButtonStyle))
            {
                _player.StepForward();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }
        
        void DrawPlaybackControls()
        {
            float panelHeight = 80;
            float panelWidth = Screen.width - 40;
            float panelX = 20;
            float panelY = Screen.height - panelHeight - 20;
            
            GUILayout.BeginArea(new Rect(panelX, panelY, panelWidth, panelHeight));
            
            GUILayout.Label("播放进度:", _normalStyle);
            
            // 进度条
            float progress = GUI.HorizontalSlider(
                new Rect(0, 25, panelWidth, 20),
                _player.Progress,
                0f,
                1f
            );
            
            // 如果用户拖动进度条
            if (Mathf.Abs(progress - _player.Progress) > 0.001f)
            {
                _player.SeekToProgress(progress);
            }
            
            // 时间显示
            GUILayout.Space(25);
            string timeText = $"{_player.CurrentTime:F2}s / {_player.TotalDuration:F2}s";
            GUILayout.Label(timeText, _normalStyle);
            
            GUILayout.EndArea();
        }
        
        void SaveReplayToFile()
        {
            if (_recorder.CurrentReplay == null)
            {
                Debug.LogWarning("没有可保存的录像！请先录制。");
                return;
            }
            
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string filePath = dir + replayFileName;
            _recorder.SaveReplay(filePath);
        }
        
        void LoadReplayFromFile()
        {
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string filePath = dir + replayFileName;
            
            if (System.IO.File.Exists(filePath))
            {
                _player.LoadReplayFromFile(filePath);
                _currentState = DemoState.Idle;
            }
            else
            {
                Debug.LogWarning($"录像文件不存在: {filePath}");
            }
        }
        
        string GetStateText()
        {
            switch (_currentState)
            {
                case DemoState.Recording:
                    return "<color=red>录制中...</color>";
                case DemoState.Playing:
                    return "<color=green>播放中...</color>";
                case DemoState.Paused:
                    return "<color=yellow>已暂停</color>";
                default:
                    return "<color=white>空闲</color>";
            }
        }
        
        void OnDestroy()
        {
            _isRunning = false;
            
            if (_ecsThread != null && _ecsThread.IsAlive)
            {
                _ecsThread.Join(1000);
            }
            
            _ecsWorld?.Dispose();
            LogManager.Instance.Shutdown();
            
            Debug.Log("录像演示已停止");
        }
    }
}

