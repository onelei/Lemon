using System;

namespace LemonFramework.AStar
{
    public readonly struct Vector2
    {
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public readonly int X;
        public readonly int Y;

        public Vector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(Vector2 point1, Vector2 point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(Vector2 point1, Vector2 point2)
        {
            return (point1.X != point2.X || point1.Y != point2.Y);
        }
    }
}