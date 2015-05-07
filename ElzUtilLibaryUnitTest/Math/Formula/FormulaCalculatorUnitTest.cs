using ElzUtilLibary.Math.Formula;
using ElzUtilLibary.Math.Formula.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElzUtilLibaryUnitTest.Math.Formula
{
    /// <summary>
    ///     Summary description for FormulaCalculatorUnitTest
    /// </summary>
    [TestClass]
    public class FormulaCalculatorUnitTest
    {
        private static FormulaCalculator _formulaCalculator;

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            _formulaCalculator = new FormulaCalculator();
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup()
        {
            _formulaCalculator.Clean();
        }

        #endregion

        [TestMethod]
        public void TestFormula1()
        {
            // => 49
            const string formula1 = "(1+2*3*(4+5))-6";

            _formulaCalculator.Formula = formula1;
            _formulaCalculator.Parse();

            Assert.IsFalse(_formulaCalculator.HasParameters);

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 49.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula2()
        {
            // if x = 5
            // => 7
            const string formula2 = "1*2+x";

            _formulaCalculator.Formula = formula2;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 1);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("x"));

            _formulaCalculator.Parameters["x"] = 5.0d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 7.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula3()
        {
            // if xy = 5
            // => 7
            const string formula3 = "1*2+xy";

            _formulaCalculator.Formula = formula3;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 1);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("xy"));

            _formulaCalculator.Parameters["xy"] = 5.0d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 7.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula4()
        {
            // if test = 5
            // => 7
            const string formula4 = "1*2+test";

            _formulaCalculator.Formula = formula4;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 1);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("test"));

            _formulaCalculator.Parameters["test"] = 5.0d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 7.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula5()
        {
            // if x = 5
            //    y = 6   
            // => 32
            const string formula5 = "1*2+x*y";

            _formulaCalculator.Formula = formula5;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 2);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("x"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("y"));

            _formulaCalculator.Parameters["x"] = 5.0d;
            _formulaCalculator.Parameters["y"] = 6.0d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 32.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula6()
        {
            // if base = 200
            //    level = 2
            //    per = 0.15
            //    set = 50
            // => 280
            const string formula6 = "base * (1.00 + (level-1) * per) + (set * (level-1))";

            _formulaCalculator.Formula = formula6;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 4);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("base"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("level"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("per"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("set"));

            _formulaCalculator.Parameters["base"] = 200.0d;
            _formulaCalculator.Parameters["level"] = 2.0d;
            _formulaCalculator.Parameters["per"] = 0.15d;
            _formulaCalculator.Parameters["set"] = 50.0d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 280.0d) < 0.00001);
        }

        [TestMethod]
        public void TestFormula7()
        {
            // if a.atk = 20
            //    b.def = 7.1
            // => 65.8
            const string formula7 = "a.atk * 4 - b.def * 2 ";

            _formulaCalculator.Formula = formula7;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 2);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("a.atk"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("b.def"));

            _formulaCalculator.Parameters["a.atk"] = 20.0d;
            _formulaCalculator.Parameters["b.def"] = 7.1d;

            _formulaCalculator.Calculate();

            Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 65.8d) < 0.00001);
        }

        //TODO: implement support for 'x^y'
        //[TestMethod]
        //public void TestFormula8()
        //{
        //    // if x = 5    
        //    // => 127
        //    const string formula7 = "1*2+x^3";

        //    _formulaCalculator.Formula = formula7;
        //    _formulaCalculator.Parse();

        //    Assert.IsTrue(_formulaCalculator.HasParameters);

        //    Assert.IsTrue(_formulaCalculator.Parameters.Count == 1);
        //    Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("x"));

        //    _formulaCalculator.Parameters["x"] = 5.0d;

        //    _formulaCalculator.Calculate();

        //    Assert.IsTrue(System.Math.Abs(_formulaCalculator.Result - 127.0d) < 0.00001);
        //}

        [TestMethod]
        public void TestFormulaParametersNotSetException()
        {
            // => throw Exception
            const string formula5 = "1*2+x*y";

            _formulaCalculator.Formula = formula5;
            _formulaCalculator.Parse();

            Assert.IsTrue(_formulaCalculator.HasParameters);

            Assert.IsTrue(_formulaCalculator.Parameters.Count == 2);
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("x"));
            Assert.IsTrue(_formulaCalculator.Parameters.ContainsKey("y"));
            
            try
            {
                _formulaCalculator.Calculate();

                Assert.Fail();
            }
            catch (FormulaParametersNotSetException e)
            {
                Assert.IsTrue(e.Message == "Parameters aren't all set!");
            }
        }

        [TestMethod]
        public void TestFormulaNotSetException()
        {
            // => throw Exception
            
            try
            {
                _formulaCalculator.Parse();

                Assert.Fail();
            }
            catch (FormulaNotSetException e)
            {
                Assert.IsTrue(e.Message == "Formula is null or empty!");
            }
        }
    }
}