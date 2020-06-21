using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace CodeWarsKata.HumanTimeFormat
{
    public class HumanTimeFormat
    {
        private class TimeUnit
        {
            public int InSeconds { get; set; }
            public string SingleName { get; set; }
            public string PluralName { get; set; }
            public string GetName(int amount) => amount == 1 ? SingleName : PluralName;
        }

        public static string formatDuration(int seconds)
        {
            if (seconds < 0)
            {
                return string.Empty;
            }

            if (seconds == 0)
            {
                return "now";
            }

            var timeUnits = new List<TimeUnit>
            {
                new TimeUnit { InSeconds = 365*24*60*60, SingleName = "year", PluralName = "years" },
                new TimeUnit { InSeconds = 24*60*60, SingleName = "day", PluralName = "days" },
                new TimeUnit { InSeconds = 60*60, SingleName = "hour", PluralName = "hours" },
                new TimeUnit { InSeconds = 60, SingleName = "minute", PluralName = "minutes" },
                new TimeUnit { InSeconds = 1, SingleName = "second", PluralName = "seconds" }
            };

            var components = new List<(int, string)>();
            foreach (var timeUnit in timeUnits)
            {
                var units = seconds / timeUnit.InSeconds;
                if (units <= 0) continue;
                components.Add((units, timeUnit.GetName(units)));
                seconds -= units * timeUnit.InSeconds;
            }

            var sb = new StringBuilder();
            for(var n = 0; n < components.Count; n++)
            {
                if (n > 0)
                {
                    if (n < components.Count - 1)
                    {
                        sb.Append(", ");
                    }
                    else if (n < components.Count)
                    {
                        sb.Append(" and ");
                    }
                }

                sb.Append(components[n].Item1);
                sb.Append(" ");
                sb.Append(components[n].Item2);
            }

            return sb.ToString();
        }
    }

    [TestFixture]
    public class Tests
    {
        private static string Duration(int seconds)
        {
            return HumanTimeFormat.formatDuration(seconds);
        }

        [Test]
        public void NegativeShouldReturnEmptyString()
        {
            Duration(-1).Should().Be(string.Empty);
        }

        [Test]
        public void ZeroShouldReturnNow()
        {
            Duration(0).Should().Be("now");
        }

        [Test]
        public void CanGetSecondsOnly()
        {
            Duration(1).Should().Be("1 second");
            Duration(20).Should().Be("20 seconds");
        }

        [Test]
        public void CanGetMinutes()
        {
            Duration(65).Should().Be("1 minute and 5 seconds");
            Duration(222).Should().Be("3 minutes and 42 seconds");
            Duration(360).Should().Be("6 minutes");
        }

        [Test]
        public void CanGetHours()
        {
            Duration(3600).Should().Be("1 hour");
            Duration(10000).Should().Be("2 hours, 46 minutes and 40 seconds");
        }

        [Test]
        public void CanGetDays()
        {
            Duration(86400).Should().Be("1 day");
            Duration(100000).Should().Be("1 day, 3 hours, 46 minutes and 40 seconds");
        }

        [Test]
        public void CanGetYears()
        {
            Duration(31536000).Should().Be("1 year");
            Duration(100000000).Should().Be("3 years, 62 days, 9 hours, 46 minutes and 40 seconds");
        }

        [Test]
        public void SampleTests()
        {
            Duration(0).Should().Be("now");
            Duration(1).Should().Be("1 second");
            Duration(62).Should().Be("1 minute and 2 seconds");
            Duration(120).Should().Be("2 minutes");
            Duration(3662).Should().Be("1 hour, 1 minute and 2 seconds");
            Duration(15731080).Should().Be("182 days, 1 hour, 44 minutes and 40 seconds");
            Duration(132030240).Should().Be("4 years, 68 days, 3 hours and 4 minutes");
            Duration(205851834).Should().Be("6 years, 192 days, 13 hours, 3 minutes and 54 seconds");
            Duration(253374061).Should().Be("8 years, 12 days, 13 hours, 41 minutes and 1 second");
            Duration(242062374).Should().Be("7 years, 246 days, 15 hours, 32 minutes and 54 seconds");
            Duration(101956166).Should().Be("3 years, 85 days, 1 hour, 9 minutes and 26 seconds");
            Duration(33243586).Should().Be("1 year, 19 days, 18 hours, 19 minutes and 46 seconds");
        }
    }
}
