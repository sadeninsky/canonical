using System.Collections;
using canonical;
using canonical.Exceptions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace tests
{
    public class OperandTester
    {
        [Test]
        [TestCaseSource(typeof(OperandTesterCases), nameof(OperandTesterCases.CtorCases))]
        public void CtorTest(ActualValueDelegate<Operand> actualValueDelegate, string label)
        {
            Assert.That(actualValueDelegate, Throws.TypeOf<OperandException>());
        }

        [Test]
        [TestCaseSource(typeof(OperandTesterCases), nameof(OperandTesterCases.ToStringCases))]
        public string ToStringTest(Operand operand)
        {
            return operand.ToString();
        }

        private class OperandTesterCases
        {
            public static IEnumerable CtorCases
            {
                get
                {
                    yield return new TestCaseData((ActualValueDelegate<Operand>) (() => new Operand(null)), "1");
                    yield return new TestCaseData((ActualValueDelegate<Operand>) (() => new Operand(new char[] { })), "2");
                    yield return new TestCaseData((ActualValueDelegate<Operand>) (() => new Operand(new[] {'a', 'b', 'a'})), "3");
                    yield return new TestCaseData((ActualValueDelegate<Operand>) (() => new Operand(new[] {'a', 'b', 'C'})), "4");
                    yield return new TestCaseData((ActualValueDelegate<Operand>) (() => new Operand(new[] {'a', 'b', 'c'}, 0)), "5");
                }
            }

            public static IEnumerable ToStringCases
            {
                get
                {
                    yield return new TestCaseData(new Operand(new[] {'x'})).Returns("x");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y'})).Returns("xy");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y'}, 2)).Returns("2xy");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y'}, 2, 3)).Returns("2xy^3");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y'}, 2.3m, 4)).Returns("2.3xy^4");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y', 'z'}, 1, 4)).Returns("xyz^4");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y', 'z'}, -1, 4)).Returns("-xyz^4");
                    yield return new TestCaseData(new Operand(new[] {'x', 'y', 'z'}, -2)).Returns("-2xyz");
                }
            }
        }
    }
}