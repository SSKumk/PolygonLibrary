using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GaussSLETests {

  [Test]
  public void Solve_Square_2x2_SimpleUniqueSolution() {
    double[,] A = { { 1, 1 }, { 1, -1 } };
    double[] b = { 3, 1 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A unique solution should be found.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_Square_3x3_UniqueSolution() {
    double[,] A = { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
    double[] b = { 8, -11, -3 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A unique solution should be found.");
    AssertArraysAreEqual(new double[] { 2, 3, -1 }, result);
  }

  [Test]
  public void Solve_Square_SingularMatrix_NoUniqueSolution() {
    double[,] A = { { 1, 1 }, { 2, 2 } };
    double[] b = { 2, 5 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "A singular matrix should not have a unique solution.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_Overdetermined_3x2_Consistent_UniqueSolution() {
    double[,] A = { { 1, 1 }, { 1, -1 }, { 2, 0 } };
    double[] b = { 3, 1, 4 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A consistent overdetermined system should have a unique solution.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_Overdetermined_3x2_Inconsistent_NoSolution() {
    double[,] A = { { 1, 1 }, { 1, -1 }, { 2, 0 } };
    double[] b = { 3, 1, 5 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "An inconsistent overdetermined system should not have a solution.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_Underdetermined_2x3_NoUniqueSolution() {
    double[,] A = { { 1, 1, 1 }, { 2, 3, 4 } };
    double[] b = { 6, 20 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "An underdetermined system cannot have a unique solution.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_PivotChoice_NoChoice_FailsOnZeroDiagonal() {
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[] b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.No, out double[]? result);

    Assert.That(success, Is.False, "Without pivoting, a zero on the diagonal should cause failure.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_PivotChoice_AllChoice_SucceedsOnZeroDiagonal() {
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[] b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "With pivoting, a zero on the diagonal should be handled correctly.");
    AssertArraysAreEqual(new double[] { 1, 1 }, result);
  }

  [Test]
  public void Solve_PivotChoice_RowWise_SucceedsBySwappingRows() {
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[] b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.RowWise, out double[]? result);

    Assert.That(success, Is.True, "Row-wise pivoting should swap rows and recover the solution.");
    AssertArraysAreEqual(new double[] { 1, 1 }, result);
  }

  [Test]
  public void Solve_PivotChoice_ColWise_SucceedsBySwappingColumnsAndRestoresVariableOrder() {
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[] b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.ColWise, out double[]? result);

    Assert.That(success, Is.True, "Column-wise pivoting should swap columns and still return variables in the original order.");
    AssertArraysAreEqual(new double[] { 1, 1 }, result);
  }

  [Test]
  public void InstanceApi_CanBeReusedAndReturnsVectorSolution() {
    GaussSLE solver = new GaussSLE(2, 2);
    solver.SetSystem((i, j) => j == 0 ? new[] { 0.0, 1.0 }[i] : 1.0, i => new[] { 1.0, 2.0 }[i], 2, 2, GaussSLE.GaussChoice.No);
    solver.Solve();

    bool firstSuccess = solver.GetSolution(out double[]? firstResult);

    solver.SetGaussChoice(GaussSLE.GaussChoice.RowWise);
    solver.Solve();
    bool secondSuccess = solver.GetSolution(out Vector? secondResult);

    solver.SetSystem(
      (i, j) => new[,] { { 2.0, 1.0 }, { 1.0, -1.0 } }[i, j],
      i => new[] { 5.0, 1.0 }[i],
      2,
      2,
      GaussSLE.GaussChoice.All
    );
    solver.Solve();
    bool thirdSuccess = solver.GetSolution(out double[]? thirdResult);

    Assert.Multiple(() => {
      Assert.That(firstSuccess, Is.False);
      Assert.That(firstResult, Is.Null);
      Assert.That(secondSuccess, Is.True);
      Assert.That(secondResult, Is.Not.Null);
      AssertVectorsAreEqual(secondResult!, V(1, 1));
      Assert.That(thirdSuccess, Is.True);
      AssertArraysAreEqual(new double[] { 2, 1 }, thirdResult);
    });
  }

  [Test]
  public void Solve_FactoryWithFunctions_FindsSolution() {
    double[,] A_data = { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
    double[] b_data = { 8, -11, -3 };
    int rows = 3;
    int cols = 3;

    bool success = GaussSLE.Solve((i, j) => A_data[i, j], i => b_data[i], rows, cols, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True);
    AssertArraysAreEqual(new double[] { 2, 3, -1 }, result);
  }

  [Test]
  public void Solve_FactoryWithHyperplanes_FindsIntersection() {
    List<HyperPlane> hps =
      [
        new(V(1, 1), 3),
        new(V(1, -1), 1)
      ];

    bool success = GaussSLE.Solve(hps, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "Intersection of two lines should be found.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_FactoryWithHyperplanes_EmptyList_Fails() {
    List<HyperPlane> hps = [];

    bool success = GaussSLE.Solve(hps, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "Solving with an empty list of hyperplanes should fail.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_ArrayFactory_ThrowsWhenRowCountDoesNotMatchRightHandSide() {
    double[,] A = { { 1, 0 }, { 0, 1 } };
    double[] b = { 1 };

    Assert.That(
      () => GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? _),
      Throws.ArgumentException.With.Message.Contains("number of rows")
    );
  }

  [Test]
  public void Solve_ArrayFactory_DoesNotMutateInputs() {
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[] b = { 1, 2 };
    double[,] aCopy = (double[,])A.Clone();
    double[] bCopy = (double[])b.Clone();

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.Multiple(() => {
      Assert.That(success, Is.True);
      AssertArraysAreEqual(new double[] { 1, 1 }, result);
      Assert.That(A, Is.EqualTo(aCopy));
      Assert.That(b, Is.EqualTo(bCopy));
    });
  }

}
