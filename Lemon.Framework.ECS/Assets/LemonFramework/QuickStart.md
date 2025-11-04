# Lemon ECS 快速开始指南

## 1. 基础服务器端 ECS 使用

### 第一步：定义组件

```csharp
using System.IO;
using LemonFramework.ECS;

namespace YourGame.Components
{
    // 位置组件
    public struct Position : IComponentData, ISerializable
    {
        public float X, Y, Z;
        
        public Position(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public void Deserialize(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }
    
    // 速度组件
    public struct Velocity : IComponentData, ISerializable
    {
        public float X, Y, Z;
        
        public Velocity(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public void Deserialize(BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }
    }
}
```

### 第二步：创建系统

```csharp
using LemonFramework.ECS;
using YourGame.Components;

namespace YourGame.Systems
{
    public class MovementSystem : ComponentSystem
    {
        protected override void OnUpdate(TimeData timeData)
        {
            // 查询所有同时拥有 Position 和 Velocity 的实体
            Entities.ForEach((ref Position pos, ref Velocity vel) =>
            {
                // 更新位置
                pos.X += vel.X * timeData.DeltaTime;
                pos.Y += vel.Y * timeData.DeltaTime;
                pos.Z += vel.Z * timeData.DeltaTime;
            });
        }
    }
}
```

### 第三步：初始化和运行

```csharp
using System;
using System.Threading;
using LemonFramework.ECS;
using YourGame.Components;
using YourGame.Systems;

class Program
{
    static void Main(string[] args)
    {
        // 1. 注册组件类型（序列化必需）
        ComponentManager.RegisterComponentType<Position>();
        ComponentManager.RegisterComponentType<Velocity>();
        
        // 2. 创建场景
        Scene gameScene = new Scene("MainGameScene");
        
        // 3. 创建实体和组件
        for (int i = 0; i < 100; i++)
        {
            Entity entity = gameScene.CreateEntity();
            gameScene.AddComponent(entity, new Position(i, 0, 0));
            gameScene.AddComponent(entity, new Velocity(1, 0, 0));
        }
        
        // 4. 添加系统
        gameScene.AddSystem(new MovementSystem());
        
        // 5. 游戏循环（60 FPS）
        TimeData timeData = new TimeData(60);
        
        while (true)
        {
            // 更新场景
            gameScene.Update(timeData);
            
            // 每秒输出一次
            if (timeData.Frame % 60 == 0)
            {
                Console.WriteLine($"帧: {timeData.Frame}");
            }
            
            // 休眠以保持帧率
            Thread.Sleep((int)timeData.MilliSeconds);
            timeData.Frame++;
        }
    }
}
```

## 2. 使用快照系统

### 创建和恢复快照

```csharp
// 运行游戏一段时间
for (int i = 0; i < 100; i++)
{
    gameScene.Update(timeData);
    timeData.Frame++;
}

// 创建快照
SceneSnapshot checkpoint = gameScene.CreateSnapshot();
Console.WriteLine($"快照创建: 帧 {checkpoint.FrameNumber}");

// 继续运行
for (int i = 0; i < 100; i++)
{
    gameScene.Update(timeData);
    timeData.Frame++;
}

// 恢复到快照状态
gameScene.RestoreFromSnapshot(checkpoint);
timeData.Frame = checkpoint.FrameNumber;
Console.WriteLine($"场景已恢复到帧 {timeData.Frame}");
```

### 保存和加载快照文件

```csharp
// 保存快照到文件
string snapshotPath = "game_snapshot.dat";
gameScene.SaveSnapshotToFile(checkpoint.SnapshotId, snapshotPath);
Console.WriteLine($"快照已保存: {snapshotPath}");

// 从文件加载快照
SceneSnapshot loadedSnapshot = gameScene.LoadSnapshotFromFile(snapshotPath);
gameScene.RestoreFromSnapshot(loadedSnapshot);
Console.WriteLine("快照已加载并恢复");
```

## 3. 多线程模式

### 在独立线程中运行 ECS

```csharp
bool isRunning = true;

Thread ecsThread = new Thread(() =>
{
    TimeData timeData = new TimeData(60);
    
    while (isRunning)
    {
        gameScene.Update(timeData);
        Thread.Sleep((int)timeData.MilliSeconds);
        timeData.Frame++;
    }
});

ecsThread.IsBackground = true;
ecsThread.Name = "ECS-Thread";
ecsThread.Start();

// 主线程可以做其他事情，比如处理网络
Console.WriteLine("按任意键停止...");
Console.ReadKey();

isRunning = false;
ecsThread.Join();
```

### 并行更新系统

```csharp
// 使用并行模式更新所有系统（适合系统之间无依赖的情况）
gameScene.UpdateParallel(timeData);
```

## 4. Unity GameObject 同步

