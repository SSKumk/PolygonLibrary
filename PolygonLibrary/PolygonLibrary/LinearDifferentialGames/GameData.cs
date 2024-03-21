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
    /// The reader from file.
    /// </summary>
    private readonly ParamReader _pr;

    /// <summary>
    /// The type of the game
    /// </summary>
    public enum GoalType { Itself, PayoffEpigraph }

    /// <summary>
    /// The type of the terminal set
    /// </summary>
    public enum MType { TerminalSet, Payoff_DistToOrigin, Payoff_distToPolytop }

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

      // , DistanceToOrigin
      // , DistanceToPolytop
      // , DistanceToCube SimplexRND???

    }


    /// <summary>
    /// The type of constraints of the first player.
    /// </summary>
    private string PSetType;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the first player
    /// </summary>
    private ConvexPolytop P;


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
    private ConvexPolytop Q;

    /// <summary>
    /// Precomputed vectograms of the second player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Qs;
#endregion

#region Data defining terminal set
    /// <summary>
    /// The type of the game.
    /// </summary>
    private readonly GoalType goalType;

    /// <summary>
    /// The type of the describing of the terminal set variants.
    /// </summary>
    private readonly MType _MType;

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
    public ConvexPolytop M;
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
      _pr = new ParamReader(inFName);

      string problemName = _pr.ReadString("ProblemName");

      // Dynamics
      n    = _pr.ReadInt("n");
      A    = new Matrix(_pr.Read2DArrayAndConvertToTNum("A", n, n));
      pDim = _pr.ReadInt("pDim");
      B    = new Matrix(_pr.Read2DArrayAndConvertToTNum("B", n, pDim));
      qDim = _pr.ReadInt("qDim");
      C    = new Matrix(_pr.Read2DArrayAndConvertToTNum("C", n, qDim));

      // Расширяем систему, если решаем задачу с награфиком функции цены
      if (goalType == GoalType.PayoffEpigraph) {
        n++; // размерность стала на 1 больше
        A = Matrix.vcat(A, Matrix.Zero(1, n - 1));
        A = Matrix.hcat(A, Matrix.Zero(n, 1));
        B = Matrix.vcat(B, Matrix.Zero(1, pDim));
        C = Matrix.vcat(C, Matrix.Zero(1, qDim));
      }

      t0 = _pr.ReadDoubleAndConvertToTNum("t0");
      T  = _pr.ReadDoubleAndConvertToTNum("T");
      dt = _pr.ReadDoubleAndConvertToTNum("dt");

      d     = _pr.ReadInt("d");
      projJ = _pr.Read1DArray<int>("projJ", d);

      // Reading data of the first player's control
      P = ReadSet('P', pDim, out string PSetTypeInfo);

      // Reading data of the second player's control
      Q = ReadSet('Q', qDim, out string QSetTypeInfo);


      // Game type
      goalType = _pr.ReadString("GoalType") switch
                   {
                     "Itself"   => GoalType.Itself
                   , "Epigraph" => GoalType.PayoffEpigraph
                   , _          => throw new ArgumentException("GameData: goalType must be \"Itself\" or \"Epigraph\".")
                   };

      // Game type
      _MType = _pr.ReadString("MType") switch
                 {
                   "TerminalSet"   => MType.TerminalSet
                 , "DistToOrigin"  => MType.Payoff_DistToOrigin
                 , "DistToPolytop" => MType.Payoff_distToPolytop
                 , _               => throw new ArgumentException("GameData: goalType must be \"Itself\" or \"Epigraph\".")
                 };


      //Reading data of terminal set type
      M = ReadTerminalSet(out string describeM);

      string gType = goalType == GoalType.Itself ? "It" : "Ep";
      ProblemName = $"{gType}_{problemName}_P#{PSetTypeInfo}_Q#{QSetTypeInfo}_M#{describeM}";


      // The Cauchy matrix
      cauchyMatrix = new CauchyMatrix(A, T, dt);

      if (goalType == GoalType.PayoffEpigraph) {
        int[] projJ_ex = new int[d + 1];
        for (int i = 0; i < d; i++) {
          projJ_ex[i] = projJ[i];
        }
        projJ_ex[d] = n - 1;
        d++; // расширили систему
        projJ = projJ_ex;
      }

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
     * Надо автоматически генерировать имя выходной папки! ProblemName должно содержать только базовое название
     *  - приставка I_ / E_ (Itself / Epigraph)
     *  - само имя
     *  - характеристики:
     *      > [t_0 T] интервал времени, на котором решается задача
     *      > Описание P и Q (ТИП)
     *      > Описание M
     *      >
     */

    /// <summary>
    /// Reads the data of the terminal set from file.
    /// </summary>
    /// <returns>The polytop that describes terminal set.</returns>
    private ConvexPolytop ReadTerminalSet(out string describeM) {
      describeM = "";
      switch (goalType) {
        // Случай игры с терминальным множеством
        case GoalType.Itself:
          switch (_MType) {
            // Явно дано терминальное множество
            case MType.TerminalSet: {
              ConvexPolytop readM = ReadSet('M', d, out string MSetTypeInfo);
              describeM += MSetTypeInfo;

              return readM;
            }

            // Дано терминальное множество и разбиение по С. 'M (+) Ball(0, C_i)'
            default: throw new NotImplementedException("Случай, когда нужна сетка по С надо сделать!");
          }

        // Случай игры с надграфиком функции платы
        case GoalType.PayoffEpigraph:
          switch (_MType) {
            // Явно дано терминальное множество расширенной системы
            case MType.TerminalSet: {
              ConvexPolytop readM = ReadSet('M', d + 1, out string MSetTypeInfo);
              describeM += MSetTypeInfo;

              return readM;
            }


            // Множество в виде расстояния до начала координат
            case MType.Payoff_DistToOrigin: {
              describeM += "DtnOrigin_";
              string BallType = _pr.ReadString("MBallType");
              describeM += BallType;
              int Theta = 10, Phi = 10;
              if (BallType == "Ball_2") {
                Theta = _pr.ReadInt("MTheta");
                Phi   = _pr.ReadInt("MPhi");

                describeM += $"-T{Theta}-P{Phi}_";
              }
              TNum CMax = _pr.ReadDoubleAndConvertToTNum("MCMax");
              describeM += $"-CMax{CMax}";

              return BallType switch
                       {
                         "Ball_1"  => ConvexPolytop.DistanceToOriginBall_1(d, CMax)
                       , "Ball_2"  => ConvexPolytop.DistanceToOriginBall_2(d, Theta, Phi, CMax)
                       , "Ball_oo" => ConvexPolytop.DistanceToOriginBall_oo(d, CMax)
                       , _         => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {BallType}")
                       };
            }

            // Множество в виде расстояния до заданного выпуклого многогранника в Rd
            case MType.Payoff_distToPolytop: {
              describeM += "DtnPolytop_";
              int           VsQnt    = _pr.ReadInt("MVsQnt");
              TNum[,]       Vs       = _pr.Read2DArrayAndConvertToTNum("MPolytop", VsQnt, d);
              ConvexPolytop Polytop  = ConvexPolytop.AsVPolytop(Array2DToHashSet(Vs, VsQnt, d));
              string        BallType = _pr.ReadString("MBallType");
              describeM += $"Vs-Qnt{VsQnt}_{BallType}";
              int Theta = 10, Phi = 10;
              if (BallType == "Ball_2") {
                Theta = _pr.ReadInt("MTheta");
                Phi   = _pr.ReadInt("MPhi");
              }
              TNum CMax = _pr.ReadDoubleAndConvertToTNum("MCMax");
              describeM += $"-CMax{CMax}";

              return BallType switch
                       {
                         "Ball_1"  => ConvexPolytop.DistanceToPolytopBall_1(Polytop, CMax)
                       , "Ball_2"  => ConvexPolytop.DistanceToPolytopBall_2(Polytop, Theta, Phi, CMax)
                       , "Ball_oo" => ConvexPolytop.DistanceToPolytopBall_oo(Polytop, CMax)
                       , _         => throw new ArgumentException($"Wrong type of the ball! Found {BallType}")
                       };
            }


            // Другие варианты функции платы
            // case :

            default: throw new ArgumentException("Другие варианты непредусмотрены!");
          }

        default:
          throw new ArgumentException($"GameData.ReadTerminalSet: goalType must be Itself or PayoffEpigraph. Found: {goalType}.");
      }
    }


    /// <summary>
    /// The function fills in the fields of the original sets
    /// </summary>
    /// <param name="player">P - first player. Q - second player. M - terminal set.</param>
    /// <param name="dim">The dimension of the set to be read.</param>
    /// <param name="setTypeInfo">The auxiliary information to write into task folder name.</param>
    private ConvexPolytop ReadSet(char player, int dim, out string setTypeInfo) {
      setTypeInfo = _pr.ReadString($"{player}SetType");
      SetType setType = setTypeInfo switch
                          {
                            "VertList"     => SetType.VertList
                          , "RectParallel" => SetType.RectParallel
                          , "Sphere"       => SetType.Sphere
                          , "Ellipsoid"    => SetType.Ellipsoid
                          , _              => throw new ArgumentOutOfRangeException($"{setTypeInfo} must be TODO!")
                          };

      // Array for coordinates of the next point
      HashSet<Vector>? res = null;
      switch (setType) {
        case SetType.VertList: {
          int     Qnt  = _pr.ReadInt($"{player}Qnt");
          TNum[,] Vert = _pr.Read2DArrayAndConvertToTNum($"{player}Vert", Qnt, dim);
          res = Array2DToHashSet(Vert, Qnt, dim);

          setTypeInfo += $"-Qnt{Qnt}";

          break;
        }
        case SetType.RectParallel: {
          TNum[] left  = _pr.Read1DArray_double($"{player}RectPLeft", dim);
          TNum[] right = _pr.Read1DArray_double($"{player}RectPRight", dim);
          res = ConvexPolytop.RectParallel(new Vector(left), new Vector(right)).Vertices;

          // setTypeStr += $"-{Qnt}"; ???? А что сюда можно написать?
          break;
        }
        case SetType.Sphere: {
          int    Theta  = _pr.ReadInt($"{player}Theta");
          int    Phi    = _pr.ReadInt($"{player}Phi");
          TNum[] Center = _pr.Read1DArray_double($"{player}Center", dim);
          TNum   Radius = _pr.ReadDoubleAndConvertToTNum($"{player}Radius");
          res = ConvexPolytop.Sphere(dim, Theta, Phi, new Vector(Center), Radius).Vertices;

          setTypeInfo += $"-T{Theta}-P{Phi}-R{Radius}";

          break;
        }
        case SetType.Ellipsoid: {
          int    Theta          = _pr.ReadInt($"{player}Theta");
          int    Phi            = _pr.ReadInt($"{player}Phi");
          TNum[] Center         = _pr.Read1DArray_double($"{player}Center", dim);
          TNum[] SemiaxesLength = _pr.Read1DArray_double($"{player}SemiaxesLength", dim);
          res = ConvexPolytop.Ellipsoid(dim, Theta, Phi, new Vector(Center), new Vector(SemiaxesLength)).Vertices;

          setTypeInfo += $"-T{Theta}-P{Phi}-SA{string.Join(' ', SemiaxesLength)}";

          break;
        }
      }

      return ConvexPolytop.AsVPolytop(res ?? throw new InvalidOperationException($"Players {player} set is empty!"));
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
