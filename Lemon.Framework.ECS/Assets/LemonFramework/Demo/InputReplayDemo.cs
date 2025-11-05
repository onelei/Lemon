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
    /// 基于输入录制的录像演示
    /// 只记录玩家输入，文件更小，适合网络游戏回放
    /// </summary>
    public class InputReplayDemo : MonoBehaviour
    {
        [Header("ECS设置")]
        [Tooltip("创建的实体数量")] public int entityCount = 5;
        [Tooltip("实体预制体")] public GameObject entityPrefab;
        
        [Header("录像设置")]
        [Tooltip("录像文件保存路径")] public string replayFileName = "input_replay.dat";
        
        private World _ecsWorld;
        private Thread _ecsThread;
        private TimeData _timeData;
        private bool _isRunning = true;
        
        private EntityActorManager _actorManager;
        private Entity _controlledEntity;
        
        // 基于输入的录像系统
        private InputRecorder _inputRecorder;
        private InputPlayer _inputPlayer;
        
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
        
        // 当前帧的输入
        private Vector2 _currentInput;
        
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
            _inputRecorder = new InputRecorder(_ecsWorld);
            _inputPlayer = new InputPlayer(_ecsWorld);
            
            // 启动ECS线程
            StartEcsThread();
            
            Debug.Log("=== 输入录制演示已启动 ===");
            Debug.Log("只记录输入，文件更小！");
            Debug.Log("使用WASD控制红色立方体");
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
            _ecsWorld = new World("InputReplayWorld");
            _timeData = new TimeData(60);
            
            _actorManager.SetWorld(_ecsWorld);
            
            // 添加移动系统 - 使用支持输入的系统
            _ecsWorld.AddSystem(new PlayerInputMovementSystem(this));
            //_ecsWorld.AddSystem(new CircleMovementSystem());
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
                Name = "ECS-InputReplay-Thread"
            };
            _ecsThread.Start();
        }
        
        void EcsUpdateLoop()
        {
            while (_isRunning)
            {
                // 录制或空闲状态：正常更新
                if (_currentState == DemoState.Recording || _currentState == DemoState.Idle)
                {
                    _ecsWorld.Update(_timeData);
                    
                    // 录制输入
                    if (_currentState == DemoState.Recording && _inputRecorder.IsRecording)
                    {
                        _inputRecorder.RecordInput(_currentInput);
                    }
                    
                    _timeData.Frame++;
                }
                // 播放状态：由InputPlayer控制更新
                
                Thread.Sleep((int)_timeData.MilliSeconds);
            }
        }
        
        void Update()
        {
            // 非播放状态：读取玩家输入
            if (_currentState != DemoState.Playing)
            {
                _currentInput = GetPlayerInput();
            }
            else
            {
                // 播放状态：从录像读取输入
                if (_inputPlayer.IsPlaying)
                {
                    _inputPlayer.Update(Time.deltaTime);
                    _currentInput = _inputPlayer.CurrentInput.Movement;
                }
            }
        }
        
        Vector2 GetPlayerInput()
        {
            Vector2 input = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W)) input.y += 1f;
            if (Input.GetKey(KeyCode.S)) input.y -= 1f;
            if (Input.GetKey(KeyCode.A)) input.x -= 1f;
            if (Input.GetKey(KeyCode.D)) input.x += 1f;
            
            return input;
        }
        
        /// <summary>
        /// 获取当前输入（供System调用）
        /// </summary>
        public Vector2 GetCurrentInput()
        {
            return _currentInput;
        }
        
        /// <summary>
        /// 获取受控实体
        /// </summary>
        public Entity GetControlledEntity()
        {
            return _controlledEntity;
        }
        
        void OnGUI()
        {
            DrawInfoPanel();
            DrawControlPanel();
            
            if (_currentState == DemoState.Playing || _currentState == DemoState.Paused)
            {
                DrawPlaybackControls();
            }
        }
        
        void DrawInfoPanel()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 350));
            
            GUILayout.Label("=== 输入录制演示 ===", _titleStyle);
            GUILayout.Space(10);
            
             GUILayout.Label($"当前帧: {_timeData.Frame}", _normalStyle);
             GUILayout.Label($"Unity FPS: {(int)(1f / Time.deltaTime)}", _normalStyle);
             GUILayout.Label($"状态: {GetStateText()}", _normalStyle);
             GUILayout.Label($"当前输入: ({_currentInput.x:F2}, {_currentInput.y:F2})", _normalStyle);
             GUILayout.Label($"关键帧缓存: {_inputPlayer.KeyframeCount}", _normalStyle);
            
            GUILayout.Space(10);
            
            if (_currentState == DemoState.Recording)
            {
                GUILayout.Label("=== 录制信息 ===", _titleStyle);
                GUILayout.Label($"总帧数: {_inputRecorder.RecordedFrames}", _normalStyle);
                GUILayout.Label($"输入帧数: {_inputRecorder.InputFrameCount}", _normalStyle);
                
                float compression = _inputRecorder.RecordedFrames > 0 
                    ? (1f - (float)_inputRecorder.InputFrameCount / _inputRecorder.RecordedFrames) * 100 
                    : 0;
                GUILayout.Label($"压缩率: {compression:F1}%", _normalStyle);
                
                float duration = _inputRecorder.RecordedFrames / 60f;
                GUILayout.Label($"录制时长: {duration:F2} 秒", _normalStyle);
            }
            else if (_currentState == DemoState.Playing || _currentState == DemoState.Paused)
            {
                GUILayout.Label("=== 播放信息 ===", _titleStyle);
                GUILayout.Label($"当前帧: {_inputPlayer.CurrentFrame}", _normalStyle);
                GUILayout.Label($"播放时间: {_inputPlayer.CurrentTime:F2}/{_inputPlayer.TotalDuration:F2} 秒", _normalStyle);
                GUILayout.Label($"播放速度: {_inputPlayer.PlaybackSpeed}x", _normalStyle);
                GUILayout.Label($"进度: {(_inputPlayer.Progress * 100):F1}%", _normalStyle);
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== 优势 ===", _titleStyle);
            GUILayout.Label("• 文件体积小（只记录输入）", _normalStyle);
            GUILayout.Label("• 适合网络传输", _normalStyle);
            GUILayout.Label("• 确定性回放", _normalStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("控制: WASD - 移动红色立方体", _normalStyle);
            
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
                    _inputRecorder.StartRecording($"输入录像_{System.DateTime.Now:HHmmss}", 60);
                    _currentState = DemoState.Recording;
                }
            }
            else if (_currentState == DemoState.Recording)
            {
                if (GUILayout.Button("停止录制", _buttonStyle))
                {
                    var replay = _inputRecorder.StopRecording();
                    _currentState = DemoState.Idle;
                }
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("取消录制", _buttonStyle))
                {
                    _inputRecorder.CancelRecording();
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
                    _inputPlayer.Play();
                    _currentState = DemoState.Playing;
                }
            }
            else if (_currentState == DemoState.Playing)
            {
                if (GUILayout.Button("暂停", _buttonStyle))
                {
                    _inputPlayer.Pause();
                    _currentState = DemoState.Paused;
                }
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("停止", _buttonStyle))
            {
                _inputPlayer.Stop();
                _currentState = DemoState.Idle;
            }
            
            GUILayout.Space(10);
            GUILayout.Label("=== 播放速度 ===", _titleStyle);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.25x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(0.25f);
            if (GUILayout.Button("0.5x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(0.5f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(1.0f);
            if (GUILayout.Button("2x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(2.0f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("4x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(4.0f);
            if (GUILayout.Button("8x", _smallButtonStyle))
                _inputPlayer.SetPlaybackSpeed(8.0f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.Label("=== 帧控制 ===", _titleStyle);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("◄ 上一帧", _smallButtonStyle))
            {
                _inputPlayer.StepBackward();
            }
            if (GUILayout.Button("下一帧 ►", _smallButtonStyle))
            {
                _inputPlayer.StepForward();
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
            
            float progress = GUI.HorizontalSlider(
                new Rect(0, 25, panelWidth, 20),
                _inputPlayer.Progress,
                0f,
                1f
            );
            
            if (Mathf.Abs(progress - _inputPlayer.Progress) > 0.001f)
            {
                _inputPlayer.SeekToProgress(progress);
            }
            
            GUILayout.Space(25);
            string timeText = $"{_inputPlayer.CurrentTime:F2}s / {_inputPlayer.TotalDuration:F2}s";
            GUILayout.Label(timeText, _normalStyle);
            
            GUILayout.EndArea();
        }
        
        void SaveReplayToFile()
        {
            if (_inputRecorder.CurrentReplay == null)
            {
                Debug.LogWarning("没有可保存的录像！请先录制。");
                return;
            }
            
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string filePath = dir + replayFileName;
            _inputRecorder.SaveReplay(filePath);
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
                _inputPlayer.LoadReplayFromFile(filePath);
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
            
            Debug.Log("输入录制演示已停止");
        }
    }
    
    /// <summary>
    /// 玩家输入控制系统
    /// 从Demo中读取输入并应用到实体
    /// </summary>
    public class PlayerInputMovementSystem : ComponentSystem
    {
        private InputReplayDemo _demo;
        
        public PlayerInputMovementSystem(InputReplayDemo demo)
        {
            _demo = demo;
        }
        
        protected override void OnUpdate(TimeData timeData)
        {
            var controlledEntity = _demo.GetControlledEntity();
            if (controlledEntity.Id == 0) return;
            
            var input = _demo.GetCurrentInput();
            if (input.sqrMagnitude < 0.001f) return;
            
            if (ComponentManager.TryGetComponent(controlledEntity, out EcsTransform transform))
            {
                float speed = 0.083f; // 5f / 60f (5 units per second at 60fps)
                Vector3 movement = new Vector3(input.x, input.y, 0) * speed;
                
                transform.Position += movement;
                ComponentManager.SetComponent(controlledEntity, transform);
            }
        }
    }
}

