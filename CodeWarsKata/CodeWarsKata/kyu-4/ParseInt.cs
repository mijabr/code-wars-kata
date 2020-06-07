using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeWarsKata.ParseInt
{
    public class Parser
    {
        private static readonly Dictionary<string, int> ValueTokens = new Dictionary<string, int>
        {
            { "ten", 10 },
            { "twenty", 20 },
            { "thirty", 30 },
            { "forty", 40 },
            { "fifty", 50 },
            { "sixty", 60 },
            { "seventy", 70 },
            { "eighty", 80 },
            { "ninety", 90 },
            { "eleven", 11 },
            { "twelve", 12 },
            { "thirteen", 13 },
            { "fourteen", 14 },
            { "fifteen", 15 },
            { "sixteen", 16 },
            { "seventeen", 17 },
            { "eighteen", 18 },
            { "nineteen", 19 },
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
        };

        private static readonly Dictionary<string, (int, int)> GroupTokens = new Dictionary<string, (int value, int group)>
        {
            { "hundred", (100, 1) },
            { "thousand", (1000, 2) },
            { "million", (1000000, 3) },
        };
        
        public static int ParseInt(string s)
        {
            var groupTotals = new int[GroupTokens.Count + 1];
            var words = s.Split(' ', '-');

            foreach (var word in words)
            {
                if (ValueTokens.TryGetValue(word, out var tokenValue))
                {
                    groupTotals[0] += tokenValue;
                }
                else if (GroupTokens.TryGetValue(word, out (int value, int group) tokenGroup))
                {
                    for (var groupIndex = 0; groupIndex < tokenGroup.group; groupIndex++)
                    {
                        groupTotals[tokenGroup.group] += groupTotals[groupIndex];
                        groupTotals[groupIndex] = 0;
                    }

                    groupTotals[tokenGroup.group] *= tokenGroup.value;
                }
            }

            return groupTotals.Sum();
        }
    }

    public class SolutionTest
    {
        [Test]
        public void CanParseSingleDigits()
        {
            Parser.ParseInt("zero").Should().Be(0);
            Parser.ParseInt("one").Should().Be(1);
            Parser.ParseInt("two").Should().Be(2);
            Parser.ParseInt("three").Should().Be(3);
            Parser.ParseInt("four").Should().Be(4);
            Parser.ParseInt("five").Should().Be(5);
            Parser.ParseInt("six").Should().Be(6);
            Parser.ParseInt("seven").Should().Be(7);
            Parser.ParseInt("eight").Should().Be(8);
            Parser.ParseInt("nine").Should().Be(9);
        }
        [Test]
        public void CanParseNumbersUpToNineteen()
        {
            Parser.ParseInt("eleven").Should().Be(11);
            Parser.ParseInt("twelve").Should().Be(12);
            Parser.ParseInt("thirteen").Should().Be(13);
            Parser.ParseInt("fourteen").Should().Be(14);
            Parser.ParseInt("fifteen").Should().Be(15);
            Parser.ParseInt("sixteen").Should().Be(16);
            Parser.ParseInt("seventeen").Should().Be(17);
            Parser.ParseInt("eighteen").Should().Be(18);
            Parser.ParseInt("nineteen").Should().Be(19);
        }

        [Test]
        public void CanParseMultiplesOfTenDigits()
        {
            Parser.ParseInt("ten").Should().Be(10);
            Parser.ParseInt("twenty").Should().Be(20);
            Parser.ParseInt("thirty").Should().Be(30);
            Parser.ParseInt("forty").Should().Be(40);
            Parser.ParseInt("fifty").Should().Be(50);
            Parser.ParseInt("sixty").Should().Be(60);
            Parser.ParseInt("seventy").Should().Be(70);
            Parser.ParseInt("eighty").Should().Be(80);
            Parser.ParseInt("ninety").Should().Be(90);
        }

        [Test]
        public void CanParseNonMultiplesOfTenUnderOneHundred()
        {
            Parser.ParseInt("twenty-one").Should().Be(21);
            Parser.ParseInt("thirty-one").Should().Be(31);
            Parser.ParseInt("sixty-six").Should().Be(66);
            Parser.ParseInt("seventy-six").Should().Be(76);
            Parser.ParseInt("ninety-nine").Should().Be(99);
        }

        [Test]
        public void CanParseHundreds()
        {
            Parser.ParseInt("one hundred").Should().Be(100);
            Parser.ParseInt("six hundred").Should().Be(600);
            Parser.ParseInt("nine hundred").Should().Be(900);
        }

        [Test]
        public void CanParseThousands()
        {
            Parser.ParseInt("two thousand").Should().Be(2000);
            Parser.ParseInt("seven thousand").Should().Be(7000);
            Parser.ParseInt("eight thousand").Should().Be(8000);
        }

        [Test]
        public void CanParseHundredsOfThousands()
        {
            Parser.ParseInt("four hundred thousand").Should().Be(400000);
            Parser.ParseInt("five hundred thousand").Should().Be(500000);
            Parser.ParseInt("six hundred thousand").Should().Be(600000);
        }

        [Test]
        public void CanParseOneMillion()
        {
            Parser.ParseInt("one million").Should().Be(1000000);
        }

        [Test]
        public void CanParseHundredsPlusExtras()
        {
            Parser.ParseInt("five hundred and seventy five").Should().Be(575);
            Parser.ParseInt("seven hundred ninety three").Should().Be(793);
            Parser.ParseInt("nine hundred and eleven").Should().Be(911);
        }

        [Test]
        public void CanParseThousandsPlusExtras()
        {
            Parser.ParseInt("one thousand eight hundred and twenty seven").Should().Be(1827);
            Parser.ParseInt("seven thousand eight hundred and twenty seven").Should().Be(7827);
        }

        [Test]
        public void CanParseHundredsOfThousandsPlusExtras()
        {
            Parser.ParseInt("one hundred twenty seven thousand four hundred and eighteen").Should().Be(127418);
        }

        [Test]
        public void CanParseMillions()
        {
            Parser.ParseInt("four hundred thirteen million one hundred twenty seven thousand four hundred and eighteen").Should().Be(413127418);
            Parser.ParseInt("four hundred million").Should().Be(400000000);
        }

        [Test]
        public void FixedTests()
        {
            Assert.AreEqual(1, Parser.ParseInt("one"));
            Assert.AreEqual(20, Parser.ParseInt("twenty"));
            Assert.AreEqual(246, Parser.ParseInt("two hundred forty-six"));
        }
    }
}
