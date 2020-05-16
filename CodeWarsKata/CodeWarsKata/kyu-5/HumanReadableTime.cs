using NUnit.Framework;
using Shouldly;
using System.Text;

namespace CodeWarsKata.TimeFormat
{
    public static class TimeFormat
    {
        public static string GetReadableTime(int seconds)
        {
            if (seconds <= 0) return "00:00:00";
            if (seconds >= 360000) return "99:59:59";

            var secondsOnly = seconds % 60;
            var minutesOnly = (seconds / 60) % 60;
            var hoursOnly = seconds / 3600;

            var sb = new StringBuilder();
            sb.Append(hoursOnly.ToString("D2"));
            sb.Append(":");
            sb.Append(minutesOnly.ToString("D2"));
            sb.Append(":");
            sb.Append(secondsOnly.ToString("D2"));

            return sb.ToString();
        }
    }

    [TestFixture]
    public class HumanReadableTimeTest
    {
        [TestCase(-1, "00:00:00")]
        [TestCase(359999, "99:59:59")]
        [TestCase(360000, "99:59:59")]
        [TestCase(0, "00:00:00")]
        [TestCase(5, "00:00:05")]
        [TestCase(59, "00:00:59")]
        [TestCase(60, "00:01:00")]
        [TestCase(3599, "00:59:59")]
        [TestCase(3600, "01:00:00")]
        [TestCase(2 * 3600 + 46 * 60 + 39, "02:46:39")]
        public void NegativeTest(int seconds, string time)
        {
            TimeFormat.GetReadableTime(seconds).ShouldBe(time);
        }

        [Test]
        public void SampleTests()
        {
            Assert.AreEqual("00:00:00", TimeFormat.GetReadableTime(0));
            Assert.AreEqual("00:00:05", TimeFormat.GetReadableTime(5));
            Assert.AreEqual("00:01:00", TimeFormat.GetReadableTime(60));
            Assert.AreEqual("23:59:59", TimeFormat.GetReadableTime(86399));
            Assert.AreEqual("99:59:59", TimeFormat.GetReadableTime(359999));
        }
    }
}
