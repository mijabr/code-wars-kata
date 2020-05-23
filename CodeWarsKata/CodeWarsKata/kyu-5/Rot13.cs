using FluentAssertions;
using NUnit.Framework;
using System.Text;

namespace CodeWarsKata.Rot13
{
    public class Kata
    {
        public static string Rot13(string input)
        {
            var sb = new StringBuilder();
            foreach (var letter in input)
            {
                if ((letter >= 'A' && letter <= 'M') || (letter >= 'a' && letter <= 'm'))
                {
                    sb.Append((char) (letter + 13));
                }
                else if ((letter >= 'N' && letter <= 'Z') || (letter >= 'n' && letter <= 'z'))
                {
                    sb.Append((char)(letter - 13));
                }
                else
                {
                    sb.Append(letter);
                }
            }

            return sb.ToString();
        }
    }

    [TestFixture]
    public class SystemTests
    {
        [Test]
        public void CanParseA()
        {
            Kata.Rot13("aA").Should().Be("nN");
        }

        [Test]
        public void CanParseN()
        {
            Kata.Rot13("nN").Should().Be("aA");
        }

        [Test]
        public void ShouldNotChangeNonAlphaCharacters()
        {
            Kata.Rot13("123").Should().Be("123");
        }

        [Test]
        public void SampleTests()
        {
            Kata.Rot13("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz").Should().Be("NOPQRSTUVWXYZABCDEFGHIJKLMnopqrstuvwxyzabcdefghijklm");
            Kata.Rot13("Why did the chicken cross the road?").Should().Be("Jul qvq gur puvpxra pebff gur ebnq?");
            Kata.Rot13("To get to the other side!").Should().Be("Gb trg gb gur bgure fvqr!");
            Kata.Rot13("EBG13 rknzcyr.").Should().Be("ROT13 example.");
        }
    }
}
