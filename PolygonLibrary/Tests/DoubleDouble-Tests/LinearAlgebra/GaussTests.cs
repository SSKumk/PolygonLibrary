using CGLibrary;
using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests;

[TestFixture]
public class GaussTests {

  [Test]
  public void SimplestTest() {
    ddouble[,] A              = { { 1, 0 }, { 0, 1 } };
    ddouble[]  b              = { 2, 3 };
    ddouble[]  expectedResult = { 2, 3 }; // ожидаемый результат

    Check(A, b, expectedResult);
  }

  [Test]
  public void SomeTest() {
    ddouble[,] A              = new ddouble[,] { { 10, 6, 2, 0 }, { 5, 1, -2, 4 }, { 3, 5, 1, -1 }, { 0, 6, -2, 2 } };
    ddouble[]  b              = new ddouble[] { 25, 14, 10, 8 };
    ddouble[]  expectedResult = { 2, 1, -0.5, 0.5 }; // ожидаемый результат

    Check(A, b, expectedResult);
  }

  [Test]
  public void HundredONSystem() {
    Matrix    A_             = GenONMatrix(100);
    Vector    exRes          = GenVector(100); // x*
    ddouble[] expectedResult = (ddouble[])exRes;

    ddouble[,] A = A_;
    ddouble[]  b = (ddouble[])(A_ * exRes); // b = Ax*

    Check(A, b, expectedResult);
  }

  [Test]
  public void SingularSystem1() {
    ddouble[,] A = new ddouble[,] { { 10, 6, 2, 0 }, { 3, 5, 1, -1 }, { 6, 10, 2, -2 }, { 5, 1, -2, 4 } };
    ddouble[]  b = new ddouble[] { 25, 10, 20, 14 };
    ddouble[]  expectedResult = { };

    Check(A, b, expectedResult, true);
  }
  [Test]
  public void SingularSystem2() {
    ddouble[,] A = new ddouble[,] { { 3, 5, 1, -1 }, { 6, 10, 2, -2 }, { 6, 10, 2, -2 }, { 5, 1, -2, 4 } };
    ddouble[]  b = new ddouble[] {  10, 20, 20, 14 };
    ddouble[]  expectedResult = { };

    Check(A, b, expectedResult, true);
  }

  private static void Check(ddouble[,] A, ddouble[] b, ddouble[] expectedResult, bool isSingular = false) {
    Assert.Multiple
      (
       () => {
         bool successNo  = GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.No, out ddouble[] resNo);
         bool successRow = GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.RowWise, out ddouble[] resRow);
         bool successCol = GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] resCol);
         bool successAll = GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] resAll);

         if (isSingular) {
           Assert.That(successNo, Is.EqualTo(false),  "GaussSLE.SolveImmutable should return true for GaussChoice.No");
           Assert.That(successRow, Is.EqualTo(false), "GaussSLE.SolveImmutable should return true for GaussChoice.RowWise");
           Assert.That(successCol, Is.EqualTo(false), "GaussSLE.SolveImmutable should return true for GaussChoice.ColWise");
           Assert.That(successAll, Is.EqualTo(false), "GaussSLE.SolveImmutable should return true for GaussChoice.ColWise");
         } else {
           Assert.That(successNo, "GaussSLE.SolveImmutable should return true for GaussChoice.No");
           Assert.That(successRow, "GaussSLE.SolveImmutable should return true for GaussChoice.RowWise");
           Assert.That(successCol, "GaussSLE.SolveImmutable should return true for GaussChoice.ColWise");
           Assert.That(successAll, "GaussSLE.SolveImmutable should return true for GaussChoice.ColWise");

           for (int i = 0; i < b.Length; i++) {
             Assert.That
               (Tools.EQ(resNo[i], expectedResult[i]), $"resNo[{i}]={resNo[i]} =/=   {expectedResult[i]}=expectedResult[{i}]");
             Assert.That
               (Tools.EQ(resRow[i], expectedResult[i]), $"resRow[{i}]={resNo[i]} =/= {expectedResult[i]}=expectedResult[{i}]");
             Assert.That
               (Tools.EQ(resCol[i], expectedResult[i]), $"resCol[{i}]={resNo[i]} =/= {expectedResult[i]}=expectedResult[{i}]");
             Assert.That
               (Tools.EQ(resAll[i], expectedResult[i]), $"resAll[{i}]={resNo[i]} =/= {expectedResult[i]}=expectedResult[{i}]");
           }
         }
       }
      );
  }

}
