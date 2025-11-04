# Lemon Framework ECS 架构文档

## 概述

这是一个为 C# 服务器和 Unity 设计的 ECS (Entity-Component-System) 框架，具有以下核心特性：

- ✅ **Scene 管理**：类似 Unity 的场景概念，管理实体和系统
- ✅ **序列化支持**：Entity 和 Component 完全支持序列化，方便创建快照和网络同步
- ✅ **多线程安全**：所有核心组件都是线程安全的，支持多线程并行执行
- ✅ **Unity GameObject 同步**：提供双向同步机制，连接 ECS 与 Unity GameObject
- ✅ **快照系统**：支持保存和恢复场景状态，适用于回放、存档、网络同步等场景

## 核心架构

### 1. Scene（场景）

`Scene` 是 ECS 世界的管理者，类似 Unity 的 GameObject 和 Component 体系：

```csharp
// 创建场景
Scene scene = new Scene("GameScene");

// 创建实体
Entity player = scene.CreateEntity();

// 添加组件
scene.AddComponent(player, new Position(new Vector3(0, 0, 0)));
scene.AddComponent(player, new Velocity(new Vector3(1, 1, 1)));

// 添加系统
scene.AddSystem(new MovementSystem());

// 更新场景（单线程）
scene.Update(timeData);

// 或使用多线程并行更新
scene.UpdateParallel(timeData);
```

**主要特性**：
- 线程安全的实体和系统管理
- 支持序列化/反序列化整个场景
- 内置快照管理器
- 支持单线程或多线程并行更新

### 2. Entity（实体）

实体是一个轻量级的 ID 标识符：

```csharp
public struct Entity : IEquatable<Entity>
{
    public readonly int Id;
}
```

**特点**：
- 简单的整数 ID
- 值类型，无内存分配
- 可序列化

### 3. Component（组件）

组件是纯数据结构，实现 `IComponentData` 和 `ISerializable` 接口：

```csharp
public struct Position : IComponentData, ISerializable
{
    public Vector3 Value;
    
    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Value.x);
        writer.Write(Value.y);
        writer.Write(Value.z);
    }
    
    public void Deserialize(BinaryReader reader)
    {
        Value = new Vector3(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }
}
```

**要求**：
- 必须是 `struct`（值类型）
- 实现 `IComponentData` 接口
- 实现 `ISerializable` 接口以支持序列化
- 在使用前需要注册：`ComponentManager.RegisterComponentType<Position>()`

### 4. System（系统）

系统包含游戏逻辑，继承自 `ComponentSystem`：

```csharp
public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate(TimeData timeData)
    {
        Entities.ForEach((ref Position pos, ref Velocity vel) =>
        {
            pos.Value += vel.Value * timeData.DeltaTime;
        });
    }
}
```

**特点**：
- 在 `OnUpdate` 中实现逻辑
- 通过 `Entities.ForEach` 查询和处理组件
- 自动处理组件的读写

## 序列化与快照

### 快照系统

快照系统允许保存和恢复整个场景状态：

```csharp
// 创建快照
SceneSnapshot snapshot = scene.CreateSnapshot();

// 恢复快照
scene.RestoreFromSnapshot(snapshot);

// 保存快照到文件
scene.SaveSnapshotToFile(snapshot.SnapshotId, "snapshot.dat");

// 从文件加载快照
SceneSnapshot loaded = scene.LoadSnapshotFromFile("snapshot.dat");
scene.RestoreFromSnapshot(loaded);
```

**应用场景**：
- 游戏存档/读档
- 回放系统
- 网络同步（发送快照到客户端）
- 调试和测试

### 序列化接口

所有需要序列化的组件都必须实现 `ISerializable`：

```csharp
public interface ISerializable
{
    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader);
}
```

## 多线程支持

### 线程安全设计

框架的所有核心组件都是线程安全的：

- `ComponentManager`：使用 `ReaderWriterLockSlim` 保护读写操作
- `EntityManager`：使用 `ReaderWriterLockSlim` 保护实体创建/销毁
- `Scene`：使用锁保护系统列表和更新循环

### 多线程模式

```csharp
// 单线程模式（顺序执行所有系统）
scene.Update(timeData);

// 多线程模式（并行执行所有系统）
scene.UpdateParallel(timeData);
```

### 在独立线程中运行

```csharp
Thread ecsThread = new Thread(() =>
{
    while (isRunning)
    {
        scene.Update(timeData);
        Thread.Sleep(16); // ~60 FPS
        timeData.Frame++;
    }
});
ecsThread.IsBackground = true;
ecsThread.Start();
```

## Unity GameObject 同步

### 同步组件

框架提供了多种 Unity 同步组件：

1. **GameObjectReference**：关联 Unity GameObject
```csharp
scene.AddComponent(entity, new GameObjectReference(gameObject));
```

2. **TransformSync**：同步 Transform 数据
```csharp
scene.AddComponent(entity, new TransformSync(transform));
```

3. **RigidbodySync**：同步刚体数据
```csharp
scene.AddComponent(entity, new RigidbodySync(rigidbody));
```

4. **RendererSync**：同步渲染器状态
```csharp
scene.AddComponent(entity, new RendererSync(renderer));
```

### 同步系统

