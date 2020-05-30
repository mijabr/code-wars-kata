using FluentAssertions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Text;

namespace CodeWarsKata.ReverseOrRotate
{
    // The input is a string str of digits.Cut the string into chunks (a chunk here is a substring of the initial string)
    // of size sz(ignore the last chunk if its size is less than sz).
    // 
    // If a chunk represents an integer such as the sum of the cubes of its digits is divisible by 2, reverse that chunk; 
    // otherwise rotate it to the left by one position.Put together these modified chunks and return the result as a string.

    public class Revrot
    {
        public static string RevRot(string str, int sz)
        {
            if (string.IsNullOrEmpty(str) || sz <= 0) return string.Empty;

            var index = 0;
            var sb = new StringBuilder();

            while (index + sz - 1 < str.Length)
            {
                var chunk = str.Substring(index, sz);
                if (IsCubeDigitSumEven(chunk))
                {
                    for (var i2 = chunk.Length - 1; i2 >= 0; i2--)
                    {
                        sb.Append(chunk[i2]);
                    }
                }
                else
                {
                    sb.Append(chunk.Substring(1, sz - 1));
                    sb.Append(chunk.Substring(0, 1));
                }

                index += sz;
            }

            return sb.ToString();
        }

        public static bool IsCubeDigitSumEven(string str)
        {
            var sum = str.Sum(digit =>
            {
                var n = (digit - '0');
                return n * n * n;
            });

            return sum % 2 == 0;
        }
    }

    [TestFixture]
    public class ReverseOrRotateTests
    {
        private static void testing(string actual, string expected)
        {
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanCheckForDigitCubeSumEvenness()
        {
            Revrot.IsCubeDigitSumEven("12345").ShouldBe(false);
            Revrot.IsCubeDigitSumEven("98765").ShouldBe(false);
            Revrot.IsCubeDigitSumEven("2").ShouldBe(true);
            Revrot.IsCubeDigitSumEven("2345").ShouldBe(true);
        }

        [Test]
        public void NullShouldReturnEmptyString()
        {
            Revrot.RevRot(null, 3).Should().Be(string.Empty);
        }

        [Test]
        public void SizeZeroOrNegativeShouldReturnEmptyString()
        {
            Revrot.RevRot("123", 0).Should().Be(string.Empty);
            Revrot.RevRot("123", -1).Should().Be(string.Empty);
        }

        [Test]
        public void CanRotateSingleChunk()
        {
            (((1 * 1 * 1) + (2 * 2 * 2) + (3 * 3 * 3) + (4 * 4 * 4) + (5 * 5 * 5)) % 2).ShouldNotBe(0);
            Revrot.RevRot("12345", 5).Should().Be("23451");
        }

        [Test]
        public void CanRotateSingleChunkAndIgnoreLeftOvers()
        {
            Revrot.RevRot("123455", 5).Should().Be("23451");
            Revrot.RevRot("1234556", 5).Should().Be("23451");
            Revrot.RevRot("12345564", 5).Should().Be("23451");
            Revrot.RevRot("123455643", 5).Should().Be("23451");
        }

        [Test]
        public void CanRotateTwoChunks()
        {
            (((1 * 1 * 1) + (2 * 2 * 2) + (3 * 3 * 3) + (4 * 4 * 4) + (5 * 5 * 5)) % 2).ShouldNotBe(0);
            (((9 * 9 * 9) + (8 * 8 * 8) + (7 * 7 * 7) + (6 * 6 * 6) + (5 * 5 * 5)) % 2).ShouldNotBe(0);
            Revrot.RevRot("1234598765", 5).Should().Be("2345187659");
        }

        [Test]
        public void CanReverseSingleChunk()
        {
            (((2 * 2 * 2) + (3 * 3 * 3) + (3 * 3 * 3)) % 2).ShouldBe(0);
            Revrot.RevRot("233", 3).Should().Be("332");
        }

        [Test]
        public void CanReverseTwoChunks()
        {
            (((2 * 2 * 2) + (3 * 3 * 3) + (3 * 3 * 3)) % 2).ShouldBe(0);
            (((8 * 8 * 8) + (5 * 5 * 5) + (9 * 9 * 9)) % 2).ShouldBe(0);
            Revrot.RevRot("233859", 3).Should().Be("332958");
        }

        [Test]
        public void SampleTests()
        {
            Console.WriteLine("Testing RevRot");
            testing(Revrot.RevRot("1234", 0), "");
            testing(Revrot.RevRot("", 0), "");
            testing(Revrot.RevRot("1234", 5), "");
            testing(Revrot.RevRot("733049910872815764", 5), "330479108928157");
            testing(Revrot.RevRot("123456987654", 6), "234561876549");
            testing(Revrot.RevRot("123456987653", 6), "234561356789");
            testing(Revrot.RevRot("66443875", 4), "44668753");
            testing(Revrot.RevRot("66443875", 8), "64438756");
            testing(Revrot.RevRot("664438769", 8), "67834466");
            testing(Revrot.RevRot("123456779", 8), "23456771");
            testing(Revrot.RevRot("", 8), "");
            testing(Revrot.RevRot("123456779", 0), "");
            testing(Revrot.RevRot("563000655734469485", 4), "0365065073456944");
        }
    }
}
