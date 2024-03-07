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
    /// Path to the folder whereto the result should be written
    /// </summary>
    public readonly string path;

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
    public enum TypeSet {

      /// <summary>
      /// List of vertices
      /// </summary>
      VertList

     ,

      /// <summary>
      /// Axis parallel Cube.
      /// </summary>
      Cube

     ,

      /// <summary>
      /// Sphere
      /// </summary>
      Sphere

      // todo Эллипсоид
    }


    /// <summary>
    /// The type of the first player set
    /// 0 - List of the vertices: num dim Xi -> number_of_points dimension their_coordinates
    /// 1 - Rectangle-parallel: dim X Y -> dimension opposite_vertices
    /// 3 - Sphere: dim x0 x1 .. xn theta phi R -> dimension center_coordinates theta_division phis_division radius
    /// </summary>
    public int PTypeSet;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the first player
    /// </summary>
    public ConvexPolytop P = null!;


    /// <summary>
    /// Precomputed vectograms of the first player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Ps;

    /// <summary>
    /// The type of the second player set
    /// 0 - List of the vertices: num dim Xi -> number_of_points dimension their_coordinates
    /// 1 - Rectangle-parallel: dim X Y -> dimension opposite_vertices
    /// 3 - Sphere: dim x0 x1 .. xn theta phi R -> dimension center_coordinates theta_division phis_division radius
    /// </summary>
    public int QTypeSet;

    /// <summary>
    /// Collection of points, which convex hull defines the constraint for the control of the second player
    /// </summary>
    public ConvexPolytop Q = null!;

    /// <summary>
    /// Precomputed vectograms of the second player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Qs;
#endregion

#region Data defining terminal set
    /// <summary>
    /// The indices of the coordinates to be projected.
    /// </summary>
    public int[] projJ;

    /// <summary>
    /// The type of the terminal set
    /// 0 - List of the vertices: num dim Xi -> number_of_points dimension their_coordinates
    /// 1 - Rectangle-parallel: dim X Y -> dimension opposite_vertices
    /// 3 - Sphere: dim x0 x1 .. xn theta phi R -> dimension center_coordinates theta_division phis_division radius
    /// </summary>
    public int MTypeSet;

    /// <summary>
    /// The terminal set
    /// </summary>
    public ConvexPolytop M = null!;
#endregion

    /// <summary>
    /// The fundamental Cauchy matrix of the corresponding system
    /// </summary>
    public readonly CauchyMatrix cauchyMatrix;

    /// <summary>
    /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
    /// </summary>
    public readonly Matrix ProjMatr;

    /// <summary>
    /// Collection of matrices D for the instants from the time grid
    /// </summary>
    public readonly SortedDictionary<TNum, Matrix> D;

    /// <summary>
    /// Collection of matrices E for the instants from the time grid
    /// </summary>
    public readonly SortedDictionary<TNum, Matrix> E;
#endregion

#region Constructor
    /// <summary>
    /// Reading and initializing data
    /// </summary>
    /// <param name="inFName">File with the data</param>
    public GameData(string inFName) {
      ParamReader pr = new ParamReader(inFName);

      ProblemName = pr.ReadString("ProblemName");
      // path        = pr.ReadString("path"); // todo понадобится, когда в файл будем писать
      //
      // if (path[^1] == '/') {
      //   StringBuilder sb = new StringBuilder(path);
      //   sb[^1] = '/';
      //   path   = sb.ToString();
      // } else if (path[^1] != '/') {
      //   path += '/';
      // }
      //
      // if (!Directory.Exists(path)) { //Cur dir must be in work directory
      //   Directory.CreateDirectory(path);
      // }

      // Dynamics
      n = pr.ReadInt("n");
      A = new Matrix(pr.Read2DArray<TNum>("A", n, n));
      p = pr.ReadInt("p");
      B = new Matrix(pr.Read2DArray<TNum>("B", n, p));
      q = pr.ReadInt("q");
      C = new Matrix(pr.Read2DArray<TNum>("C", n, q));

      t0 = TConv.FromDouble(pr.ReadDouble("t0"));
      T  = TConv.FromDouble(pr.ReadDouble("T"));
      dt = TConv.FromDouble(pr.ReadDouble("dt"));

      d     = pr.ReadInt("d");
      projJ = pr.Read1DArray<int>("projJ", d);

      // The Cauchy matrix
      cauchyMatrix = new CauchyMatrix(A, T, dt);

      // Reading data on the first player's control and generating the constraint if necessary
      ReadSets(pr, 'p');

      // Reading data on the second player's control and generating the constraint if necessary
      ReadSets(pr, 'q');

      // WriteQTubes = pr.ReadBoolean("WriteQTubes");

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
    /// <summary>
    /// The function fills in the fields of the original sets
    /// </summary>
    /// <param name="pr">ParamReader</param>
    /// <param name="set">p - first player. q - second player. M - terminal set.</param>
    /// <exception cref="ArgumentException">If 'set' != p,q,M </exception>
    /// <exception cref="InvalidOperationException">If the read set is empty</exception>
    private void ReadSets(ParamReader pr, char set) {
      string pref;
      int    typeSetInt, dim;
      switch (set) {
        case 'p': {
          pref       = "P";
          PTypeSet   = pr.ReadInt("PTypeSet");
          typeSetInt = PTypeSet;
          dim        = p;
        }

          break;
        case 'q': {
          pref       = "Q";
          QTypeSet   = pr.ReadInt("QTypeSet");
          typeSetInt = QTypeSet;
          dim        = q;
        }

          break;
        case 'M': {
          pref       = "M";
          MTypeSet   = pr.ReadInt("MTypeSet");
          typeSetInt = MTypeSet;
          dim        = d;
        }

          break;
        default: throw new ArgumentException($"{set} must be 'p', 'q' or 'M'!");
      }

      TypeSet typeSet = typeSetInt switch
                          {
                            1 => TypeSet.VertList
                          , 2 => TypeSet.Cube
                          // , 2 => TypeSet.Sphere
                          , _ => throw new ArgumentOutOfRangeException($"{typeSetInt} must be [0, 2]!"),
                          };

      // Array for coordinates of the next point
      HashSet<Vector>? res = null;
      switch (typeSet) {
        case TypeSet.VertList: {
          int     Qnt  = pr.ReadInt(pref + "Qnt");
          TNum[,] Vert = pr.Read2DArray<TNum>(pref + "Vert", Qnt, dim);
          res = Array2DToHashSet(Vert, Qnt, dim);

          break;
        }
        case TypeSet.Cube: {
          TNum MCube = TConv.FromDouble(pr.ReadDouble(pref + "Cube"));
          res = ConvexPolytop.Cube(d,MCube).Vertices;

          break;
        }
        case TypeSet.Sphere: {
          throw new NotImplementedException("Сделать сферу!");

          break;
        }
      }

      switch (set) {
        case 'p':
          P = ConvexPolytop.AsVPolytop(res ?? throw new InvalidOperationException("First players set is empty!"));

          break;
        case 'q':
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
