using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using CGLibrary.Toolkit;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class for keeping game parameter data
  /// </summary>
  public class GameData {

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
    public Matrix A;

    /// <summary>
    /// Dimension of the useful control
    /// </summary>
    public readonly int pDim;

    /// <summary>
    /// The useful control matrix
    /// </summary>
    public Matrix B;

    /// <summary>
    /// Dimension of the disturbance
    /// </summary>
    public readonly int qDim;

    /// <summary>
    /// The disturbance matrix
    /// </summary>
    public Matrix C;

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
    public ConvexPolytop P;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the second player
    /// </summary>
    public ConvexPolytop Q;


    // public readonly string DynamicPQInfo;
    // public readonly string PInfo;
    // public readonly string QInfo;
    //
    // public readonly string DynamicsHash;
    // public readonly string PHash;
    // public readonly string QHash;

    // calc hashes
    // string Astr = A.ToString();
    // string Bstr = B.ToString();
    // string Cstr = C.ToString();
    //
    // DynamicPQInfo = $"{Astr}{Bstr}{Cstr}{T}{dt}{PSetInfo}{QSetInfo}{projInfo}";
    // PInfo         = $"{Astr}{Bstr}{T}{dt}{PSetInfo}{projInfo}";
    // QInfo         = $"{Astr}{Cstr}{T}{dt}{QSetInfo}{projInfo}";

    // DynamicsHash = Hashes.GetMD5Hash(DynamicPQInfo);
    // PHash       = Hashes.GetMD5Hash(PInfo);
    // QHash       = Hashes.GetMD5Hash(QInfo);
#endregion
#endregion

#region Matrices related to the system
    /// <summary>
    /// The fundamental Cauchy matrix of the corresponding system
    /// </summary>
    public CauchyMatrix CauchyMatrix;

    /// <summary>
    /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
    /// </summary>
    public readonly Matrix ProjMatrix;

    public Matrix Xstar(TNum t) {
      if (_Xstar.TryGetValue(t, out Matrix? matrix)) {
        return matrix;
      }

      Matrix projMatrix = ProjMatrix * CauchyMatrix[t];
      _Xstar.Add(t, projMatrix);

      return projMatrix;
    }

    private readonly SortedDictionary<TNum, Matrix> _Xstar = new SortedDictionary<TNum, Matrix>(Tools.TComp);
#endregion

#region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GameData"/> class by reading and setting all required parameters.
    /// The order of reading is critical for correct initialization.
    /// </summary>
    /// <param name="prDyn">The reader for dynamic parameters.</param>
    /// <param name="prP">The reader for the first player's control parameters.</param>
    /// <param name="prQ">The reader for the second player's control parameters.</param>
    public GameData(ParamReader prDyn, ParamReader prP, ParamReader prQ) {
      // Reading general information
      Name = prDyn.ReadString("Name");

      // Reading dynamics
      n    = prDyn.ReadNumber<int>("Dim");
      A    = new Matrix(prDyn.Read2DArray<TNum>("A", n, n));
      pDim = prDyn.ReadNumber<int>("PDim");
      B    = new Matrix(prDyn.Read2DArray<TNum>("B", n, pDim));
      qDim = prDyn.ReadNumber<int>("QDim");
      C    = new Matrix(prDyn.Read2DArray<TNum>("C", n, qDim));

      t0 = prDyn.ReadNumber<TNum>("t0");
      T  = prDyn.ReadNumber<TNum>("T");
      dt = prDyn.ReadNumber<TNum>("dt");

      // Reading projection information
      projDim = prDyn.ReadNumber<int>("ProjDim");
      projInd = prDyn.Read1DArray<int>("ProjInd", projDim);

      // Reading the first player's control data
      P = PolytopeReader.Read(prP);

      // Reading the second player's control data
      Q = PolytopeReader.Read(prQ);

      // Computing the Cauchy matrix
      CauchyMatrix = new CauchyMatrix(A, T, dt);

      // Setting up the projection matrix
      TNum[,] projMatrixArr = new TNum[projDim, n];
      for (int i = 0; i < projDim; i++) {
        projMatrixArr[i, projInd[i]] = Tools.One;
      }
      ProjMatrix = new Matrix(projMatrixArr);
    }
#endregion

  }

}
