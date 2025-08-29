namespace LemonFramework.ECS.Demo.Components
{
    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
            => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Vector3 operator *(Vector3 a, float b)
            => new Vector3(a.x * b, a.y * b, a.z * b);
    }
}