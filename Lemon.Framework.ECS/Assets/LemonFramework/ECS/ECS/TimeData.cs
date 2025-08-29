namespace LemonFramework.ECS
{
    public struct TimeData
    {
        public int Frame;
        public int TargetFrame;
        public float DeltaTime;
        public float MilliSeconds;

        public TimeData(int targetFrame)
        {
            Frame = 0;
            TargetFrame = targetFrame;
            DeltaTime = 1.0f / targetFrame;
            MilliSeconds = DeltaTime * 1000;
        }
    }
}