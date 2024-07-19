using System.Diagnostics;
using System.Numerics;
using CGLibrary;

namespace Tests.ToolsTests;

public class TestsBase<TNum, TConv> : Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The random engine.
  /// </summary>
  public static readonly GRandomLC _random = new GRandomLC(0);

#region Auxiliary functions
  /// <summary>
  /// Generates a random TNum value in (0,1): a value between 0 and 1, excluding the values 0 and 1.
  /// More accurate [1, 999] / 1000 
  /// </summary>
  /// <returns>The generated random TNum value.</returns>
  public static TNum GenInner(GRandomLC rnd) { return rnd.NextFromInt(1, 999) / TConv.FromDouble(1000.0); }


  /// <summary>
  /// Rotates the given swarm by an arbitrary non-degenerate matrix.
  /// </summary>
  /// <param name="S">The swarm to be rotated.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  /// <returns>A rotated swarm.</returns>
  public static List<Vector> RotateRND(List<Vector> S, GRandomLC? rnd = null)
    => Rotate(S, Matrix.GenONMatrix(S.First().Dim, rnd));

  /// <summary>
  /// Generates a linear combination of the given points.
  /// </summary>
  /// <param name="points">The list of point to lin-combine.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A linear combination of the given points.</returns>
  public static Vector GenConvexCombination(IReadOnlyCollection<Vector> points, GRandomLC? random = null) {
    GRandomLC  rnd = random ?? _random;
    List<TNum> ws  = new List<TNum>();

    TNum difA = Tools.One;
    for (int i = 0; i < points.Count - 1; i++) {
      TNum alpha = GenInner(rnd) * difA;
      ws.Add(alpha);
      difA -= alpha;
    }
    ws.Add(difA);

    Vector res = Vector.LinearCombination(points, ws);

    return res;
  }

  /// <summary>
  /// Generates a random non-zero vector, each coordinate [-50, 50].
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A random non-zero vector.</returns>
  public static Vector GenShift(int dim, GRandomLC? random = null)
    => Vector.GenVector(dim, TConv.FromInt(1), TConv.FromInt(10), random);

  ///<summary>
  /// Method applies a rotation and a shift to two lists of points.
  ///</summary>
  ///<param name="PDim">The dimension of the space in which the points exist.</param>
  ///<param name="P">A reference to the list of points to be transformed (answer).</param>
  ///<param name="S">A reference to the list of points representing the swarm to be transformed.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  public static void ShiftAndRotate(int PDim, ref List<Vector> P, ref List<Vector> S, GRandomLC? rnd = null) {
    GRandomLC random = rnd ?? _random;


    Matrix rotation = Matrix.GenONMatrix(PDim, random);
    Vector shift    = Vector.GenVector(PDim, TConv.FromInt(1), TConv.FromInt(10), random);

    P = Rotate(P, rotation);
    P = Shift(P, shift);

    S = Rotate(S, rotation);
    S = Shift(S, shift);
  }

  /// <summary>
  /// Rotates the given swarm of points by given unitary matrix.
  /// </summary>
  /// <param name="S">The swarm of points to rotate.</param>
  /// <param name="rotation">Matrix to rotate a swarm.</param>
  /// <returns>The rotated swarm of points.</returns>
  public static List<Vector> Rotate(IEnumerable<Vector> S, Matrix rotation) {
    Debug.Assert
      (
       S.First().Dim == rotation.Rows
     , "ToolsForTests.Rotate: the dimension of points must be equal to the count of rotation rows."
      );

    IEnumerable<Vector> rotated = S.Select(s => s * rotation);

    return rotated.Select(v => v).ToList();
  }

  /// <summary>
  /// Generates a rotation matrix for a given dimension, axes, and angle.
  /// </summary>
  /// <param name="dim">The dimension of the matrix.</param>
  /// <param name="ax1">The first axis for the rotation [1,2, ... d].</param>
  /// <param name="ax2">The second axis for the rotation [1,2, ... d].</param>
  /// <param name="angle">The angle of rotation in radians.</param>
  /// <returns>A rotation matrix of type Matrix.</returns>
  public static Matrix MakeRotationMatrix(int dim, int ax1, int ax2, TNum angle) {
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
#endregion

}
