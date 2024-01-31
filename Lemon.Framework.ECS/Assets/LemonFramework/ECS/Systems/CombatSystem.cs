using Lemon.Framework.ECS.Entitys;
using Lemon.Framework.ECS.Components;

namespace Lemon.Framework.ECS.Systems
{
    public class CombatSystem
    {
        private readonly World world;

        public CombatSystem(World world)
        {
            this.world = world;
        }

        public void PerformAttack(Entity attacker, Entity defender)
        {
            var attackComponent = world.GetComponent<AttackComponent>(attacker);
            var healthComponent = world.GetComponent<HealthComponent>(defender);
            healthComponent.Health -= attackComponent.Damage;
        }
    }
}