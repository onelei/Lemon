using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lemon.Framework.ECS.Entitys
{
    public class Entity
    {
        public readonly int Id;

        public Entity(int id)
        {
            Id = id;
        }
    }
}