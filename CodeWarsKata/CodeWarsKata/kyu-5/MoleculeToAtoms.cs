using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeWarsKata.MoleculeToAtoms
{
    public class Kata
    {
        public static Dictionary<string, int> ParseMolecule(string formula)
        {
            if (formula == null) return new Dictionary<string, int>();

            var molecules = new Stack<Dictionary<string, int>>();
            molecules.Push(new Dictionary<string, int>());

            for (var index = 0; index < formula.Length; index++)
            {
                if (formula[index] == '(' || formula[index] == '[' || formula[index] == '{')
                {
                    molecules.Push(new Dictionary<string, int>());
                    continue;
                }

                Dictionary<string, int> molecule;
                if (formula[index] == ')' || formula[index] == ']' || formula[index] == '}')
                {
                    molecule = molecules.Pop();
                }
                else
                {
                    string element;
                    var elementStart = index;
                    var elementLength = 1;
                    while (index + 1 < formula.Length && char.IsLower(formula[index + 1]))
                    {
                        elementLength++;
                        index++;
                    }

                    element = formula.Substring(elementStart, elementLength);

                    molecule = new Dictionary<string, int> { {element, 1 } };
                }

                var multiplier = 1;
                var multiplierStart = index + 1;
                var multiplierLength = 0;
                while (index + 1 < formula.Length && char.IsNumber(formula[index + 1]))
                {
                    multiplierLength++;
                    index++;
                }

                if (multiplierLength > 0)
                {
                    multiplier = int.Parse(formula.Substring(multiplierStart, multiplierLength));
                }

                var currentMolecule = molecules.Pop();
                foreach (var element in molecule)
                {
                    if (currentMolecule.ContainsKey(element.Key))
                    {
                        currentMolecule[element.Key] += element.Value * multiplier;
                    }
                    else
                    {
                        currentMolecule.Add(element.Key, element.Value * multiplier);
                    }
                }

                molecules.Push(currentMolecule);
            }

            return molecules.Pop();
        }
    }

    [TestFixture]
    public class ParseMoleculeTest
    {
        [Test]
        public void NullShouldReturnEmptyDictionary()
        {
            Kata.ParseMolecule(null).Should().BeEquivalentTo(new Dictionary<string, int>());
        }

        [Test]
        public void EmptyShouldReturnEmptyDictionary()
        {
            Kata.ParseMolecule(string.Empty).Should().BeEquivalentTo(new Dictionary<string, int>());
        }

        [Test]
        public void CanParseSingleLetterElements()
        {
            Kata.ParseMolecule("OPNO").Should().BeEquivalentTo(new Dictionary<string, int>{ {"O", 2}, {"P", 1}, {"N", 1}});
        }

        [Test]
        public void CanParseTwoLetterElements()
        {
            Kata.ParseMolecule("MgPN").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 1 }, { "P", 1 }, { "N", 1 } });
        }

        [Test]
        public void CanParseManyLetterElements()
        {
            Kata.ParseMolecule("GrrElemT").Should().BeEquivalentTo(new Dictionary<string, int> { { "Grr", 1 }, { "Elem", 1 }, { "T", 1 } });
        }

        [Test]
        public void CanParseNumberMultiplierElements()
        {
            Kata.ParseMolecule("Mg2P3N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 2 }, { "P", 3 }, { "N", 1 } });
        }

        [Test]
        public void CanParseMultiDigitNumberMultiplierElements()
        {
            Kata.ParseMolecule("Mg17P3N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 17 }, { "P", 3 }, { "N", 1 } });
        }

        [Test]
        public void CanParseBracketedMultiplierElements()
        {
            Kata.ParseMolecule("(MgP)3N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 3 }, { "P", 3 }, { "N", 1 } });
        }

        [Test]
        public void CanParseSquareBracketedMultiplierElements()
        {
            Kata.ParseMolecule("[MgP]3N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 3 }, { "P", 3 }, { "N", 1 } });
        }

        [Test]
        public void CanParseCurlyBracketedMultiplierElements()
        {
            Kata.ParseMolecule("{MgP}3N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 3 }, { "P", 3 }, { "N", 1 } });
        }

        [Test]
        public void CanParseNestedBracketedMultiplierElements()
        {
            Kata.ParseMolecule("({MgP}3)2N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 6 }, { "P", 6 }, { "N", 1 } });
        }

        [Test]
        public void CanParseOuterNestedBracketedMultiplierElements()
        {
            Kata.ParseMolecule("((MgP)10S)20N").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 200 }, { "P", 200 }, { "S", 20 }, { "N", 1 } });
        }

        [Test]
        public void SampleTests()
        {
            Kata.ParseMolecule("H2O").Should().BeEquivalentTo(new Dictionary<string, int> { { "H", 2 }, { "O", 1 } });
            Kata.ParseMolecule("Mg(OH)2").Should().BeEquivalentTo(new Dictionary<string, int> { { "Mg", 1 }, { "O", 2 }, { "H", 2 } });
            Kata.ParseMolecule("K4[ON(SO3)2]2").Should().BeEquivalentTo(new Dictionary<string, int> { { "K", 4 }, { "O", 14 }, { "N", 2 }, { "S", 4 } });
        }
    }
}
