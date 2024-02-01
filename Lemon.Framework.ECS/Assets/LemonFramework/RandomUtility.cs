using UnityEngine;

namespace LemonFramework
{
    public class RandomUtility
    {
        private static System.Random random = new System.Random(123456);
          
        public static int GetDeterministicRandom()
        {
            return random.Next();
        }
        
        public static int GetDeterministicRandom(int min,int max)
        {
            return random.Next(min,max);
        }


        public static void UnitTest()
        {
            Debug.Log(GetDeterministicRandom(10,100));
            Debug.Log(GetDeterministicRandom(50,150));
            Debug.Log(GetDeterministicRandom(60,700));
            Debug.Log(GetDeterministicRandom(80,900));
            Debug.Log(GetDeterministicRandom(10000,90000));
        }
    }
}