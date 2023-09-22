using System.Diagnostics;
using System.Numerics;
using CGLibrary;

namespace Tests.ToolsTests;

public class ToolsTests<TNum, TConv> : Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Rotates the given swarm of points by given unitary matrix.
  /// </summary>
  /// <param name="S">The swarm of points to rotate.</param>
  /// <param name="rotation">Matrix to rotate a swarm.</param>
  /// <returns>The rotated swarm of points.</returns>
  public static List<Point> Rotate(IEnumerable<Point> S, Matrix rotation) {
    Debug.Assert
      (S.First().Dim == rotation.Rows, "ToolsTests.Rotate: the dimension of points must be equal to the count of rotation rows.");

    IEnumerable<Vector> rotated = S.Select(s => new Vector(s) * rotation);

    return rotated.Select(v => new Point(v)).ToList();
  }

  /// <summary>
  /// Generates a rotation matrix for a given dimension, axes, and angle.
  /// </summary>
  /// <param name="dim">The dimension of the matrix.</param>
  /// <param name="ax1">The first axis for the rotation [1,2, ... d].</param>
  /// <param name="ax2">The second axis for the rotation [1,2, ... d].</param>
  /// <param name="angle">The angle of rotation in radians.</param>
  /// <returns>A rotation matrix of type Matrix.</returns>
  public static Matrix GenRotationMatrix(int dim, int ax1, int ax2, TNum angle) {
#if DEBUG
    Debug.Assert(dim > 1, "ToolsTest.GenRotationMatrix: the dimension must be greater than 1.");
    Debug.Assert(ax1 > 0 && ax1 < dim + 1, "ToolsTest.GenRotationMatrix: the rotation axis must be one of [1,2, ..., d].");
    Debug.Assert(ax2 > 0 && ax2 < dim + 1, "ToolsTest.GenRotationMatrix: the rotation axis must be one of [1,2, ..., d].");
#endif

    ax1--; //чтобы к индексам привести
    ax2--; //чтобы к индексам привести
    TNum[,] rotM = Matrix.Eye(dim);
    for (int r = 0; r < dim; r++) {
      for (int k = 0; k < dim; k++) {
        if ((r == ax1 && k == ax1) || (r == ax2 && k == ax2)) { // индексы совпали
          rotM[r, k] = TNum.Cos(angle);
        }
        if (r == ax1 && k == ax2) { // у первого синуса минус
          rotM[r, k] = -TNum.Sin(angle);
        }
        if (r == ax2 && k == ax1) {
          rotM[r, k] = TNum.Sin(angle);
        }
      }
    }

    return new Matrix(rotM);
  }

}
