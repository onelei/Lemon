using System.Collections.Generic;
using System.Threading;
using LemonFrameSync.Logger;
using LemonFramework.ECS;
using LemonFramework.ECS.Actors;
using LemonFramework.ECS.Components;
using LemonFramework.ECS.Entitys;
using LemonFramework.ECS.Snapshot;
using LemonFramework.ECS.Systems;
using UnityEngine;

namespace LemonFramework.Demo
{
    /// <summary>
    /// 预测回滚演示
    /// 模拟网络游戏中的快照-回滚-重放机制
    /// </summary>
    public class PredictionRollbackDemo : MonoBehaviour
    {
        [Header("ECS设置")] [Tooltip("创建的实体数量")] public int entityCount = 1;

        [Tooltip("实体预制体")] public GameObject entityPrefab;

        [Header("快照设置")] [Tooltip("每隔多少帧创建一次快照")]
        public int snapshotInterval = 30; // 0.5秒 @ 60fps

        [Tooltip("最多保留多少个快照")] public int maxSnapshots = 10;

        [Header("回滚测试")] [Tooltip("回滚多少帧")] public int rollbackFrames = 60; // 1秒

        private World _ecsWorld;
        private Thread _ecsThread;
        private TimeData _timeData;
        private bool _isRunning = true;

        private EntityActorManager _actorManager;
        private Entity _controlledEntity; // 可控制的实体

        // 快照相关
        private readonly Queue<WorldSnapshot> _snapshotQueue = new Queue<WorldSnapshot>();
        private readonly object _snapshotLock = new object(); // 快照队列锁
        private int _lastSnapshotFrame = 0;

        // UI显示
        private GUIStyle _titleStyle;
        private GUIStyle _normalStyle;
        private GUIStyle _buttonStyle;
        private bool _isRollingBack = false;
        private int _currentSnapshotCount = 0;

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

            // 启动ECS线程
            StartEcsThread();

            Debug.Log("=== 预测回滚演示已启动 ===");
            Debug.Log($"每 {snapshotInterval} 帧创建一次快照");
            Debug.Log($"使用WASD控制红色立方体");
        }

        void InitializeStyles()
        {
            _titleStyle = new GUIStyle
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState {textColor = Color.yellow}
            };

            _normalStyle = new GUIStyle
            {
                fontSize = 14,
                normal = new GUIStyleState {textColor = Color.white}
            };