**Unity → ECS 同步**（读取 Unity 状态）：
```csharp
scene.AddSystem(new UnityTransformSyncSystem());
scene.AddSystem(new UnityRigidbodySyncSystem());
```

**ECS → Unity 同步**（写入 Unity 状态）：
```csharp
scene.AddSystem(new ECSToUnityTransformSyncSystem());
scene.AddSystem(new ECSToUnityRigidbodySyncSystem());
```

### 双向同步示例

```csharp
// 创建实体并关联 GameObject
Entity cubeEntity = scene.CreateEntity();
scene.AddComponent(cubeEntity, new GameObjectReference(cubeObject));
scene.AddComponent(cubeEntity, new TransformSync(cubeObject.transform));

// 添加同步系统
scene.AddSystem(new UnityTransformSyncSystem()); // Unity → ECS

// 在 ECS 中修改位置
var transformSync = scene.ComponentManager.GetComponent<TransformSync>(cubeEntity);
transformSync.Position = new Vector3(10, 0, 0);
transformSync.IsDirty = true; // 标记需要同步
scene.ComponentManager.SetComponent(cubeEntity, transformSync);

// 添加 ECS → Unity 同步系统
scene.AddSystem(new ECSToUnityTransformSyncSystem()); // ECS → Unity
```

## 使用示例

### 1. 基础 ECS 示例

```csharp
// 注册组件类型
ComponentManager.RegisterComponentType<Position>();
ComponentManager.RegisterComponentType<Velocity>();

// 创建场景
Scene scene = new Scene("GameScene");

// 创建实体
Entity entity = scene.CreateEntity();
scene.AddComponent(entity, new Position(new Vector3(0, 0, 0)));
scene.AddComponent(entity, new Velocity(new Vector3(1, 0, 0)));

// 添加系统
scene.AddSystem(new MovementSystem());

// 游戏循环
TimeData timeData = new TimeData(60);
while (true)
{
    scene.Update(timeData);
    timeData.Frame++;
    Thread.Sleep(16);
}
```

### 2. 快照示例

```csharp
// 运行一段时间
for (int i = 0; i < 100; i++)
{
    scene.Update(timeData);
    timeData.Frame++;
}

// 创建快照
SceneSnapshot snapshot = scene.CreateSnapshot();

// 继续运行
for (int i = 0; i < 100; i++)
{
    scene.Update(timeData);
    timeData.Frame++;
}

// 回到快照状态
scene.RestoreFromSnapshot(snapshot);
```

### 3. Unity 同步示例

参见 `UnitySyncDemo.cs`，展示了完整的 Unity GameObject 同步流程。

## 性能优化

### 1. 组件查询缓存

`ComponentManager` 会缓存查询结果，避免重复创建列表：

```csharp
// 第一次查询会创建列表
var entities = ComponentManager.GetEntitiesWithComponents(typeof(Position), typeof(Velocity));

// 后续查询会复用同一个列表
var entities2 = ComponentManager.GetEntitiesWithComponents(typeof(Position), typeof(Velocity));
```

### 2. 避免装箱

所有组件都是值类型（struct），避免了 GC 压力。

### 3. 读写锁

使用 `ReaderWriterLockSlim` 允许多个读者并发访问，提高并发性能。

### 4. 多线程并行

使用 `UpdateParallel` 可以让多个系统并行执行（如果它们不共享数据）。

## 项目结构

```
LemonFramework/ECS/
├── ECS/
│   ├── Scene.cs                    // 场景管理
│   ├── Entity.cs                   // 实体定义
│   ├── ComponentManager.cs         // 组件管理器（线程安全）
│   ├── EntityManager.cs            // 实体管理器（线程安全）
│   ├── ComponentSystem.cs          // 系统基类
│   ├── IComponentData.cs           // 组件接口
│   ├── ISerializable.cs            // 序列化接口
│   └── TimeData.cs                 // 时间数据
├── Snapshot/
│   └── Snapshot.cs                 // 快照系统
├── Unity/
│   ├── UnityComponents.cs          // Unity 同步组件
│   └── UnitySyncSystem.cs          // Unity 同步系统
└── Demo/
    ├── SceneDemo.cs                // 场景和快照演示
    └── UnitySyncDemo.cs            // Unity 同步演示
```

## 注意事项

### 1. 组件注册

在使用组件之前，必须先注册：

```csharp
ComponentManager.RegisterComponentType<YourComponent>();
```

### 2. 线程安全

虽然框架是线程安全的，但在多线程环境下仍需注意：
- 避免在不同线程同时修改同一个组件
- 使用 `Update()` 而非 `UpdateParallel()` 如果系统之间有依赖关系

### 3. Unity GameObject 引用

`GameObjectReference.GameObject` 字段不会被序列化，反序列化后需要手动重建关联。

### 4. 序列化格式

当前使用二进制序列化，如果需要 JSON 或其他格式，可以修改 `ISerializable` 实现。

## 扩展建议

1. **网络同步**：使用快照系统实现客户端-服务器同步
2. **回放系统**：记录每帧的快照，实现完整回放
3. **热更新**：序列化场景状态，更新代码后恢复
4. **存档系统**：保存快照到文件实现存档/读档
5. **调试工具**：创建编辑器工具可视化 ECS 状态

## 许可证

根据项目需要添加许可证信息。

