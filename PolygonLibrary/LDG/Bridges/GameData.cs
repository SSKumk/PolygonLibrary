using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using CGLibrary.Toolkit;

namespace LDG;

/// <summary>
/// Class for keeping game parameter data
/// </summary>
public class GameData<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Enums
  /// <summary>
  /// The type of the set.
  /// ConvexPolytop - The polytope in the format of the library
  /// RectAxisParallel - Rectangle-parallel: dim X Y -> dimension opposite_vertices
  /// Sphere - Sphere: dim x0 x1 .. xn theta phi R -> dimension center_coordinates theta_division phis_division radius
  /// Ellipsoid - Ellipsoid: dim x0 x1 .. xn theta phi a0 a1 ... an -> dimension center_coordinates theta_division phis_division semi-axis_length
  /// </summary>
  public enum SetType {

    /// <summary>
    /// Formed polytope
    /// </summary>
    ConvexPolytop

   ,

    /// <summary>
    /// Axis parallel Cube.
    /// </summary>
    RectParallel

   ,

    /// <summary>
    /// hD-Sphere.
    /// </summary>
    Sphere

   ,

    /// <summary>
    /// hD-Ellipsoid.
    /// </summary>
    Ellipsoid

   ,

    /// <summary>
    /// The convex hull of a set of hD-points
    /// </summary>
    ConvexHull

  }
#endregion

#region Input data
  //todo: xml
  public string Name;

#region Data defining the dynamics of the game
  /// <summary>
  /// Dimension of the phase vector
  /// </summary>
  public int n;

  /// <summary>
  /// The main matrix
  /// </summary>
  public Geometry<TNum, TConv>.Matrix A;

  /// <summary>
  /// Dimension of the useful control
  /// </summary>
  public readonly int pDim;

  /// <summary>
  /// The useful control matrix
  /// </summary>
  public Geometry<TNum, TConv>.Matrix B;

  /// <summary>
  /// Dimension of the disturbance
  /// </summary>
  public readonly int qDim;

  /// <summary>
  /// The disturbance matrix
  /// </summary>
  public Geometry<TNum, TConv>.Matrix C;

  /// <summary>
  /// The initial instant
  /// </summary>
  public readonly TNum t0;

  /// <summary>
  /// The final instant
  /// </summary>
  public readonly TNum T;

  /// <summary>
  /// The time step
  /// </summary>
  public readonly TNum dt;
#endregion

  public int    projDim;  // размерность выделенных m координат
  public int[]  projInd;  // индексы выделенных m координат
  public string projInfo; // текстовая характеристика пространства выделенных координат

#region Control constraints
  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the first player
  /// </summary>
  public Geometry<TNum, TConv>.ConvexPolytop P;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the second player
  /// </summary>
  public Geometry<TNum, TConv>.ConvexPolytop Q;
#endregion
#endregion

#region Matrices related to the system
  /// <summary>
  /// The fundamental Cauchy matrix of the corresponding system
  /// </summary>
  public Geometry<TNum, TConv>.CauchyMatrix CauchyMatrix;

  /// <summary>
  /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
  /// </summary>
  public Geometry<TNum, TConv>.Matrix ProjMatrix;

  public Geometry<TNum, TConv>.Matrix Xstar(TNum t) {
    if (_Xstar.TryGetValue(t, out Geometry<TNum, TConv>.Matrix? matrix)) {
      return matrix;
    }

    Geometry<TNum, TConv>.Matrix projMatrix = ProjMatrix * CauchyMatrix[t];
    _Xstar.Add(t, projMatrix);

    return projMatrix;
  }

  private readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> _Xstar =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);
#endregion

#region Constructor
  /// <summary>
  /// Initializes a new instance of the <see cref="GameData"/> class by reading and setting all required parameters.
  /// The order of reading is critical for correct initialization.
  /// </summary>
  /// <param name="prDyn">The reader for dynamic parameters.</param>
  /// <param name="prP">The reader for the first player's control parameters.</param>
  /// <param name="prQ">The reader for the second player's control parameters.</param>
  public GameData(
      Geometry<TNum, TConv>.ParamReader      prDyn
    , Geometry<TNum, TConv>.ParamReader      prP
    , Geometry<TNum, TConv>.ParamReader      prQ
    , TransformReader<TNum, TConv>.Transform trP
    , TransformReader<TNum, TConv>.Transform trQ
    ) {
    // Reading general information
    Name = prDyn.ReadString("Name");

    // Reading dynamics
    n    = prDyn.ReadNumber<int>("Dim");
    A    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("A", n, n));
    pDim = prDyn.ReadNumber<int>("PDim");
    B    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("B", n, pDim));
    qDim = prDyn.ReadNumber<int>("QDim");
    C    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("C", n, qDim));

    t0 = prDyn.ReadNumber<TNum>("t0");
    T  = prDyn.ReadNumber<TNum>("T");
    dt = prDyn.ReadNumber<TNum>("dt");

    // Reading projection information
    projDim = prDyn.ReadNumber<int>("ProjDim");
    projInd = prDyn.Read1DArray<int>("ProjInd", projDim);

    // Reading the first player's control data
    P = TransformReader<TNum, TConv>.DoTransform(PolytopeReader<TNum, TConv>.Read(prP), trP, pDim);


    // Reading the second player's control data
    Q = TransformReader<TNum, TConv>.DoTransform(PolytopeReader<TNum, TConv>.Read(prQ), trQ, qDim);

    // Computing the Cauchy matrix
    CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(A, T, dt);

    // Setting up the projection matrix
    TNum[,] projMatrixArr = new TNum[projDim, n];
    for (int i = 0; i < projDim; i++) {
      projMatrixArr[i, projInd[i]] = Geometry<TNum, TConv>.Tools.One;
    }
    ProjMatrix = new Geometry<TNum, TConv>.Matrix(projMatrixArr);
  }
#endregion

}
