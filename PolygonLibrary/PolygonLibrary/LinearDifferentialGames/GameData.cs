using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using PolygonLibrary.Toolkit;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // todo: Продумать систему пространства имён; в смысле?
  // todo: Добавить в нашу библиотеку проект LDG2D. Для этого сделать пространство имён.
  // todo: Закинуть в Toolkit всё, что не геометрия

  /// <summary>
  /// Class for keeping game parameter data
  /// </summary>
  public class GameData {

#region Enums
    /// <summary>
    /// The type of the game
    /// </summary>
    public enum GoalType { Itself, PayoffEpigraph }


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

      // , DistanceToOrigin
      // , DistanceToPolytop
      // , DistanceToCube SimplexRND???

    }
#endregion

#region Input data
    /// <summary>
    /// The reader from file.
    /// </summary>
    private readonly ParamReader _pr;

    //todo: xml
    public string ProblemName;

    public GoalType goalType;
    public string   gType;

#region Data defining the dynamics of the game
    /// <summary>
    /// Dimension of the phase vector
    /// </summary>
    public readonly int n;

    /// <summary>
    /// The main matrix
    /// </summary>
    public readonly Matrix A;

    /// <summary>
    /// Dimension of the useful control
    /// </summary>
    public readonly int pDim;

    /// <summary>
    /// The useful control matrix
    /// </summary>
    public readonly Matrix B;

    /// <summary>
    /// Dimension of the disturbance
    /// </summary>
    public readonly int qDim;

    /// <summary>
    /// The disturbance matrix
    /// </summary>
    public readonly Matrix C;

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

#region Control constraints
    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the first player
    /// </summary>
    public ConvexPolytop P;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the second player
    /// </summary>
    public ConvexPolytop Q;

    //todo: xml
    public readonly string PSetInfo;
    public readonly string QSetInfo;
    public readonly string GameInfo;
#endregion
#endregion

#region Matrices related to the system
    /// <summary>
    /// The fundamental Cauchy matrix of the corresponding system
    /// </summary>
    public readonly CauchyMatrix cauchyMatrix;
#endregion

#region Constructor
    /// <summary>
    /// Reading and initializing data. Order is important!!!
    /// </summary>
    /// <param name="paramReader">The reader with the data.</param>
    public GameData(ParamReader paramReader) {
      _pr = paramReader;

      ProblemName = _pr.ReadString("ProblemName");

      // Game type
      goalType = _pr.ReadString("GoalType") switch
                   {
                     "Itself"   => GoalType.Itself
                   , "Epigraph" => GoalType.PayoffEpigraph
                   , _          => throw new ArgumentException("GameData.Ctor: GoalType must be \"Itself\" or \"Epigraph\".")
                   };

      gType = goalType == GoalType.Itself ? "It_" : "Ep_";


      // Dynamics
      n    = _pr.ReadNumber<int>("n");
      A    = new Matrix(_pr.Read2DArray<TNum>("A", n, n));
      pDim = _pr.ReadNumber<int>("pDim");
      B    = new Matrix(_pr.Read2DArray<TNum>("B", n, pDim));
      qDim = _pr.ReadNumber<int>("qDim");
      C    = new Matrix(_pr.Read2DArray<TNum>("C", n, qDim));

      t0 = _pr.ReadNumber<TNum>("t0");
      T  = _pr.ReadNumber<TNum>("T");
      dt = _pr.ReadNumber<TNum>("dt");

      // Reading data of the first player's control
      P = ReadSet(_pr, 'P', pDim, out PSetInfo);

      // Reading data of the second player's control
      Q = ReadSet(_pr, 'Q', qDim, out QSetInfo);

      // The Cauchy matrix
      cauchyMatrix = new CauchyMatrix(A, T, dt);

      // Расширяем систему, если решаем задачу с надграфиком функции цены
      if (goalType == GoalType.PayoffEpigraph) {
        n++; // размерность стала на 1 больше
        A = Matrix.vcat(A, Matrix.Zero(1, n - 1));
        A = Matrix.hcat(A, Matrix.Zero(n, 1))!;
        B = Matrix.vcat(B, Matrix.Zero(1, pDim));
        C = Matrix.vcat(C, Matrix.Zero(1, qDim));
      }


      // calc hashes
      string Astr = A.ToString();
      string Bstr = B.ToString();
      string Cstr = C.ToString();

      GameInfo = $"{Astr}{Bstr}{Cstr}{T}{dt}{PSetInfo}{QSetInfo}";
      PSetInfo = $"{Astr}{Bstr}{T}{dt}{PSetInfo}";
      QSetInfo = $"{Astr}{Cstr}{T}{dt}{QSetInfo}";
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
    public static ConvexPolytop ReadSet(ParamReader pr, char player, int dim, out string setTypeInfo) {
      setTypeInfo = pr.ReadString($"{player}SetType");
      SetType setType = setTypeInfo switch
                          {
                            "ConvexPolytope" => SetType.ConvexPolytop
                          , "RectParallel"   => SetType.RectParallel
                          , "Sphere"         => SetType.Sphere
                          , "Ellipsoid"      => SetType.Ellipsoid
                          , _ => throw new ArgumentOutOfRangeException
                                   ($"GameData.ReadSet: {setTypeInfo} is not supported for now.")
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
          int    Theta  = pr.ReadNumber<int>($"{player}Theta");
          int    Phi    = pr.ReadNumber<int>($"{player}Phi");
          TNum[] Center = pr.Read1DArray<TNum>($"{player}Center", dim);
          TNum   Radius = pr.ReadNumber<TNum>($"{player}Radius");
          res = ConvexPolytop.Sphere(dim, Theta, Phi, new Vector(Center, false), Radius);

          setTypeInfo += $"-T{Theta}-P{Phi}-R{Radius}";

          break;
        }
        case SetType.Ellipsoid: {
          int    Theta          = pr.ReadNumber<int>($"{player}Theta");
          int    Phi            = pr.ReadNumber<int>($"{player}Phi");
          TNum[] Center         = pr.Read1DArray<TNum>($"{player}Center", dim);
          TNum[] SemiaxesLength = pr.Read1DArray<TNum>($"{player}SemiaxesLength", dim);
          res = ConvexPolytop.Ellipsoid(dim, Theta, Phi, new Vector(Center, false), new Vector(SemiaxesLength, false));

          setTypeInfo += $"-T{Theta}-P{Phi}-SA{string.Join(' ', SemiaxesLength)}";

          break;
        }
      }

      return res ?? throw new InvalidOperationException($"GameData.ReadSets: Player {player} set is empty!");
    }

#endregion

  }

}
