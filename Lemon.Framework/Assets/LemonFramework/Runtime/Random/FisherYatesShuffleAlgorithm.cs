using System;
using SRandom = System.Random;

namespace LemonFramework.Random
{
    /// <summary>
    /// Fisher-Yates Shuffle Algorithm
    /// </summary>
    public class FisherYatesShuffleAlgorithm
    {
        private static SRandom _random;

        public FisherYatesShuffleAlgorithm()
        {
            _random = new SRandom();
        }

        public FisherYatesShuffleAlgorithm(int seed)
        {
            _random = new SRandom(seed);
        }

        public static void Next<T>(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            int n = array.Length;
            if (n <= 1)
                return;
            for (var i = n - 1; i > 0; i--)
            {
                var j = _random.Next(i + 1);
                //Swap(array[i], array[j]);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}