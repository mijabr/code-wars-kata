using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;

namespace CodeWarsKata
{
    public class Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Direction
    {
        public Direction(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Snail
    {
        public Position Position { get; set; } = new Position(0, 0);
        public Direction Direction => directions[currentDirectionIndex];

        private int[][] array;

        public int[][] Array
        {
            set
            {
                array = value;
                SetBoundary();
            }
            get => array;
        }

        public int CurrentArrayElement => array[Position.Y][Position.X];

        private void SetBoundary()
        {
            boundaryTop = 0;
            boundaryLeft = 0;
            boundaryBottom = array.Length - 1;
            boundaryRight = array.Length - 1;
        }

        private readonly Direction[] directions = { new Direction(1, 0), new Direction(0, 1), new Direction(-1, 0), new Direction(0, -1) };
        private int currentDirectionIndex;
        private int boundaryTop;
        private int boundaryLeft;
        private int boundaryBottom;
        private int boundaryRight;

        public bool Move(int times)
        {
            var result = false;
            while (times > 0)
            {
                result = Move();
                times--;
            }

            return result;
        }

        public bool Move()
        {
            if (Position.X + Direction.X > boundaryRight ||
                Position.Y + Direction.Y > boundaryBottom ||
                Position.X + Direction.X < boundaryLeft ||
                Position.Y + Direction.Y < boundaryTop)
            {
                Turn();
            }

            if (Position.X + Direction.X > boundaryRight ||
                Position.Y + Direction.Y > boundaryBottom ||
                Position.X + Direction.X < boundaryLeft ||
                Position.Y + Direction.Y < boundaryTop)
            {
                return false;
            }

            Position.X += Direction.X;
            Position.Y += Direction.Y;

            return true;
        }

        private void Turn()
        {
            currentDirectionIndex++;
            if (currentDirectionIndex > directions.Length - 1)
            {
                currentDirectionIndex = 0;
            }

            if (IsFacingDown())
            {
                boundaryTop++;
            }

            if (IsFacingLeft())
            {
                boundaryRight--;
            }

            if (IsFacingUp())
            {
                boundaryBottom--;
            }

            if (IsFacingRight())
            {
                boundaryLeft++;
            }
        }

        private bool IsFacingDown()
        {
            return currentDirectionIndex == 1;
        }

        private bool IsFacingLeft()
        {
            return currentDirectionIndex == 2;
        }

        private bool IsFacingUp()
        {
            return currentDirectionIndex == 3;
        }

        private bool IsFacingRight()
        {
            return currentDirectionIndex == 0;
        }

        public int[] GetAnswers()
        {
            if (array == null || Array.Length == 0 || Array[0].Length == 0) return new int[0];

            var answer = new List<int>();
            answer.Add(CurrentArrayElement);
            while (Move())
            {
                answer.Add(CurrentArrayElement);
            }

            return answer.ToArray();
        }
    }

    public class SnailSolution
    {
        public static int[] Snail(int[][] array)
        {
            var snail = new Snail {Array = array};
            return snail.GetAnswers();
        }
    }

    [TestFixture]
    public class SnailTests
    {
        private Snail snail;

        [SetUp]
        public void SetUp()
        {
            snail = new Snail {Array = new[] {new[] {1, 2, 3}, new[] {4, 5, 6}, new[] {7, 8, 9}}};
        }

        [Test]
        public void SnailShouldStartOnTopLeftAndFaceRight()
        {
            snail.Position.X.ShouldBe(0);
            snail.Position.Y.ShouldBe(0);
            snail.Direction.X.ShouldBe(1);
            snail.Direction.Y.ShouldBe(0);
            snail.CurrentArrayElement.ShouldBe(1);
        }

        [Test]
        public void SnailShouldMoveToNextPosition()
        {
            snail.Move();

            snail.Position.X.ShouldBe(1);
            snail.Position.Y.ShouldBe(0);
        }

        [Test]
        public void SnailShouldMakeFirstTurn()
        {
            snail.Move(3);

            snail.Position.X.ShouldBe(2);
            snail.Position.Y.ShouldBe(1);
        }

        [Test]
        public void SnailShouldSecondTurn()
        {
            snail.Move(5);

            snail.Position.X.ShouldBe(1);
            snail.Position.Y.ShouldBe(2);
        }

        [Test]
        public void SnailShouldMakeThirdTurn()
        {
            snail.Move(7);

            snail.Position.X.ShouldBe(0);
            snail.Position.Y.ShouldBe(1);
        }

        [Test]
        public void SnailShouldMakeFourthTurn()
        {
            snail.Move(8);

            snail.Position.X.ShouldBe(1);
            snail.Position.Y.ShouldBe(1);
        }

        [Test]
        public void SnailShouldDetectWhenCantMove()
        {
            snail.Move(9).ShouldBe(false);
        }

        [Test]
        public void TestWith0X0()
        {
            int[][] array =
            {
                new int[0]
            };
            var expected = new int[0];

            snail.Array = array;
            snail.GetAnswers().ShouldBe(expected);
        }

        [Test]
        public void TestWith3X3()
        {
            int[][] array =
            {
                new []{1, 2, 3},
                new []{4, 5, 6},
                new []{7, 8, 9}
            };
            var expected = new[] { 1, 2, 3, 6, 9, 8, 7, 4, 5 };

            snail.Array = array;
            snail.GetAnswers().ShouldBe(expected);
        }

        [Test]
        public void TestWith4X4()
        {
            int[][] array =
            {
                new []{1, 2, 3, 4},
                new []{5, 6, 7, 8},
                new []{9, 10,11,12},
                new []{13,14,15,16}
            };
            var expected = new[] { 1, 2, 3, 4, 8, 12, 16, 15, 14, 13, 9, 5, 6, 7, 11, 10};

            snail.Array = array;
            snail.GetAnswers().ShouldBe(expected);
        }
    }
}
