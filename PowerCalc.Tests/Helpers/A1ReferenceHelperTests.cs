using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAlex.PowerCalc.Helpers;
using FluentAssertions;


namespace TAlex.PowerCalc.Tests.Helpers
{
    public class A1ReferenceHelperTests
    {
        [TestCase(0, 0, "A1")]
        [TestCase(6, 3, "G4")]
        [TestCase(30, 86, "AE87")]
        public void ToStringTest(int column, int row, string expected)
        {
            //action
            string actual = A1ReferenceHelper.ToString(column, row);

            //assert
            actual.Should().Be(expected);
        }

        [TestCase(0, 0, 2, 2, "A1:C3")]
        public void ToString2Test(int column1, int row1, int column2, int row2, string expected)
        {
            //action
            string actual = A1ReferenceHelper.ToString(column1, row1, column2, row2);

            //assert
            actual.Should().Be(expected);
        }


        [TestCase("A1", 0, 0)]
        [TestCase("G4", 6, 3)]
        [TestCase("AE87", 30, 86)]
        public void ParseTest(string reference, int expectedColumn, int expectedRow)
        {
            //arrange
            int actualColumn, actualRow;

            //action
            A1ReferenceHelper.Parse(reference, out actualColumn, out actualRow);

            //assert
            actualColumn.Should().Be(expectedColumn);
            actualRow.Should().Be(expectedRow);
        }

        [TestCase("A1:C3", 0, 0, 2, 2)]
        public void Parse2Test(string reference, int expectedColumn1, int expectedRow1, int expectedColumn2, int expectedRow2)
        {
            //arrange
            int actualColumn1, actualRow1, actualColumn2, actualRow2;

            //action
            A1ReferenceHelper.Parse(reference, out actualColumn1, out actualRow1, out actualColumn2, out actualRow2);

            //assert
            actualColumn1.Should().Be(expectedColumn1);
            actualRow1.Should().Be(expectedRow1);
            actualColumn2.Should().Be(expectedColumn2);
            actualRow2.Should().Be(expectedRow2);
        }
    }
}
