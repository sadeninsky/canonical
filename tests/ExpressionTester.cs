using System.Collections;
using canonical;
using NUnit.Framework;

namespace tests
{
    public class ExpressionTester
    {
        [Test]
        [TestCaseSource(typeof(ExpressionTesterCases), nameof(ExpressionTesterCases.SimplifyCases))]
        public void SimplifyTest(string input, string expResult)
        {
            var expression = Expression.Parse(input).Simplify();
            Assert.That(expression.ToString(), Is.EqualTo(expResult));
        }

        private class ExpressionTesterCases
        {
            public static IEnumerable SimplifyCases
            {
                get
                {
                    yield return new TestCaseData("2x + 3y^2 + 4z^3 = x + 2y^2 + 3z^3", "z^3 + y^2 + x = 0");
                    yield return new TestCaseData("2x + 3y^2 + 4z^3 = 4z^3 + 3y^2 + 2x", "0");
                    yield return new TestCaseData("2(2x + 3y^2 + 4z^3) = 2x + 3y^2 + 4z^3", "4z^3 + 3y^2 + 2x = 0");
                }
            }
        }
    }
}