using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeWarsKata
{
    public class Buffer
    {
        public Buffer(int size)
        {
            data = new byte[size];
        }

        private readonly byte[] data;

        public int Size => data.Length;
        public int Low => -data.Length / 2;
        public int High => data.Length / 2 - 1;

        private int TranslateIndex(int index)
        {
            return index + data.Length / 2;
        }

        public byte GetAt(int index)
        {
            return data[TranslateIndex(index)];
        }

        public void SetAt(int index, byte theData)
        {
            data[TranslateIndex(index)] = theData;
        }

        public void IncrementAt(int index)
        {
            data[TranslateIndex(index)]++;
        }

        public void DecrementAt(int index)
        {
            data[TranslateIndex(index)]--;
        }
    }

    public class Computer
    {
        public Computer()
        {
            Data = new Buffer(50000);
        }

        public int DataPointer { get; private set; }

        public int InstructionPointer { get; set; }

        public Buffer Data { get; }

        public string Code { get; private set; }

        public void SetCode(string code)
        {
            Code = code;
        }

        private IEnumerator<byte> inputEnumerator;

        public void SetInput(string theInput)
        {
            inputEnumerator = theInput?.Select(b => (byte)b).GetEnumerator();
        }

        private List<byte> output;

        public string Process(string code, string input, int maxInstructions = 1000000)
        {
            if (code == null) return string.Empty;

            SetCode(code);
            SetInput(input);
            output = new List<byte>();
            InstructionPointer = 0;
            DataPointer = 0;

            while (InstructionPointer < code.Length && maxInstructions-- > 0)
            {
                ProcessInstruction(code[InstructionPointer]);
            }

            var sb = new StringBuilder();
            output.ForEach(b => sb.Append(char.ConvertFromUtf32(b)));
            return sb.ToString();
        }

        public void ProcessInstruction(char instruction)
        {
            if (instruction == '>')
            {
                DataPointer++;
            }
            else if (instruction == '<')
            {
                DataPointer--;
            }
            else if (instruction == '+')
            {
                Data.IncrementAt(DataPointer);
            }
            else if (instruction == '-')
            {
                Data.DecrementAt(DataPointer);
            }
            else if (instruction == '.')
            {
                output.Add(Data.GetAt(DataPointer));
            }
            else if (instruction == ',')
            {
                inputEnumerator?.MoveNext();
                Data.SetAt(DataPointer, inputEnumerator?.Current ?? 0);
            }
            else if (instruction == '[')
            {
                ProcessJumpForwardInstruction(instruction);
            }
            else if (instruction == ']')
            {
                ProcessJumpBackInstruction(instruction);
            }

            InstructionPointer++;
        }

        private void ProcessJumpBackInstruction(char instruction)
        {
            if (Data.GetAt(DataPointer) == 0) return;

            var nested = 1;
            while (nested != 0)
            {
                InstructionPointer--;
                instruction = Code[InstructionPointer];
                if (instruction == '[') nested--;
                if (instruction == ']') nested++;
            }
        }

        private void ProcessJumpForwardInstruction(char instruction)
        {
            if (Data.GetAt(DataPointer) != 0) return;

            var nested = 1;
            while (nested != 0)
            {
                InstructionPointer++;
                instruction = Code[InstructionPointer];
                if (instruction == '[') nested++;
                if (instruction == ']') nested--;
            }
        }
    }

    public static class Kata
    {
        public static string BrainLuck(string code, string input)
        {
            var computer = new Computer();
            return computer.Process(code, input);
        }
    }

    [TestFixture]
    public class BufferTests
    {
        [Test]
        public void CanCreateBufferWithSize()
        {
            var buffer = new Buffer(100);

            buffer.Size.ShouldBe(100);
            buffer.Low.ShouldBe(-50);
            buffer.High.ShouldBe(49);

            buffer.SetAt(-50, 1);
            buffer.GetAt(-50).ShouldBe((byte)1);

            buffer.SetAt(49, 1);
            buffer.GetAt(49).ShouldBe((byte)1);
        }

        [Test]
        public void CanCreateBufferWithOddSize()
        {
            var buffer = new Buffer(99);

            buffer.Size.ShouldBe(99);
            buffer.Low.ShouldBe(-49);
            buffer.High.ShouldBe(48);

            buffer.SetAt(-49, 1);
            buffer.GetAt(-49).ShouldBe((byte)1);

            buffer.SetAt(48, 1);
            buffer.GetAt(48).ShouldBe((byte)1);
        }
    }

    [TestFixture]
    public class ComputerTests
    {
        private Computer computer;

        [SetUp]
        public void SetUp()
        {
            computer = new Computer();
        }

        [Test]
        public void NullCodeTest()
        {
            computer.Process(null, "").ShouldBe(string.Empty);
        }

        [Test]
        public void NullInputTest()
        {
            computer.Process(",", "").ShouldBe(string.Empty);
        }

        [Test]
        public void EmptyTest()
        {
            computer.Process("", "").ShouldBe(string.Empty);
        }

        [Test]
        public void CanIncrementDataPointer()
        {
            computer.Process(">", "");

            computer.DataPointer.ShouldBe(1);
            computer.InstructionPointer.ShouldBe(1);
        }

        [Test]
        public void CanDecrementDataPointer()
        {
            computer.Process("<", "");

            computer.DataPointer.ShouldBe(-1);
        }

        [Test]
        public void CanIncrementData()
        {
            computer.Process("+", "");

            computer.Data.GetAt(0).ShouldBe((byte)1);
        }

        [Test]
        public void CanIncrementDataPast255()
        {
            computer.Data.SetAt(0, 255);
            computer.Process("+", "");

            computer.Data.GetAt(0).ShouldBe((byte)0);
        }

        [Test]
        public void CanDecrementData()
        {
            computer.Data.SetAt(0, 20);
            computer.Process("-", "");

            computer.Data.GetAt(0).ShouldBe((byte)19);
        }

        [Test]
        public void CanDecrementDataBeforeZero()
        {
            computer.Process("-", "");

            computer.Data.GetAt(0).ShouldBe((byte)255);
        }

        [Test]
        public void CanOutputData()
        {
            var output = computer.Process(".", "");

            output.ShouldBe(char.ConvertFromUtf32(0));
        }

        [Test]
        public void CanAcceptInput()
        {
            computer.Process(",", "A");

            computer.Data.GetAt(0).ShouldBe((byte)'A');
        }

        [Test]
        public void GivenDataIsZero_ThenShouldJumpForward()
        {
            computer.Data.SetAt(0, 0);
            computer.SetCode("[.....]...");

            computer.ProcessInstruction('[');

            computer.InstructionPointer.ShouldBe(7);
        }

        [Test]
        public void GivenDataIsZero_AndNestedSquareBrackets_ThenShouldJumpForwardToMatchingBracket()
        {
            computer.Data.SetAt(0, 0);
            computer.InstructionPointer = 2;
            computer.SetCode("..[.[[]]..]...");

            computer.ProcessInstruction('[');

            computer.InstructionPointer.ShouldBe(11);
        }

        [Test]
        public void GivenDataNonZero_ThenShouldNotJumpForward()
        {
            computer.Data.SetAt(0, (byte)'X');
            computer.InstructionPointer = 2;
            computer.SetCode("..[.....]...");

            computer.ProcessInstruction('[');

            computer.InstructionPointer.ShouldBe(3);
        }

        [Test]
        public void GivenDataIsNonZero_ThenShouldJumpBack()
        {
            computer.Data.SetAt(0, (byte)'X');
            computer.InstructionPointer = 8;
            computer.SetCode("..[.....]...");
            computer.ProcessInstruction(']');

            computer.InstructionPointer.ShouldBe(3);
        }

        [Test]
        public void GivenDataIsNonZero_AndNestedSquareBrackets_ThenShouldJumpBackToMatchingBracket()
        {
            computer.Data.SetAt(0, (byte)'X');
            computer.InstructionPointer = 10;
            computer.SetCode("..[.[[..]]]...");

            computer.ProcessInstruction(']');

            computer.InstructionPointer.ShouldBe(3);
        }

        [Test]
        public void GivenDataIsZero_ThenShouldNotJumpBack()
        {
            computer.Data.SetAt(0, 0);
            computer.InstructionPointer = 6;
            computer.SetCode("[.....]...");

            computer.ProcessInstruction(']');

            computer.InstructionPointer.ShouldBe(7);
        }
    }

    [TestFixture]
    public class BrainLuckTest
    {
        private Computer computer;
        private int maxInstructions;
        private string output;

        [SetUp]
        public void SetUp()
        {
            maxInstructions = 100 * 1000;
            computer = new Computer();
        }

        private string WhenProcess(string code, string input = null)
        {
            output = computer.Process(code, input, maxInstructions);
            return output;
        }

        [Test]
        public void OutputUntil255Test()
        {
            WhenProcess(",+[-.,+]", "Codewars" + char.ConvertFromUtf32(255)).ShouldBe("Codewars");
        }

        [Test]
        public void OutputUntilZeroTest()
        {
            WhenProcess(",[.[-],]", "Codewars" + char.ConvertFromUtf32(0)).ShouldBe("Codewars");
        }

        [Test]
        public void MultiplyTest()
        {
            WhenProcess(",>,<[>[->+>+<<]>>[-<<+>>]<<<-]>>.", char.ConvertFromUtf32(8) + char.ConvertFromUtf32(9));

            output.ShouldBe(char.ConvertFromUtf32(72));
        }

        [Test]
        public void FactorialTest()
        {
            const string code = @"
  ++++++++++>>>+>>>>+>+<[[+++++[>++++
  ++++<-]>.<++++++[>--------<-]+<<]<<
  [<<]<.>>>>+<[->[<+>-[<+>-[<+>-[<+>-
  [<+>-[<+>-[<+>-[<+>-[<+>-[<[-]>-+>[
  <->-]<[->>>[>>]<<[->[>>+<<-]>+<<<<]
  <]>[-]+>+<<]]]]]]]]]]<[>+<-]+>>]<<[
  <<]>>[->>[>>]>>[-<<[<<]<<[<<]>[>[>>
  ]>>[>>]>>[>>]>+>+<<<<[<<]<<[<<]<<[<
  <]>-]>[>>]>>[>>]>>[>>]>[<<<[<<]<<[<
  <]<<[<<]>+>[>>]>>[>>]>>[>>]>-]<<<[<
  <]>[>[>>]>+>>+<<<<<[<<]>-]>[>>]>[<<
  <[<<]>+>[>>]>-]>>[<[<<+>+>-]<[>>>+<
  <<-]<[>>+<<-]>>>-]<[-]>>+[>[>>>>]>[
  >>>>]>[-]+>+<[<<<<]>-]>[>>>>]>[>>>>
  ]>->-[<<<+>>+>-]<[>+<-]>[[<<+>+>-]<
  [<->-[<->-[<->-[<->-[<->-[<->-[<->-
  [<->-[<->-[<-<---------->>[-]>>>>[-
  ]+>+<<<<<]]]]]]]]]]<[>>+<<-]>>]<+<+
  <[>>>+<<<-]<<[<<<<]<<<<<[<<]+>>]>>>
  >>[>>>>]+>[>>>>]<<<<[-<<<<]>>>>>[<<
  <<]<<<<<[<<]<<[<<]+>>]>>[>>]>>>>>[-
  >>>>]<<[<<<<]>>>>[>>>>]<<<<<<<<[>>>
  >[<<+>>->[<<+>>-]>]<<<<[<<]<<]<<<<<
  [->[-]>>>>>>>>[<<+>>->[<<+>>-]>]<<<
  <[<<]<<<<<<<]>>>>>>>>>[<<<<<<<+>>>>
  >>>->[<<<<<<<+>>>>>>>-]>]<<<<<<<<<]";
            maxInstructions = 100000; // this one never stops

            WhenProcess(code).ShouldBe("1\n1\n2\n6\n24\n120\n720\n5040\n40320\n362880\n3628800\n");
        }

        [Test]
        public void HelloWorldTest()
        {
            const string code = @"++++++++[>++++[>++>+++>+++>+<<<<-]>+>+>->>+[<]<-]>>.>---.+++++++..+++.>>.<-.<.+++.------.--------.>>+.>++.";

            WhenProcess(code).ShouldBe("Hello World!\n");
        }

        [Test]
        public void PityTheFoolTest()
        {
            const string code = @"++++++++++ [-> +++>+++++++>++++++++++>+++++++++++>++++++++++++ <<<<<]
>> +++ .< ++ .>>> ++ .< +++++  .> ++++ .> + .<<<< .>>> .< - .--- .<< .
>> + .> ----- ..--- .<<< + .";

            WhenProcess(code).ShouldBe("I pity the fool!");
        }

        [Test]
        public void AprilFoolsTest()
        {
            const string code = @"+++++[->----[---->+<]>++.-[++++>---<]>.++.---------.+++.[++>---<]>--.++[->+++<]>.+++++++++..---.+++++++.+[-->+++++<]>-.<]";

            WhenProcess(code).ShouldBe("April fools!");
        }

        [Test]
        public void AlphabetTest()
        {
            const string code = "+++++[>++++++<-]>++[>+>+>++>+++<<<<-]>------[->.>+.>+.<<<]";

            WhenProcess(code).ShouldBe(" Aa Bb Cc Dd Ee Ff Gg Hh Ii Jj Kk Ll Mm Nn Oo Pp Qq Rr Ss Tt Uu Vv Ww Xx Yy Zz");
        }

        [Test]
        public void FibonacciTest()
        {
            const string code = @">++++[-<+++++++++++>]
>,[>++++++[-<-------->]>+++++++++[-<<<[->+>+<<]>>[-<<+>>]>]<<[-<+>],]
<<+++++.-----.+++++.-----
>-->+>+
<<
[-
  >>[->+>+<<]
  <[->>>+<<<]
  >>[-<<+>>]
  >[-<<+>>>+<]
  <<<<<.
  >>>>>>[>>>>++++++++++<<<<[->+>>+>-[<-]<[->>+<<<<[->>>+<<<]>]<<]>+[-<+>]>>>[-]>[-<<<<+>>>>]<<<<]<[>++++++[<++++++++>-]<-.[-]<]<<<<]";
            maxInstructions = 100000; // This one never ends

            WhenProcess(code).ShouldBe("1,1,2,3,5,8,13,21,34,55,89,144,233,121,98,219,61,24,85,109,");
        }

        //[Test]
        public void SlowLoopsTest()
        {
            const string code = @"+>+>,-[-[->+<]<<[->>>>+>+<<<<<]>[->>>>>+>+<<<<<<]>>>[-<<<<+>>>>]>[-<<<+>>>]>[-<<<<<+>>>>>]>[-<<<<<+>>>>>]<<<<]";
            maxInstructions = 10000000;

            WhenProcess(code).ShouldBe(string.Empty);
        }
    }
}
