using System;
using System.Collections.Generic;

namespace LemonFramework.AStar
{
    public class Node
    {
        public readonly Vector2 Position;

        /// <summary>
        /// 起点到该点的距离;
        /// </summary>
        public int G;

        /// <summary>
        /// 启发函数预测的次点到终点的距离;
        /// </summary>
        public int H;

        public int F => G + H;

        /// <summary>
        /// 父节点;
        /// </summary>
        public Node Parent;

        /// <summary>
        /// 邻居节点;
        /// </summary>
        private List<Node> _neighbors;

        private bool _isNeighborInitialized = false;

        public Node(Vector2 position)
        {
            Position = position;
        }

        public Node(int x, int y) : this(new Vector2(x, y))
        {
        }

        public List<Node> GetNeighbors()
        {
            if (!_isNeighborInitialized)
            {
                _isNeighborInitialized = true;
                _neighbors = new List<Node>
                {
                    new(Position.X - 1, Position.Y),
                    new(Position.X - 1, Position.Y - 1),
                    new(Position.X - 1, Position.Y + 1),
                    new(Position.X + 1, Position.Y),
                    new(Position.X + 1, Position.Y - 1),
                    new(Position.X + 1, Position.Y + 1),
                    new(Position.X, Position.Y - 1),
                    new(Position.X, Position.Y + 1)
                };
            }

            return _neighbors;
        }

        public bool Equal(Node node)
        {
            if (node == null) return false;
            return Position == node.Position;
        }

        public int GetH(Node target)
        {
            int a = Math.Abs(Position.X - target.Position.X);
            int b = Math.Abs(Position.Y - target.Position.Y);
            return 10 * (int)Math.Sqrt(a * a + b * b);
        }

        public int GetG(Node target)
        {
            if (target.Parent.Equal(null))
                return 0;

            if (target.Position == Position)
            {
                return 0;
            }

            // 和自己同行或者同列,返回10;
            if (target.Position.X == Position.X || target.Position.Y == Position.Y)
            {
                return 10;
            }

            // 在自己的四个拐角就返回14;
            return 14;
        }
    }
}