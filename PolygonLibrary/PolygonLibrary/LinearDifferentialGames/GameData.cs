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
      /// Rectangle axices parallel
      /// </summary>
      RectParallel

     ,

      /// <summary>
      /// Sphere
      /// </summary>
      Sphere

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

#region Control tubes data
    /// <summary>
    /// The vectogram tube of the first player
    /// </summary>
    public StableBridge2D PTube;

    /// <summary>
    /// The vectogram tube of the entire constraint of the second player
    /// </summary>
    public StableBridge2D QTube;

    /// <summary>
    ///  Flag to write the partial tubes data
    /// </summary>
    public bool WriteQTubes;
#endregion

#region Constructor
    /// <summary>
    /// Reading and initializing data
    /// </summary>
    /// <param name="inFName">File with the data</param>
    public GameData(string inFName) {
      ParamReader pr = new ParamReader(inFName);

      ProblemName = pr.ReadString("ProblemName");
      path        = pr.ReadString("path");

      if (path[^1] == '/') {
        StringBuilder sb = new StringBuilder(path);
        sb[^1] = '/';
        path   = sb.ToString();
      } else if (path[^1] != '/') {
        path += '/';
      }

      if (!Directory.Exists(path)) { //Cur dir must be in work directory
        Directory.CreateDirectory(path);
      }

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

      PTube = new StableBridge2D(ProblemName, "PTube", 0.0, TubeType.Vectogram1st);
      QTube = new StableBridge2D(ProblemName, "QTube", 0.0, TubeType.Vectogram2nd);

      ConvexPolytop cpMain = null!;
      for (t = T; Tools.GE(t, t0); t -= dt) {
        Debug.Assert(pVertices != null, nameof(pVertices) + " is null");
        Debug.Assert(qVertices != null, nameof(qVertices) + " is null");
        PTube.Add
          (new TimeSection2D(t, new ConvexPolytop(pVertices.Select(pPoint => (Point2D)(-1.0 * D[t] * pPoint)).ToList(), true)));
        QTube.Add(new TimeSection2D(t, new ConvexPolytop(qVertices.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
      }

      // Multiplication of the vectogram tubes by time step
      Ps = new SortedDictionary<TNum, ConvexPolytop>();
      foreach (TimeSection2D ts in PTube)
        Ps[ts.t] = new ConvexPolytop(ts.section.Vertices.Select(pPoint => dt * pPoint));
      Qs = new SortedDictionary<TNum, ConvexPolytop>();
      foreach (TimeSection2D ts in QTube)
        Qs[ts.t] = new ConvexPolytop(ts.section.Vertices.Select(qPoint => dt * qPoint));
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
          projJ      = pr.Read1DArray<int>("projJ", d);
          MTypeSet   = pr.ReadInt("MTypeSet");
          typeSetInt = MTypeSet;
          dim        = d;
        }

          break;
        default: throw new ArgumentException($"{set} must be 'p', 'q' or 'M'!");
      }

      TypeSet typeSet = typeSetInt switch
                          {
                            0 => TypeSet.VertList
                          , 1 => TypeSet.RectParallel
                          , 2 => TypeSet.Sphere
                          , _ => throw new ArgumentOutOfRangeException($"{typeSetInt} must be [0, 2]!"),
                          };

      // Array for coordinates of the next point
      List<Point>? res = null;
      switch (typeSet) {
        case TypeSet.VertList: {
          int     Qnt  = pr.ReadInt(pref + "Qnt");
          TNum[,] Vert = pr.Read2DArray<TNum>(pref + "Vert", Qnt, dim);
          res = Array2DToList(Vert, Qnt, dim);

          break;
        }
        case TypeSet.RectParallel: {
          TNum[] leftCorner  = pr.Read1DArray<TNum>(pref + "RectParallelLeft", d);
          TNum[] rightCorner = pr.Read1DArray<TNum>(pref + "RectParallelRight", d);
          res = new List<Point>() { new Point(leftCorner), new Point(rightCorner) };

          break;
        }
        case TypeSet.Sphere: {
          TNum[] center = pr.Read1DArray<TNum>(pref + "Center", d);
          res = ; // todo Перенести фабрики многогранников в основную библиотеку (без генерации доп точек)

          break;
        }
      }

      switch (set) {
        case 'p':
          P = new ConvexPolytop(res ?? throw new InvalidOperationException("First players set is empty!"));

          break;
        case 'q':
          Q = new ConvexPolytop(res ?? throw new InvalidOperationException("Second players set is empty!"));

          break;
        case 'M':
          M = new ConvexPolytop(res ?? throw new InvalidOperationException("Terminal set is empty!"));

          break;
      }
    }

    /// <summary>
    /// Converts a two-dimensional array to a list of points.
    /// </summary>
    /// <param name="ar">The two-dimensional array to convert.</param>
    /// <param name="row">The number of rows in the array.</param>
    /// <param name="col">The number of columns in the array.</param>
    /// <returns>A list of points obtained from the two-dimensional array.</returns>
    private static List<Point> Array2DToList(TNum[,] ar, int row, int col) {
      var list = new List<Point>();
      for (int i = 0; i < row; i++) {
        var point = new TNum[col];
        for (int j = 0; j < col; j++) {
          point[j] = ar[i, j];
        }

        list.Add(new Point(point));
      }

      return list;
    }
#endregion

  }

}
