using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text;

namespace CodeWarsKata.ConwaysGameOfLifeUnlimited
{
    public class ConwayLife
    {
        public static int[,] GetGeneration(int[,] cells, int generations)
        {
            while (generations-- > 0)
            {
                cells = DoLife(cells);
            }

            return cells;
        }

        public static int[,] DoLife(int[,] cells)
        {
            var width = cells.GetUpperBound(0);
            var height = cells.GetUpperBound(1);
            var top = height;
            var bottom = 0;
            var left = width;
            var right = 0;
            var nextGen = new int[width + 3, height + 3];

            for (var y = -1; y <= height + 1; y++)
            {
                for (var x = -1; x <= width + 1; x++)
                {
                    var result = CountNeighbours(cells, x, y);
                    if (result.NeighbourCount == 3 || result.NeighbourCount == 2 && result.Self)
                    {
                        nextGen[x + 1, y + 1] = 1;
                        if (left > x + 1) left = x + 1;
                        if (right < x + 1) right = x + 1;
                        if (top > y + 1) top = y + 1;
                        if (bottom < y + 1) bottom = y + 1;
                    }
                }
            }

            if (right - left + 1 < 0 || bottom - top + 1 < 0)
            {
                return new int[0, 0];
            }

            var nextGenTruncated = new int[right - left + 1, bottom - top + 1];
            for (int y2 = top, dy = 0; y2 <= bottom; y2++, dy++)
            {
                for (int x2 = left, dx = 0; x2 <= right; x2++, dx++)
                {
                    nextGenTruncated[dx, dy] = nextGen[x2, y2];
                }
            }

            return nextGenTruncated;
        }

        public class CountNeighboursResult
        {
            public bool Self { get; set; }
            public int NeighbourCount { get; set; }
        }

        public static CountNeighboursResult CountNeighbours(int[,] cells, int x, int y)
        {
            var width = cells.GetUpperBound(0);
            var height = cells.GetUpperBound(1);
            var result = new CountNeighboursResult();
            for (var dy = y - 1; dy <= y + 1; dy++)
            {
                for (var dx = x - 1; dx <= x + 1; dx++)
                {
                    if (dx >= 0 && dx <= width && dy >= 0 && dy <= height)
                    {
                        if (dx == x && dy == y)
                        {
                            result.Self = cells[dx, dy] != 0;
                        }
                        else
                        {
                            result.NeighbourCount += cells[dx, dy];
                        }
                    }
                }
            }

            return result;
        }
    }

    public class Dump
    {
        public static void Array(int[,] array)
        {
            for (var y = 0; y <= array.GetUpperBound(1); y++)
            {
                var row = new StringBuilder();
                for (var x = 0; x <= array.GetUpperBound(0); x++)
                {
                    row.Append(array[x, y]);
                }
                Console.WriteLine(row.ToString());
            }
        }
    }

    [TestFixture]
    public class SolutionTest
    {
        [Test]
        public void TestEmpty()
        {
            var empty = new[,]
            {
                {0, 0, 0},
                {0, 0, 0},
                {0, 0, 0}
            };

            ConwayLife.GetGeneration(empty, 1).Should().BeEquivalentTo(new int [0, 0]);
        }

        [Test]
        public void TestBlock()
        {
            var block = new[,]
            {
                {1, 1},
                {1, 1}
            };

            ConwayLife.GetGeneration(block, 1).Should().BeEquivalentTo(block);
        }

        [Test]
        public void TestBeeHive()
        {
            var beeHive = new[,]
            {
                {0, 1, 1, 0},
                {1, 0, 0, 1},
                {0, 1, 1, 0}
            };

            ConwayLife.GetGeneration(beeHive, 1).Should().BeEquivalentTo(beeHive);
        }

        [Test]
        public void TestBlinker()
        {
            int[][,] blinker =
            {
                new[,]
                {
                    {1},
                    {1},
                    {1}
                },
                new[,]
                {
                    {1, 1, 1}
                }
            };

            ConwayLife.GetGeneration(blinker[0], 1).Should().BeEquivalentTo(blinker[1]);
            ConwayLife.GetGeneration(blinker[1], 1).Should().BeEquivalentTo(blinker[0]);
        }

        [Test]
        public void TestToad()
        {
            int[][,] toad =
            {
                new[,]
                {
                    {0, 1, 1, 1},
                    {1, 1, 1, 0}
                },
                new[,]
                {
                    {0, 0, 1, 0},
                    {1, 0, 0, 1},
                    {1, 0, 0, 1},
                    {0, 1, 0, 0}
                },
            };

            ConwayLife.GetGeneration(toad[0], 1).Should().BeEquivalentTo(toad[1]);
            ConwayLife.GetGeneration(toad[1], 1).Should().BeEquivalentTo(toad[0]);
        }

        [Test]
        public void TestGlider()
        {
            int[][,] gliders =
            {
                new[,]
                {
                    {1, 0, 0},
                    {0, 1, 1},
                    {1, 1, 0}
                },
                new[,]
                {
                    {0, 1, 0},
                    {0, 0, 1},
                    {1, 1, 1}
                },
                new[,]
                {
                    {1, 0, 1},
                    {0, 1, 1},
                    {0, 1, 0},
                },
                new[,]
                {
                    {0, 0, 1},
                    {1, 0, 1},
                    {0, 1, 1},
                }
            };

            ConwayLife.GetGeneration(gliders[0], 1).Should().BeEquivalentTo(gliders[1]);
            ConwayLife.GetGeneration(gliders[1], 1).Should().BeEquivalentTo(gliders[2]);
            ConwayLife.GetGeneration(gliders[2], 1).Should().BeEquivalentTo(gliders[3]);
            ConwayLife.GetGeneration(gliders[3], 1).Should().BeEquivalentTo(gliders[0]);
        }
    }
}
