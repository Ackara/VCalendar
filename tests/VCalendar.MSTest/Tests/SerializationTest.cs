using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Collections.Generic;
using System.IO;

namespace Acklann.VCalendar.Tests
{
    [TestClass]
    public class SerializationTest
    {
        [DataTestMethod]
        [DynamicData(nameof(GetCalendarFiles), DynamicDataSourceType.Method)]
        public void Can_read_ics_file(string filePath)
        {
            // Act
            var calendar = VCalendar.Calendar.ReadFile(filePath);

            // Assert
            calendar.ShouldNotBeNull();
            calendar.Events.ShouldNotBeEmpty();
            calendar.Events.ShouldAllBe(x => !string.IsNullOrEmpty(x.UID));
            calendar.Events.ShouldAllBe(x => !string.IsNullOrEmpty(x.Summary));
            calendar.Events.ShouldAllBe(x => x.DTStart != default);
            calendar.Events.ShouldAllBe(x => x.DTEnd != default);
        }

        [TestMethod]
        public void Can_save_ics_file()
        {
            // Arrange
            string filePath = TestData.GetFilePath("*.ics");
            string outFile = Path.Combine(Path.GetTempPath(), $"testcalendar.ics");
            int oldCount = 0, newCount = 0;

            // Act
            var calendar = VCalendar.Calendar.ReadFile(filePath);
            oldCount = calendar.Events.Count;
            calendar.SaveTo(outFile);
            calendar = VCalendar.Calendar.ReadFile(outFile);
            newCount = calendar.Events.Count;

            // Assert
            calendar.ShouldNotBeNull();
            oldCount.ShouldBe(newCount);
            calendar.Events.ShouldNotBeEmpty();
            calendar.Events.ShouldAllBe(x => !string.IsNullOrEmpty(x.UID));
            calendar.Events.ShouldAllBe(x => !string.IsNullOrEmpty(x.Summary));
            calendar.Events.ShouldAllBe(x => x.DTStart != default);
            calendar.Events.ShouldAllBe(x => x.DTEnd != default);
        }

        #region Backing Members

        private static IEnumerable<object[]> GetCalendarFiles()
        {
            foreach (string filePath in TestData.GetFilePaths("*.ics"))
            {
                yield return new object[] { filePath };
            }
        }

        #endregion Backing Members
    }
}