using System.Collections;
using System.Collections.Generic;
using System.Linq;
using canonical;
using canonical.Parsers;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace tests
{
    public class ExpressionParserTester
    {
        private void TryParseOperand(string input, Constraint constraint)
        {
            Operand operand;
            string right;
            var thisIsOperand = ExpressionParser.TryParseOperand(input, out operand, out right);
            Assert.That(thisIsOperand, constraint);
        }

        private void TryParseOperation(string input, Constraint constraint)
        {
            Operation? operation;
            string right;
            var thisIsOperation = ExpressionParser.TryParseOperation(input, out operation, out right);
            Assert.That(thisIsOperation, constraint);
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.TryParseOperationFalseCases))]
        public void TryParseOperationFalseTest(string input)
        {
            TryParseOperation(input, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.TryParseOperationTrueCases))]
        public void TryParseOperationTrueTest(string input)
        {
            TryParseOperation(input, Is.True);
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.TryParseOperandFalseCases))]
        public void TryParseOperandFalseTest(string input)
        {
            TryParseOperand(input, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.TryParseOperandTrueCases))]
        public void TryParseOperandTrueTest(string input)
        {
            TryParseOperand(input, Is.True);
        }


        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.IsBracedExpressionFalseCases))]
        public void IsBracedExpressionFalseTest(string input)
        {
            decimal multiplier, divider;
            string innerExpression;
            var isBracedExpression = ExpressionParser.TryParseBracedExpression(input, out multiplier, out innerExpression, out divider);
            Assert.That(isBracedExpression, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.IsBracedExpressionTrueCases))]
        public void IsBracedExpressionTrueTest(string input, decimal expMultiplier, string expInnerExpression, decimal expDivider)
        {
            decimal multiplier, divider;
            string innerExpression;
            var isBracedExpression = ExpressionParser.TryParseBracedExpression(input, out multiplier, out innerExpression, out divider);
            Assert.That(isBracedExpression, Is.True);
            Assert.That(multiplier, Is.EqualTo(expMultiplier));
            Assert.That(divider, Is.EqualTo(expDivider));
            Assert.That(innerExpression, Is.EqualTo(expInnerExpression));
        }

        [Test]
        [TestCaseSource(typeof(ExpressionParserTesterCases), nameof(ExpressionParserTesterCases.ParseTrueCases))]
        public void ParseTest(string input, IEnumerable<string> expectedStringOperands)
        {
            var operands = ExpressionParser.Parse(input);
            Assert.That(operands.Select(x => x.ToString()), Is.EqualTo(expectedStringOperands));
        }

        private class ExpressionParserTesterCases
        {
            public static IEnumerable TryParseOperandFalseCases
            {
                get
                {
                    yield return new TestCaseData(null);
                    yield return new TestCaseData("");
                    yield return new TestCaseData("1");
                    yield return new TestCaseData("1.2.3");
                    yield return new TestCaseData("a2");
                    yield return new TestCaseData("^");
                    yield return new TestCaseData("^2");
                    yield return new TestCaseData("^2.3");
                    yield return new TestCaseData("^2a");
                    yield return new TestCaseData("^a2");
                    yield return new TestCaseData("+2x^3");
                    yield return new TestCaseData("2x^");
                    yield return new TestCaseData("2x^3y");
                    yield return new TestCaseData("2x^3.4");
                    yield return new TestCaseData("2x^3^");
                    yield return new TestCaseData("2x^3.");
                    yield return new TestCaseData("2x^y3");
                    yield return new TestCaseData("2xyx");
                }
            }

            public static IEnumerable TryParseOperandTrueCases
            {
                get
                {
                    yield return new TestCaseData("2xyz^45");
                    yield return new TestCaseData("-2xyz^45");
                    yield return new TestCaseData("2.3xyz^45");
                    yield return new TestCaseData("-2.3xyz^45");
                    yield return new TestCaseData("2.3xyz");
                    yield return new TestCaseData("-2.3xyz");
                    yield return new TestCaseData("xy^2");
                    yield return new TestCaseData("-xy^2");
                    yield return new TestCaseData("x");
                    yield return new TestCaseData("-x");
                }
            }

            public static IEnumerable TryParseOperationFalseCases
            {
                get
                {
                    yield return new TestCaseData(null);
                    yield return new TestCaseData("");
                    yield return new TestCaseData("  ");
                    yield return new TestCaseData("1+");
                    yield return new TestCaseData("/");
                }
            }

            public static IEnumerable TryParseOperationTrueCases
            {
                get
                {
                    yield return new TestCaseData("   +");
                    yield return new TestCaseData("   -");
                    yield return new TestCaseData("+q");
                    yield return new TestCaseData("-  q");
                }
            }

            public static IEnumerable IsBracedExpressionFalseCases
            {
                get
                {
                    yield return new TestCaseData(null);
                    yield return new TestCaseData("");
                    yield return new TestCaseData("  ");
                    yield return new TestCaseData(" (");
                    yield return new TestCaseData(" )");
                    yield return new TestCaseData("1(");
                    yield return new TestCaseData("1 (q)");
                    yield return new TestCaseData("1(q) 2");
                    yield return new TestCaseData("1(q) / 2");
                }
            }

            public static IEnumerable IsBracedExpressionTrueCases
            {
                get
                {
                    yield return new TestCaseData("   (a) ", 1m, "a", 1m);
                    yield return new TestCaseData("   2(q)", 2m, "q", 1m);
                    yield return new TestCaseData("2(a)/3", 2m, "a", 3m);
                    yield return new TestCaseData("2.3(a)", 2.3m, "a", 1m);
                    yield return new TestCaseData("2.3(a)/4", 2.3m, "a", 4m);
                    yield return new TestCaseData("-2.3(a)/4", -2.3m, "a", 4m);
                    yield return new TestCaseData("-2.3(a)/4.5", -2.3m, "a", 4.5m);
                    yield return new TestCaseData("-1(a)/4.5", -1m, "a", 4.5m);
                    yield return new TestCaseData("-1(a)", -1m, "a", 1m);
                    yield return new TestCaseData("-(a)/2.3", -1m, "a", 2.3m);
                    yield return new TestCaseData("2(x^2 + 3.5y + y)", 2m, "x^2 + 3.5y + y", 1m);
                }
            }

            public static IEnumerable ParseTrueCases
            {
                get
                {
                    yield return new TestCaseData("x^2 + 3.5y + y", new[] {"x^2", "3.5y", "y"});
                    yield return new TestCaseData("2(x^2 + 3.5y + y)", new[] {"2x^2", "7y", "2y"});
                    yield return new TestCaseData("-3(x^2 + 3.5y + y)", new[] {"-3x^2", "-10.5y", "-3y"});
                    yield return new TestCaseData("-3(x^2 - 3.5y - y)", new[] {"-3x^2", "10.5y", "3y"});
                    yield return new TestCaseData("-4(x^2 - 3.5y - y + 5xy)", new[] {"-4x^2", "14y", "4y", "-20xy"});
                    yield return new TestCaseData("x^2 - 3.5y - y", new[] {"x^2", "-3.5y", "-y"});
                    yield return new TestCaseData("-4(x^2 - 4y - 8y + 16xy)/2", new[] {"-2x^2", "8y", "16y", "-32xy"});
                }
            }
        }
    }
}