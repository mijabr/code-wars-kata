using NUnit.Framework;
using Shouldly;

namespace CodeWarsKata.MovingZerosToTheEnd
{
    public class Kata
    {
        public static int[] MoveZeroes(int[] arr)
        {
            if ((arr?.Length ?? 0) == 0) return arr;

            var index = 0;
            var indexOut = 0;
            while (index < arr.Length)
            {
                while (index < arr.Length && arr[index] == 0)
                {
                    index++;
                }

                if (index < arr.Length)
                {
                    arr[indexOut++] = arr[index];
                    index++;
                }
            }

            while (indexOut < index)
            {
                arr[indexOut++] = 0;
            }

            return arr;
        }
    }

    [TestFixture]
    public class MovingZerosToTheEndTest
    {
        private int[] MoveZeroes(int[] arr)
        {
            return Kata.MoveZeroes(arr);
        }

        [Test]
        public void NullTest()
        {
            MoveZeroes(null).ShouldBe(null);
        }

        [Test]
        public void EmptyTest()
        {
            MoveZeroes(new int[0]).ShouldBe(new int[0]);
        }

        [Test]
        public void NoZerosTest()
        {
            MoveZeroes(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
        }

        [Test]
        public void AllZerosTest()
        {
            MoveZeroes(new[] { 0, 0, 0 }).ShouldBe(new[] { 0, 0, 0 });
        }

        [Test]
        public void ZerosOnEndTest()
        {
            MoveZeroes(new[] { 1, 2, 3, 0, 0 }).ShouldBe(new[] { 1, 2, 3, 0, 0 });
        }

        [Test]
        public void SingleZeroTest()
        {
            MoveZeroes(new[] { 0, 1 }).ShouldBe(new[] { 1, 0 });
        }

        [Test]
        public void StartingAndEndingWithZeroTest()
        {
            MoveZeroes(new[] { 0, 1, 6, 1, 0 }).ShouldBe(new[] { 1, 6, 1, 0, 0 });
        }

        [Test]
        public void SampleTest()
        {
            MoveZeroes(new [] { 1, 2, 0, 1, 0, 1, 0, 3, 0, 1 }).ShouldBe(new[] { 1, 2, 1, 1, 3, 1, 0, 0, 0, 0 });
        }
    }
}
