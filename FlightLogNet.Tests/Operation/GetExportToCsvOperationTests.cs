namespace FlightLogNet.Tests.Operation
{
    using System;
    using System.Text;
    using System.Linq;

    using FlightLogNet.Operation;

    using Xunit;

    using Microsoft.Extensions.Configuration;

    public class GetExportToCsvOperationTests
    {
        private readonly GetExportToCsvOperation getExportToCsvOperation;
        private readonly IConfiguration configuration;

        public GetExportToCsvOperationTests(GetExportToCsvOperation getExportToCsvOperation, IConfiguration configuration)
        {
            this.getExportToCsvOperation = getExportToCsvOperation;
            this.configuration = configuration;
        }

        // TODO 6.1: Odstra�t� skip a doplnt� test, aby otestoval vr�cen� CSV soubor.
        [Fact]
        public void Execute_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            TestDatabaseGenerator.DeleteOldDatabase(this.configuration);
            DateTime fixedDate = new DateTime(2020, 1, 2, 16, 57, 10);
            TestDatabaseGenerator.CreateTestDatabaseWithFixedTime(fixedDate, this.configuration);
            // ...

            // Act
            var result = getExportToCsvOperation.Execute();

            byte[] expectedCsv = ExpectedResult.export;
            expectedCsv = expectedCsv.Skip(3).ToArray();
            // Assert
            Assert.Equal(expectedCsv, result);
        }
    }
}