            _buttonStyle = new GUIStyle("button")
            {
                fontSize = 14,
                fixedHeight = 40
            };
        }

        void InitializeEcs()
        {
            _ecsWorld = new World("RollbackWorld");
            _timeData = new TimeData(60);

            _actorManager.SetWorld(_ecsWorld);

            // 添加移动系统
            _ecsWorld.AddSystem(new AutoMovementSystem());
        }

        void CreateEntities()
        {
            if (entityPrefab == null)
            {
                entityPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                entityPrefab.transform.localScale = Vector3.one * 0.5f;
            }

            // 创建网格排列的实体
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

                // 设置颜色
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mat = new Material(Shader.Find("Standard"));

                    // 第一个实体是玩家控制的，用红色标记
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
                Name = "ECS-Rollback-Thread"
            };
            _ecsThread.Start();
        }

        void EcsUpdateLoop()
        {
            while (_isRunning)
            {
                // 更新ECS
                _ecsWorld.Update(_timeData);

                // 定期创建快照
                if (_timeData.Frame - _lastSnapshotFrame >= snapshotInterval)
                {
                    CreateSnapshot();
                    _lastSnapshotFrame = _timeData.Frame;
                }

                Thread.Sleep((int) _timeData.MilliSeconds);
                _timeData.Frame++;
            }
        }

        void Update()
        {
            // 玩家输入控制
            HandlePlayerInput();
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
                // 修改玩家实体的位置
                if (_ecsWorld.ComponentManager.TryGetComponent(_controlledEntity, out EcsTransform transform))
                {
                    transform.Position += movement;
                    _ecsWorld.ComponentManager.SetComponent(_controlledEntity, transform);
                }
            }
        }

        void CreateSnapshot()
        {
            var snapshot = _ecsWorld.CreateSnapshot();

            lock (_snapshotLock)
            {
                _snapshotQueue.Enqueue(snapshot);
                _currentSnapshotCount++;

                // 限制快照数量
                while (_snapshotQueue.Count > maxSnapshots)
                {
                    var oldSnapshot = _snapshotQueue.Dequeue();
                    _ecsWorld.SnapshotManager.RemoveSnapshot(oldSnapshot.SnapshotId);
                    _currentSnapshotCount--;
                }
            }

            Debug.Log($"[快照] 创建快照 #{snapshot.SnapshotId}, 帧:{snapshot.FrameNumber}, 队列:{_snapshotQueue.Count}");
        }

        /// <summary>
        /// 回滚到指定帧数之前
        /// </summary>
        void RollbackFrames(int frames)
        {
            lock (_snapshotLock)
            {
                if (_snapshotQueue.Count == 0)
                {
                    Debug.LogWarning("没有可用的快照！");
                    return;
                }

                _isRollingBack = true;

                // 找到最接近的快照
                WorldSnapshot targetSnapshot = null;
                int targetFrame = _timeData.Frame - frames;

                foreach (var snapshot in _snapshotQueue)
                {
                    if (snapshot.FrameNumber <= targetFrame)
                    {
                        targetSnapshot = snapshot;
                    }
                }

                if (targetSnapshot == null)
                {
                    targetSnapshot = _snapshotQueue.Peek(); // 使用最早的快照
                }

                // 恢复快照
                _ecsWorld.RestoreFromSnapshot(targetSnapshot);
                _timeData.Frame = targetSnapshot.FrameNumber;

                Debug.Log(
                    $"<color=cyan>[回滚] 回滚到快照 #{targetSnapshot.SnapshotId}, 帧:{targetSnapshot.FrameNumber}</color>");

                _isRollingBack = false;
            }
        }

        /// <summary>
        /// 回滚到最早的快照
        /// </summary>
        void RollbackToOldest()
        {
            lock (_snapshotLock)
            {
                if (_snapshotQueue.Count == 0)
                {
                    Debug.LogWarning("没有可用的快照！");
                    return;
                }

                _isRollingBack = true;

                var oldestSnapshot = _snapshotQueue.Peek();
                _ecsWorld.RestoreFromSnapshot(oldestSnapshot);
                _timeData.Frame = oldestSnapshot.FrameNumber;

                Debug.Log(
                    $"<color=cyan>[回滚] 回滚到最早快照 #{oldestSnapshot.SnapshotId}, 帧:{oldestSnapshot.FrameNumber}</color>");

                _isRollingBack = false;
            }
        }

        /// <summary>
        /// 保存当前状态到文件
        /// </summary>
        void SaveToFile()
        {
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string filePath = dir + "/rollback_snapshot.dat";
            var snapshot = _ecsWorld.CreateSnapshot();
            _ecsWorld.SaveSnapshotToFile(snapshot.SnapshotId, filePath);
            Debug.Log($"<color=green>快照已保存到: {filePath}</color>");
        }

        /// <summary>
        /// 从文件加载状态
        /// </summary>
        void LoadFromFile()
        {
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string filePath = dir + "/rollback_snapshot.dat";
            if (System.IO.File.Exists(filePath))
            {
                var snapshot = _ecsWorld.LoadSnapshotFromFile(filePath);
                _ecsWorld.RestoreFromSnapshot(snapshot);
                _timeData.Frame = snapshot.FrameNumber;
                Debug.Log($"<color=green>快照已从文件加载, 帧:{snapshot.FrameNumber}</color>");
            }
            else
            {
                Debug.LogWarning("快照文件不存在！");
            }
        }

        void OnGUI()
        {
            // 在OnGUI开始时就缓存快照数组，确保在Layout和Repaint事件中使用相同的数组
            WorldSnapshot[] snapshotArray;
            lock (_snapshotLock)
            {
                snapshotArray = _snapshotQueue.ToArray();
            }

            // 左侧信息面板
            GUILayout.BeginArea(new Rect(10, 10, 350, 400));

            GUILayout.Label("=== 预测回滚演示 ===", _titleStyle);
            GUILayout.Space(10);

            GUILayout.Label($"当前帧: {_timeData.Frame}", _normalStyle);
            GUILayout.Label($"Unity FPS: {(int) (1f / Time.deltaTime)}", _normalStyle);
            GUILayout.Label($"快照数量: {_currentSnapshotCount}/{maxSnapshots}", _normalStyle);
            GUILayout.Label($"快照间隔: {snapshotInterval} 帧", _normalStyle);
            GUILayout.Label($"状态: {(_isRollingBack ? "回滚中..." : "运行中")}", _normalStyle);

            if (snapshotArray.Length > 0)
            {
                var oldest = snapshotArray[0];
                GUILayout.Label($"最早快照: 帧 {oldest.FrameNumber}", _normalStyle);
            }

            GUILayout.Space(10);
            GUILayout.Label("控制:", _titleStyle);
            GUILayout.Label("WASD - 移动红色立方体", _normalStyle);

            GUILayout.EndArea();

            // 右侧按钮面板
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, 500));

            GUILayout.Label("=== 回滚操作 ===", _titleStyle);
            GUILayout.Space(10);

            if (GUILayout.Button($"回滚 {rollbackFrames} 帧 (1秒)", _buttonStyle))
            {
                RollbackFrames(rollbackFrames);
            }

            GUILayout.Space(5);

            if (GUILayout.Button($"回滚 {rollbackFrames * 2} 帧 (2秒)", _buttonStyle))
            {
                RollbackFrames(rollbackFrames * 2);
            }

            GUILayout.Space(5);

            if (GUILayout.Button("回滚到最早快照", _buttonStyle))
            {
                RollbackToOldest();
            }

            GUILayout.Space(20);
            GUILayout.Label("=== 文件操作 ===", _titleStyle);
            GUILayout.Space(10);

            if (GUILayout.Button("保存快照到文件", _buttonStyle))
            {
                SaveToFile();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("从文件加载快照", _buttonStyle))
            {
                LoadFromFile();
            }

            GUILayout.Space(20);
            GUILayout.Label("=== 快照队列 ===", _titleStyle);
            GUILayout.Space(5);

            int index = 0;
            foreach (var snapshot in snapshotArray)
            {
                string label = $"{index}: 帧 {snapshot.FrameNumber}";
                if (GUILayout.Button(label, GUILayout.Height(25)))
                {
                    // 点击直接恢复到该快照
                    _ecsWorld.RestoreFromSnapshot(snapshot);
                    _timeData.Frame = snapshot.FrameNumber;
                    Debug.Log($"<color=cyan>直接恢复到快照 #{snapshot.SnapshotId}</color>");
                }

                index++;
            }

            GUILayout.EndArea();
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

            Debug.Log("预测回滚演示已停止");
        }
    }

    /// <summary>
    /// 自动移动系统（用于演示）
    /// </summary>
    public class AutoMovementSystem : ComponentSystem
    {
        protected override void OnUpdate(TimeData timeData)
        {
            var entities = ComponentManager.GetEntitiesWithComponents(typeof(EcsTransform));

            foreach (var entity in entities)
            {
                // 跳过第一个实体（玩家控制的）
                if (entity.Id == 1) continue;

                if (ComponentManager.TryGetComponent(entity, out EcsTransform transform))
                {
                    // 简单的上下浮动动画
                    float t = timeData.Frame * 0.05f + entity.Id;
                    float y = Mathf.Sin(t) * 0.5f;

                    transform.Position = new UnityEngine.Vector3(
                        transform.Position.x,
                        y,
                        transform.Position.z
                    );

                    // 缓慢旋转
                    transform.Rotation = UnityEngine.Quaternion.Euler(0, timeData.Frame * 0.5f, 0);

                    ComponentManager.SetComponent(entity, transform);
                }
            }
        }
    }
}