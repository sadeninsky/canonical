using System.Collections;
using System.Collections.Generic;
using System.Linq;
using canonical.Parsers;
using NUnit.Framework;

namespace tests
{
    public class EquationParserTester
    {
        [Test]
        [TestCaseSource(typeof(EquationParserTesterCases), nameof(EquationParserTesterCases.IsThisEquationTrueCases))]
        public void ThisIsEquationTest(string input, string left, string right)
        {
            string actualLeft, actualRight;
            var isThisEquation = EquationParser.IsThisEquation(input, out actualLeft, out actualRight);
            Assert.That(isThisEquation, Is.True);
            Assert.That(actualLeft, Is.EqualTo(left));
            Assert.That(actualRight, Is.EqualTo(right));
        }

        [Test]
        [TestCaseSource(typeof(EquationParserTesterCases), nameof(EquationParserTesterCases.IsThisEquationFalseCases))]
        public void ThisIsNotEquationTest(string input)
        {
            Assert.That(EquationParser.IsThisEquation(input), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(EquationParserTesterCases), nameof(EquationParserTesterCases.ParseTrueCases))]
        public void ParseTest(string input, IEnumerable<string> expectedStringOperands)
        {
            var operands = EquationParser.Parse(input);
            Assert.That(operands.Select(x => x.ToString()), Is.EqualTo(expectedStringOperands));
        }

        private class EquationParserTesterCases
        {
            public static IEnumerable IsThisEquationFalseCases
            {
                get
                {
                    yield return new TestCaseData(null);
                    yield return new TestCaseData("");
                    yield return new TestCaseData("  ");
                    yield return new TestCaseData("a");
                    yield return new TestCaseData("aa");
                    yield return new TestCaseData("  aa  ");
                    yield return new TestCaseData("=a");
                    yield return new TestCaseData("a=");
                    yield return new TestCaseData("=a=");
                    yield return new TestCaseData(" a = b = ");
                    yield return new TestCaseData(" abc= = xyz ");
                }
            }

            public static IEnumerable IsThisEquationTrueCases
            {
                get
                {
                    yield return new TestCaseData("a=b", "a", "b");
                    yield return new TestCaseData(" a = b ", "a", "b");
                    yield return new TestCaseData("abc=bcd", "abc", "bcd");
                    yield return new TestCaseData("  abc =  bcd ", "abc", "bcd");
                    yield return new TestCaseData("  abc xyz = xyz abc  ", "abc xyz", "xyz abc");
                }
            }

            public static IEnumerable ParseTrueCases
            {
                get
                {
                    yield return new TestCaseData("x^2 + 3.5y + y = x^2 + 3.5y + y", new[] {"x^2", "3.5y", "y", "-x^2", "-3.5y", "-y"});
                    yield return new TestCaseData("2(x^2 + 3.5y + y) = 2(x^2 + 3.5y + y)", new[] {"2x^2", "7y", "2y", "-2x^2", "-7y", "-2y"});
                    yield return new TestCaseData("-3(x^2 + 3.5y + y) = -3(x^2 + 3.5y + y)", new[] {"-3x^2", "-10.5y", "-3y", "3x^2", "10.5y", "3y"});
                    yield return new TestCaseData("-3(x^2 - 3.5y - y) = -3(x^2 - 3.5y - y)", new[] {"-3x^2", "10.5y", "3y", "3x^2", "-10.5y", "-3y"});
                    yield return new TestCaseData("-4(x^2 - 3.5y - y + 5xy) = -4(x^2 - 3.5y - y + 5xy)",
                        new[] {"-4x^2", "14y", "4y", "-20xy", "4x^2", "-14y", "-4y", "20xy"});
                    yield return new TestCaseData("x^2 - 3.5y - y = x^2 - 3.5y - y", new[] {"x^2", "-3.5y", "-y", "-x^2", "3.5y", "y"});
                    yield return new TestCaseData("-4(x^2 - 4y - 8y + 16xy)/2 = -4(x^2 - 4y - 8y + 16xy)/2",
                        new[] {"-2x^2", "8y", "16y", "-32xy", "2x^2", "-8y", "-16y", "32xy"});
                }
            }
        }
    }
}