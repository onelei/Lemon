using Lemon.Framework.ECS.Components;
using Lemon.Framework.ECS.Entitys;
using Lemon.Framework.ECS.Systems;
using LemonFramework;
using UnityEngine;

namespace Lemon.Framework.ECS.Sample
{
    public class ECSSample : MonoBehaviour
    {
        private void Start()
        {
            var world = new World();
            var player = new Entity(1);
            var enemy = new Entity(2);
            var healthComponent = new HealthComponent { Health = 100, MaxHealth = 100 };
            var attackComponent = new AttackComponent { Damage = 10 };
            world.AddComponent(player, attackComponent);
            world.AddComponent(enemy, healthComponent);
            var combatSystem = new CombatSystem(world);
            combatSystem.PerformAttack(player, enemy);
            
            RandomUtility.UnitTest();
        }
    }
}