### Unity MonoBehaviour 示例

```csharp
using UnityEngine;
using LemonFramework.ECS;
using LemonFramework.ECS.Unity;

public class GameManager : MonoBehaviour
{
    private Scene _scene;
    private Entity _playerEntity;
    private TimeData _timeData;
    
    public GameObject playerGameObject;
    
    void Start()
    {
        // 注册组件类型
        ComponentManager.RegisterComponentType<GameObjectReference>();
        ComponentManager.RegisterComponentType<TransformSync>();
        
        // 创建场景
        _scene = new Scene("UnityScene");
        _timeData = new TimeData(60);
        
        // 创建实体并关联 GameObject
        _playerEntity = _scene.CreateEntity();
        _scene.AddComponent(_playerEntity, new GameObjectReference(playerGameObject));
        _scene.AddComponent(_playerEntity, new TransformSync(playerGameObject.transform));
        
        // 添加同步系统
        _scene.AddSystem(new UnityTransformSyncSystem()); // Unity → ECS
        _scene.AddSystem(new ECSToUnityTransformSyncSystem()); // ECS → Unity
    }
    
    void Update()
    {
        // 在主线程更新场景
        _scene.Update(_timeData);
        _timeData.Frame++;
    }
    
    void OnDestroy()
    {
        _scene?.Dispose();
    }
}
```

### 在 ECS 中控制 Unity GameObject

```csharp
// 修改 ECS 中的 Transform
if (_scene.ComponentManager.TryGetComponent(_playerEntity, out TransformSync transformSync))
{
    // 修改位置
    transformSync.Position = new Vector3(10, 0, 0);
    transformSync.IsDirty = true; // 标记为需要同步
    
    _scene.ComponentManager.SetComponent(_playerEntity, transformSync);
}

// 下一帧更新时，ECSToUnityTransformSyncSystem 会将改变同步到 Unity GameObject
```

## 5. 实用技巧

### 查询实体

```csharp
// 手动查询特定组件的实体
var entities = _scene.ComponentManager.GetEntitiesWithComponents(
    typeof(Position), 
    typeof(Velocity)
);

foreach (var entity in entities)
{
    var pos = _scene.ComponentManager.GetComponent<Position>(entity);
    Console.WriteLine($"实体 {entity.Id} 位置: ({pos.X}, {pos.Y}, {pos.Z})");
}
```

### 定期创建快照（自动存档）

```csharp
int framesBetweenSnapshots = 600; // 每 600 帧（10秒 @ 60fps）

if (timeData.Frame % framesBetweenSnapshots == 0)
{
    var snapshot = gameScene.CreateSnapshot();
    string filename = $"autosave_{timeData.Frame}.dat";
    gameScene.SaveSnapshotToFile(snapshot.SnapshotId, filename);
    Console.WriteLine($"自动存档: {filename}");
}
```

### 网络同步示例

```csharp
// 服务器端：创建快照并发送给客户端
SceneSnapshot snapshot = serverScene.CreateSnapshot();
byte[] snapshotData = serverScene.Serialize(); // 序列化整个场景
SendToClients(snapshotData); // 发送给所有客户端

// 客户端：接收并应用快照
byte[] receivedData = ReceiveFromServer();
clientScene.Deserialize(receivedData); // 反序列化并恢复场景
```

## 6. 性能建议

1. **组件大小**：保持组件小而专注，避免大型数据结构
2. **查询缓存**：框架会自动缓存查询结果，重复查询性能很好
3. **多线程**：如果系统之间无依赖，使用 `UpdateParallel()` 可以提升性能
4. **快照频率**：快照操作需要序列化所有数据，不要过于频繁
5. **组件注册**：在程序启动时一次性注册所有组件类型

## 7. 常见问题

**Q: 为什么必须注册组件类型？**

A: 组件注册用于序列化，框架需要将类型映射到 ID 才能正确序列化和反序列化。

**Q: 可以在运行时动态添加/删除系统吗？**

A: 可以，使用 `scene.AddSystem()` 和 `scene.RemoveSystem()` 都是线程安全的。

**Q: Unity GameObject 引用在序列化后会丢失吗？**

A: 是的，`GameObjectReference.GameObject` 不会被序列化。反序列化后需要手动重建关联。

**Q: 如何在多线程环境下安全地修改组件？**

A: 框架的 `ComponentManager` 是线程安全的，但避免在不同线程同时修改同一个组件。

## 8. 更多示例

查看以下文件获取完整示例：

- `SceneDemo.cs` - 场景管理和快照演示
- `UnitySyncDemo.cs` - Unity GameObject 同步演示
- `ECSDemo.cs` - 基础 ECS 演示

## 下一步

阅读 `ECS_README.md` 了解架构详细说明和高级特性。

