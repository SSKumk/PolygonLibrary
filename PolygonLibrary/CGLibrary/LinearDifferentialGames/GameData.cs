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
    /// RectParallel - Rectangle-parallel: dim X Y -> dimension opposite_vertices
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

    public int projDim;     // размерность выделенных m координат
    public int[] projInd;   // индексы выделенных m координат
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
      pDim = prDyn.ReadNumber<int>("pDim");
      B    = new Matrix(prDyn.Read2DArray<TNum>("B", n, pDim));
      qDim = prDyn.ReadNumber<int>("qDim");
      C    = new Matrix(prDyn.Read2DArray<TNum>("C", n, qDim));

      t0 = prDyn.ReadNumber<TNum>("t0");
      T  = prDyn.ReadNumber<TNum>("T");
      dt = prDyn.ReadNumber<TNum>("dt");

      // Reading projection information
      projDim  = prDyn.ReadNumber<int>("ProjDim");
      projInd  = prDyn.Read1DArray<int>("ProjInd", projDim);
      projInfo = string.Join(';', projInd);

      // Reading the first player's control data
      P = PolytopeReader.Read(prP);

      // Reading the second player's control data
      Q = PolytopeReader.Read(prQ);

      // Computing the Cauchy matrix
      CauchyMatrix = new CauchyMatrix(A, T, dt);

      // Setting up the projection matrix
      TNum[,] ProjMatrixArr = new TNum[projDim, n];
      for (int i = 0; i < projDim; i++) {
        ProjMatrixArr[i, projInd[i]] = Tools.One;
      }
      ProjMatrix = new Matrix(ProjMatrixArr);
    }
#endregion

#region Aux procedures
    /// <summary>
    /// The function fills in the fields of the original sets
    /// </summary>
    /// <param name="pr">The reader used to gather data from a file.</param>
    /// <param name="player">P - first player. Q - second player. M - terminal set.</param>
    /// <param name="dim">The dimension of the set to be read.</param>
    /// <param name="setTypeInfo">The auxiliary information to write into task folder name.</param>
    public static ConvexPolytop ReadExplicitSet(ParamReader pr, char player, int dim, out string setTypeInfo) {
      setTypeInfo = pr.ReadString($"{player}SetType");
      SetType setType =
        setTypeInfo switch
          {
            "ConvexPolytope" => SetType.ConvexPolytop
          , "RectParallel"   => SetType.RectParallel
          , "Sphere"         => SetType.Sphere
          , "Ellipsoid"      => SetType.Ellipsoid
          , "ConvexHull"     => SetType.ConvexHull
          , _                => throw new ArgumentOutOfRangeException($"GameData.ReadExplicitSet: {setTypeInfo} is not supported for now.")
          };

      // Array for coordinates of the next point
      ConvexPolytop? res = null;
      switch (setType) {
        case SetType.ConvexPolytop: {
          res = ConvexPolytop.CreateFromReader(pr);

          setTypeInfo += res.WhichRepToString();
          // todo: какие ещё х-ки записать, для однозначной идентификации?

          break;
        }
        case SetType.RectParallel: {
          TNum[] left   = pr.Read1DArray<TNum>($"{player}RectPLeft", dim);
          TNum[] right  = pr.Read1DArray<TNum>($"{player}RectPRight", dim);
          Vector vLeft  = new Vector(left, false);
          Vector vRight = new Vector(right, false);
          res = ConvexPolytop.RectParallel(vLeft, vRight);

          setTypeInfo += $"-{vLeft}-{vRight}";

          break;
        }
        case SetType.Sphere: {
          int    theta  = pr.ReadNumber<int>($"{player}Theta");
          int    phi    = pr.ReadNumber<int>($"{player}Phi");
          TNum[] center = pr.Read1DArray<TNum>($"{player}Center", dim);
          TNum   radius = pr.ReadNumber<TNum>($"{player}Radius");
          res = ConvexPolytop.Sphere(new Vector(center, false), radius, theta, phi);

          setTypeInfo += $"-T{theta}-P{phi}-R{radius}";

          break;
        }
        case SetType.Ellipsoid: {
          int    theta          = pr.ReadNumber<int>($"{player}Theta");
          int    phi            = pr.ReadNumber<int>($"{player}Phi");
          TNum[] center         = pr.Read1DArray<TNum>($"{player}Center", dim);
          TNum[] semiaxesLength = pr.Read1DArray<TNum>($"{player}SemiaxesLength", dim);
          res = ConvexPolytop.Ellipsoid(theta, phi, new Vector(center, false), new Vector(semiaxesLength, false));

          setTypeInfo += $"-T{theta}-P{phi}-SA{string.Join(' ', semiaxesLength)}";

          break;
        }

        case SetType.ConvexHull: {
          int vsCount = pr.ReadNumber<int>("VsCount");
          // todo: Возможно стоит воспользоваться StringReader -ом, но надо запомнить последнюю позицию, после чего выставить в ParamReader -е.
          /*
           * using (StringReader reader = new StringReader(someString.Substring(startPosition)))
{
    string line;
    int currentPosition = startPosition;

    while ((line = reader.ReadLine()) != null)
    {
        Console.WriteLine($"Строка: {line}, Начальная позиция: {currentPosition}");
        currentPosition += line.Length + 1; // +1 для символа новой строки
    }
}
           */

          throw new NotImplementedException();

          break;
        }
      }

      return res ?? throw new InvalidOperationException($"GameData.ReadSets: Player {player} set is empty!");
    }
#endregion

  }

}
