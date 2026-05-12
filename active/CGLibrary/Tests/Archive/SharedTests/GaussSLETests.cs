using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>; // Используем double
using static Tests.SharedTests.StaticHelpers;              // Используем V() и Assert-хелперы
using System.Collections.Generic;

namespace Tests.SharedTests;

[TestFixture]
public class GaussSLE_Tests {

#region Square Systems
  [Test]
  public void Solve_Square_2x2_SimpleUniqueSolution() {
    // x + y = 3
    // x - y = 1
    // Solution: x = 2, y = 1
    double[,] A = { { 1, 1 }, { 1, -1 } };
    double[]  b = { 3, 1 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A unique solution should be found.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_Square_3x3_UniqueSolution() {
    // 2x +  y -  z = 8
    //-3x -  y + 2z = -11
    //-2x +  y + 2z = -3
    // Solution: x = 2, y = 3, z = -1
    double[,] A = { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
    double[]  b = { 8, -11, -3 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A unique solution should be found.");
    AssertArraysAreEqual(new double[] { 2, 3, -1 }, result);
  }

  [Test]
  public void Solve_Square_SingularMatrix_NoUniqueSolution() {
    // System with parallel lines (no unique solution)
    // x + y = 2
    // 2x + 2y = 5
    double[,] A = { { 1, 1 }, { 2, 2 } };
    double[]  b = { 2, 5 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "A singular matrix should not have a unique solution.");
    Assert.That(result, Is.Null);
  }
#endregion

#region Rectangular Systems
  [Test]
  public void Solve_Overdetermined_3x2_Consistent_UniqueSolution() {
    // More equations than variables, but they are consistent
    // x + y = 3
    // x - y = 1
    // 2x + 0y = 4  (redundant but consistent)
    // Solution: x = 2, y = 1
    double[,] A = { { 1, 1 }, { 1, -1 }, { 2, 0 } };
    double[]  b = { 3, 1, 4 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "A consistent overdetermined system should have a unique solution.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_Overdetermined_3x2_Inconsistent_NoSolution() {
    // More equations than variables, and they are inconsistent
    // x + y = 3
    // x - y = 1
    // 2x + 0y = 5  (this one is inconsistent with the first two)
    double[,] A = { { 1, 1 }, { 1, -1 }, { 2, 0 } };
    double[]  b = { 3, 1, 5 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "An inconsistent overdetermined system should not have a solution.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_Underdetermined_2x3_NoUniqueSolution() {
    // Fewer equations than variables, cannot have a unique solution
    // x + y + z = 6
    // 2x + 3y + 4z = 20
    double[,] A = { { 1, 1, 1 }, { 2, 3, 4 } };
    double[]  b = { 6, 20 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "An underdetermined system cannot have a unique solution.");
    Assert.That(result, Is.Null);
  }
#endregion

#region Pivot Selection
  [Test]
  public void Solve_PivotChoice_NoChoice_FailsOnZeroDiagonal() {
    // 0x + y = 1
    // x + y = 2
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[]  b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.No, out double[]? result);

    Assert.That(success, Is.False, "Without pivoting, a zero on the diagonal should cause failure.");
    Assert.That(result, Is.Null);
  }

  [Test]
  public void Solve_PivotChoice_AllChoice_SucceedsOnZeroDiagonal() {
    // 0x + y = 1
    // x + y = 2
    double[,] A = { { 0, 1 }, { 1, 1 } };
    double[]  b = { 1, 2 };

    bool success = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "With pivoting, a zero on the diagonal should be handled correctly.");
    AssertArraysAreEqual(new double[] { 1, 1 }, result);
  }
#endregion

#region Factory Methods
  [Test]
  public void Solve_FactoryWithFunctions_FindsSolution() {
    // Use the 3x3 system from the first test
    double[,] A_data = { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
    double[]  b_data = { 8, -11, -3 };
    int rows = 3;
    int cols = 3;

    bool success = GaussSLE.Solve((i, j) => A_data[i, j], i => b_data[i], rows, cols, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True);
    AssertArraysAreEqual(new double[] { 2, 3, -1 }, result);
  }

  [Test]
  public void Solve_FactoryWithHyperplanes_FindsIntersection() {
    // System:
    // x + y = 3
    // x - y = 1
    List<HyperPlane> hps = new List<HyperPlane>
      {
        new(V(1, 1), 3),
        new(V(1, -1), 1)
      };

    bool success = GaussSLE.Solve(hps, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.True, "Intersection of two lines should be found.");
    AssertArraysAreEqual(new double[] { 2, 1 }, result);
  }

  [Test]
  public void Solve_FactoryWithHyperplanes_EmptyList_Fails() {
    List<HyperPlane> hps = new List<HyperPlane>();

    bool success = GaussSLE.Solve(hps, GaussSLE.GaussChoice.All, out double[]? result);

    Assert.That(success, Is.False, "Solving with an empty list of hyperplanes should fail.");
    Assert.That(result, Is.Null);
  }
#endregion

}
