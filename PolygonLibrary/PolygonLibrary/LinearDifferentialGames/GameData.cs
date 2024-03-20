using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // todo Продумать систему пространства имён
  // todo Добавить в нашу библиотеку проект LDG2D. Для этого сделать пространство имён.
  // todo Закинуть в Toolkit всё, что не геометрия

  /// <summary>
  /// Class for keeping game parameter data
  /// </summary>
  public class GameData {

#region Input data
    /// <summary>
    /// Name of the problem; to be written in all resultant files for checking their consistency
    /// </summary>
    public readonly string ProblemName;

    /// <summary>
    /// The type of the game
    /// </summary>
    public enum GoalType { Itself, PayoffSupergraphic }

#region Data defining the dynamics of the game
    /// <summary>
    /// Dimension of the phase vector
    /// </summary>
    public readonly int n;

    /// <summary>
    /// Dimension of the projected system
    /// </summary>
    public readonly int d;

    /// <summary>
    /// The main matrix
    /// </summary>
    public readonly Matrix A;

    /// <summary>
    /// Dimension of the useful control
    /// </summary>
    public readonly int p;

    /// <summary>
    /// The useful control matrix
    /// </summary>
    public readonly Matrix B;

    /// <summary>
    /// Dimension of the disturbance
    /// </summary>
    public readonly int q;

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
    /// The type of the set.
    /// VertList - List of the vertices: num dim Xi -> number_of_points dimension their_coordinates
    /// RectParallel - Rectangle-parallel: dim X Y -> dimension opposite_vertices
    /// Sphere - Sphere: dim x0 x1 .. xn theta phi R -> dimension center_coordinates theta_division phis_division radius
    /// Ellipsoid - Ellipsoid: dim x0 x1 .. xn theta phi a0 a1 ... an -> dimension center_coordinates theta_division phis_division semi-axis_length
    /// </summary>
    public enum SetType {

      /// <summary>
      /// List of vertices.
      /// </summary>
      VertList

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

    , DistanceToOrigin
    , DistanceToPolytop
      // , DistanceToCube SimplexRND???

    }


    /// <summary>
    /// The type of constraints of the first player.
    /// </summary>
    private string PSetType;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the first player
    /// </summary>
    private ConvexPolytop P = null!;


    /// <summary>
    /// Precomputed vectograms of the first player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Ps;

    /// <summary>
    /// The type of constraints of the second player.
    /// </summary>
    private string QSetType;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the second player
    /// </summary>
    private ConvexPolytop Q = null!;

    /// <summary>
    /// Precomputed vectograms of the second player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Qs;
#endregion

#region Data defining terminal set
    /// <summary>
    /// The goal type.
    /// </summary>
    private GoalType goalType;

    /// <summary>
    /// The indices of the coordinates to be projected.
    /// </summary>
    private int[] projJ;

    /// <summary>
    /// The type of terminal set.
    /// </summary>
    private string MSetType;

    /// <summary>
    /// The type of terminal set
    /// </summary>
    public ConvexPolytop M = null!;
#endregion

    /// <summary>
    /// The fundamental Cauchy matrix of the corresponding system
    /// </summary>
    private readonly CauchyMatrix cauchyMatrix;

    /// <summary>
    /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
    /// </summary>
    private readonly Matrix ProjMatr;

    /// <summary>
    /// Collection of matrices D for the instants from the time grid
    /// </summary>
    private readonly SortedDictionary<TNum, Matrix> D;

    /// <summary>
    /// Collection of matrices E for the instants from the time grid
    /// </summary>
    private readonly SortedDictionary<TNum, Matrix> E;
#endregion

#region Constructor
    /// <summary>
    /// Reading and initializing data. Order is important!!!
    /// </summary>
    /// <param name="inFName">File with the data.</param>
    public GameData(string inFName) {
      ParamReader pr = new ParamReader(inFName);

      ProblemName = pr.ReadString("ProblemName");

      // Game type
      goalType = pr.ReadInt("GoalType") switch
                   {
                     0 => GoalType.Itself
                   , 1 => GoalType.PayoffSupergraphic
                   , _ => throw new ArgumentOutOfRangeException($"GameData: goalType must be 0 or 1.")
                   };

      // Dynamics
      n = pr.ReadInt("n");
      A = new Matrix(pr.Read2DArrayAndConvertToTNum("A", n, n));
      p = pr.ReadInt("p");
      B = new Matrix(pr.Read2DArrayAndConvertToTNum("B", n, p));
      q = pr.ReadInt("q");
      C = new Matrix(pr.Read2DArrayAndConvertToTNum("C", n, q));

      // Расширяем систему, если решаем задачу с награфиком функции цены
      if (goalType == GoalType.PayoffSupergraphic) {
        n++; // размерность стала на 1 больше
        A = Matrix.vcat(A, Matrix.Zero(1, n - 1));
        A = Matrix.hcat(A, Matrix.Zero(n, 1));
        B = Matrix.vcat(B, Matrix.Zero(1, p));
        C = Matrix.vcat(C, Matrix.Zero(1, q));
      }

      t0 = pr.ReadDoubleAndConvertToTNum("t0");
      T  = pr.ReadDoubleAndConvertToTNum("T");
      dt = pr.ReadDoubleAndConvertToTNum("dt");

      d     = pr.ReadInt("d");
      projJ = pr.Read1DArray<int>("projJ", d);

      if (goalType == GoalType.PayoffSupergraphic) {
        int[] projJ_ex = new int[d + 1];
        for (int i = 0; i < d; i++) {
          projJ_ex[i] = projJ[i];
        }
        projJ_ex[d] = n - 1;
        d++; // расширили систему
        projJ = projJ_ex;
      }

      // The Cauchy matrix
      cauchyMatrix = new CauchyMatrix(A, T, dt);

      // Reading data on the first player's control and generating the constraint if necessary
      ReadSets(pr, 'P');

      // Reading data on the second player's control and generating the constraint if necessary
      ReadSets(pr, 'Q');

      //Reading data of terminal set type
      ReadSets(pr, 'M');

      // The projection matrix
      TNum[,] ProjMatrArr = new TNum[d, n];
      for (int i = 0; i < d; i++) {
        ProjMatrArr[i, projJ[i]] = Tools.One;
      }
      ProjMatr = new Matrix(ProjMatrArr);

      // The matrices D and E
      D = new SortedDictionary<TNum, Matrix>();
      E = new SortedDictionary<TNum, Matrix>();
      TNum t = T;
      while (Tools.GE(t, t0)) {
        Matrix Xstar = ProjMatr * cauchyMatrix[t];
        D[t] = Xstar * B;
        E[t] = Xstar * C;

        t -= dt;
      }

      Ps = new SortedDictionary<TNum, ConvexPolytop>();
      Qs = new SortedDictionary<TNum, ConvexPolytop>();

      // Вычисляем вдоль всего моста выражения:  -dt*X(T,t_i)*B*P  и  dt*X(T,t_i)*C*Q
      for (t = T; Tools.GE(t, t0); t -= dt) {
        TNum
          t1 = t; // Для борьбы с "Captured variable is modified in the outer scope" (Code Inspection: Access to modified captured variable)
        Ps[t] = ConvexPolytop.AsFLPolytop(P.Vertices.Select(pPoint => -dt * D[t1] * pPoint).ToHashSet(), true);
        Qs[t] = ConvexPolytop.AsVPolytop(Q.Vertices.Select(qPoint => dt * E[t1] * qPoint).ToHashSet(), true);
      }
    }
#endregion

#region Aux procedures

    /*
     * Логику чтения и обработки терминального множества надо разделить от P и Q!
     * SetType наверное стоит оставить только явные множества, без distTo...
     * Нужно ещё одно перечисление MType TerminalSet / Supergraphic
     * MType.TerminalSet и GoalType.Itself, то только задание множества (в Rd)
     * MType.TerminalSet и GoalType.Supergraphic, то множество в R{d+1}
     * MType.Payoff и GoalType.Itself, то множество в Rd + сетка по С
     * MType.Payoff и GoalType.Supergraphic, то расстояния до множества (множество в Rd) или нуля, итог в R{d+1}
     */




    /// <summary>
    /// The function fills in the fields of the original sets
    /// </summary>
    /// <param name="pr">ParamReader</param>
    /// <param name="set">P - first player. Q - second player. M - terminal set.</param>
    private void ReadSets(ParamReader pr, char set) {
      string pref;
      int    dim;
      string typeSetInt;
      switch (set) {
        case 'P': {
          pref       = "P";
          PSetType   = pr.ReadString("PSetType");
          typeSetInt = PSetType;
          dim        = p;
        }

          break;
        case 'Q': {
          pref       = "Q";
          QSetType   = pr.ReadString("QSetType");
          typeSetInt = QSetType;
          dim        = q;
        }

          break;
        case 'M': {
          pref       = "M";
          MSetType   = pr.ReadString("MSetType");
          typeSetInt = MSetType;
          dim        = goalType == GoalType.PayoffSupergraphic ? d - 1 : d;
        }

          break;
        default: throw new ArgumentException($"{set} must be 'p', 'q' or 'M'!");
      }

      SetType setType = typeSetInt switch
                          {
                            "VertList"          => SetType.VertList
                          , "RectParallel"      => SetType.RectParallel
                          , "Sphere"            => SetType.Sphere
                          , "Ellipsoid"         => SetType.Ellipsoid
                          , "DistanceToOrigin"  => SetType.DistanceToOrigin
                          , "DistanceToPolytop" => SetType.DistanceToPolytop
                          , _                   => throw new ArgumentOutOfRangeException($"{typeSetInt} must be TODO!")
                          };

      // Array for coordinates of the next point
      HashSet<Vector>? res = null;
      switch (setType) {
        case SetType.VertList: {
          int     Qnt  = pr.ReadInt(pref + "Qnt");
          TNum[,] Vert = pr.Read2DArrayAndConvertToTNum(pref + "Vert", Qnt, dim);
          res = Array2DToHashSet(Vert, Qnt, dim);

          break;
        }
        case SetType.RectParallel: {
          TNum[] left  = pr.Read1DArray_double(pref + "RectPLeft", dim);
          TNum[] right = pr.Read1DArray_double(pref + "RectPRight", dim);
          res = ConvexPolytop.RectParallel(new Vector(left), new Vector(right)).Vertices;

          break;
        }
        case SetType.Sphere: {
          int    Theta  = pr.ReadInt(pref + "Theta");
          int    Phi    = pr.ReadInt(pref + "Phi");
          TNum[] Center = pr.Read1DArray_double(pref + "Center", dim);
          TNum   Radius = pr.ReadDoubleAndConvertToTNum(pref + "Radius");
          res = ConvexPolytop.Sphere(dim, Theta, Phi, new Vector(Center), Radius).Vertices;

          break;
        }
        case SetType.Ellipsoid: {
          int    Theta          = pr.ReadInt(pref + "Theta");
          int    Phi            = pr.ReadInt(pref + "Phi");
          TNum[] Center         = pr.Read1DArray_double(pref + "Center", dim);
          TNum[] SemiaxesLength = pr.Read1DArray_double(pref + "SemiaxesLength", dim);
          res = ConvexPolytop.Ellipsoid(dim, Theta, Phi, new Vector(Center), new Vector(SemiaxesLength)).Vertices;

          break;
        }
        case SetType.DistanceToOrigin: {
          if (goalType == GoalType.PayoffSupergraphic) {
            string BallType = pr.ReadString(pref + "BallType");
            int    Theta    = 10, Phi = 10;
            if (BallType == "Ball_2") {
              Theta = pr.ReadInt(pref + "Theta");
              Phi   = pr.ReadInt(pref + "Phi");
            }
            TNum CMax = pr.ReadDoubleAndConvertToTNum(pref + "CMax");
            res = BallType switch
                    {
                      "Ball_1"  => ConvexPolytop.DistanceToOriginBall_1(dim, CMax).Vertices
                    , "Ball_2"  => ConvexPolytop.DistanceToOriginBall_2(dim, Theta, Phi, CMax).Vertices
                    , "Ball_oo" => ConvexPolytop.DistanceToOriginBall_oo(dim, CMax).Vertices
                    , _         => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {BallType}")
                    };
          } else { throw new ArgumentException("DistanceToOrigin is allowed only if GoalType = 1!"); }

          break;
        }
        case SetType.DistanceToPolytop: {
          if (goalType == GoalType.PayoffSupergraphic) {
            int           VsQnt    = pr.ReadInt(pref + "VsQnt");
            TNum[,]       Vs       = pr.Read2DArrayAndConvertToTNum(pref + "Polytop", VsQnt, dim);
            ConvexPolytop Polytop  = ConvexPolytop.AsVPolytop(Array2DToHashSet(Vs, VsQnt, dim));
            string        BallType = pr.ReadString(pref + "BallType");
            int           Theta    = 10, Phi = 10;
            if (BallType == "Ball_2") {
              Theta = pr.ReadInt(pref + "Theta");
              Phi   = pr.ReadInt(pref + "Phi");
            }
            TNum CMax = pr.ReadDoubleAndConvertToTNum(pref + "CMax");
            res = BallType switch
                    {
                      "Ball_1"  => ConvexPolytop.DistanceToPolytopBall_1(Polytop, CMax).Vertices
                    , "Ball_2"  => ConvexPolytop.DistanceToPolytopBall_2(Polytop, Theta, Phi, CMax).Vertices
                    , "Ball_oo" => ConvexPolytop.DistanceToPolytopBall_oo(Polytop, CMax).Vertices
                    , _         => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {BallType}")
                    };
          } else { throw new ArgumentException("DistanceToPolytop is allowed only if GoalType = 1!"); }

          break;
        }
      }

      switch (set) {
        case 'P':
          P = ConvexPolytop.AsVPolytop(res ?? throw new InvalidOperationException("First players set is empty!"));

          break;
        case 'Q':
          Q = ConvexPolytop.AsVPolytop(res ?? throw new InvalidOperationException("Second players set is empty!"));

          break;
        case 'M':
          M = ConvexPolytop.AsVPolytop(res ?? throw new InvalidOperationException("Terminal set is empty!"));

          break;
      }
    }

    /// <summary>
    /// Converts a two-dimensional array to a HashSet of points.
    /// </summary>
    /// <param name="ar">The two-dimensional array to convert.</param>
    /// <param name="row">The number of rows in the array.</param>
    /// <param name="col">The number of columns in the array.</param>
    /// <returns>A hash set of points obtained from the two-dimensional array.</returns>
    private static HashSet<Vector> Array2DToHashSet(TNum[,] ar, int row, int col) {
      HashSet<Vector> list = new HashSet<Vector>();
      for (int i = 0; i < row; i++) {
        TNum[] point = new TNum[col];
        for (int j = 0; j < col; j++) {
          point[j] = ar[i, j];
        }

        list.Add(new Vector(point));
      }

      return list;
    }
#endregion

  }

}
