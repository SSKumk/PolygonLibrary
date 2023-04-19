using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using LDGObjects;
using ParamReaderLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;

namespace TwoPartialPursuer;

/// <summary>
/// Enumeration of the computation type.
/// In particular, such an information is necessary for correct deleting 
/// results of previous computations in the problem folder
/// </summary>
public enum ComputationType {
  /// <summary>
  /// This exemplar of GameData is initialized for computation of stable bridges.
  /// All files of results should be deleted
  /// </summary>
  StableBridge
}

// public enum ControlConstrType {
//   /// <summary>
//   /// A convex hull of a collection of points;
//   /// now only for the case p = 2 
//   /// (because the convex hull can be constructed in 2D only)
//   /// </summary>
//   ConvexHull
// , BoxConstr
// , Circle
// , Ellipse
// }

/// <summary>
/// Class for keeping game parameter data
/// </summary>
public class GameData {

#region Information about the initial clearance of the problem folder
  /// <summary>
  /// Extensions of files with certain objects (correspond to the ComputationType enumeration)
  /// </summary>
  static public readonly SortedDictionary<ComputationType, string> Extensions =
    new SortedDictionary<ComputationType, string>()
      {
        { ComputationType.StableBridge, "bridge" }
      };

  /// <summary>
  /// Information what types of files to delete during some certain computation
  /// </summary>
  static protected SortedDictionary<ComputationType, List<ComputationType>> toDelete =
    new SortedDictionary<ComputationType, List<ComputationType>>()
      {
        // what to delete when stable bridges are computed
        {
          ComputationType.StableBridge, new List<ComputationType>()
            {
              ComputationType.StableBridge
            }
        }
      };
#endregion

#region Input data
  /// <summary>
  /// Name of the problem; to be written in all resultant files for checking their consistency
  /// </summary> 
  public readonly string ProblemName;

  /// <summary>
  /// Short name of the problem; to be used in SharpEye
  /// </summary>
  public readonly string ShortProblemName;

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
  public readonly double t0;

  /// <summary>
  /// The final instant
  /// </summary>
  public readonly double T;

  /// <summary>
  /// The time step
  /// </summary>
  public readonly double dt;
#endregion

#region Control constraints
  /// <summary>
  /// The type of the first player set
  /// 0 - List of the vertices: number of points and their coordinates
  /// 1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
  /// 2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
  /// 3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
  /// 4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
  /// </summary>
  public int PTypeSet;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the first player
  /// </summary>
  public List<Point> pVertices = null!;


  /// <summary>
  /// Precomputed vectograms of the first player
  /// </summary>
  public readonly SortedDictionary<double, ConvexPolygon> Ps;

  /// <summary>
  /// The type of the second player set
  /// 0 - List of the vertices: number of points and their coordinates
  /// 1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
  /// 2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
  /// 3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
  /// 4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
  /// </summary>
  public int QTypeSet;

  /// <summary>
  /// The type of the second player partitioning
  /// 0 - List of the vertexes:  n array -> number_of_points and their coordinates: Q1 and Q2
  /// 1 - Const breakdown: ind_1 ind_2 -> two non adjacent vertex indexes
  /// 2 - k alternating partitioning: k array -> number_of_parts  [an array of vertex indices of size 2 x k]
  /// 3 - rotating partitioning: ind step -> index_of_origin and step of rotating  
  /// </summary>
  public int QTypePart;

  /// <summary>
  /// Partition list by vertex indices
  /// </summary>
  public List<(int, int)> QPart;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices = null!;

  /// <summary>
  /// Collection of points, which convex hull defines the first part of constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices1;

  /// <summary>
  /// Collection of points, which convex hull defines the second part of the constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices2;


  /// <summary>
  /// Precomputed vectograms of the second player
  /// </summary>
  public readonly SortedDictionary<double, ConvexPolygon> Qs;

  /// <summary>
  /// Precomputed vectograms of the second player
  /// </summary>
  public readonly SortedDictionary<double, ConvexPolygon> Qs1;

  /// <summary>
  /// Precomputed vectograms of the second player
  /// </summary>
  public readonly SortedDictionary<double, ConvexPolygon> Qs2;
#endregion

#region Data defining terminal set
  /// <summary>
  /// The index of the first of two coordinates
  /// </summary>
  public int projI;

  /// <summary>
  /// The index of the second of two coordinates
  /// </summary>
  public int projJ;

  /// <summary>
  /// The type of the terminal set
  /// 0 - List of the vertices: number of points and their coordinates
  /// 1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
  /// 2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
  /// 3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
  /// 4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
  /// </summary>
  public int MTypeSet;

  /// <summary>
  /// The terminal set
  /// </summary>
  public ConvexPolygon M = null!;
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
  public readonly SortedDictionary<double, Matrix> D;

  /// <summary>
  /// Collection of matrices E for the instants from the time grid
  /// </summary>
  public readonly SortedDictionary<double, Matrix> E;
#endregion

#region Constructor
  /// <summary>
  /// Reading and initializing data
  /// </summary>
  /// <param name="inFName">File with the data</param>
  /// <param name="compObj">Computation type (to delete correctly files of previous computations)</param>
  public GameData(string inFName, ComputationType compObj) {
    ParamReader pr = new ParamReader(inFName);

    ProblemName      = pr.ReadString("ProblemName");
    ShortProblemName = pr.ReadString("ShortProblemName");
    path             = pr.ReadString("path");

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
    A = new Matrix(pr.Read2DArray<double>("A", n, n));
    p = pr.ReadInt("p");
    B = new Matrix(pr.Read2DArray<double>("B", n, p));
    q = pr.ReadInt("q");
    C = new Matrix(pr.Read2DArray<double>("C", n, q));

    t0 = pr.ReadDouble("t0");
    T  = pr.ReadDouble("T");
    dt = pr.ReadDouble("dt");

    // The Cauchy matrix
    cauchyMatrix = new CauchyMatrix(A, T, dt);

    // Reading data on the first player's control and generating the constraint if necessary
    ReadSets(pr, 'p');

    // Reading data on the second player's control and generating the constraint if necessary
    ReadSets(pr, 'q');

    ReadQPartioning(pr);

    //Reading data of terminal set type
    ReadSets(pr, 'M');

    // The projection matrix
    double[,] ProjMatrArr = new double[2, n];
    ProjMatrArr[0, projI] = 1.0;
    ProjMatrArr[1, projJ] = 1.0;
    ProjMatr              = new Matrix(ProjMatrArr);

    // The matrices D and E
    D = new SortedDictionary<double, Matrix>(new Tools.DoubleComparer(Tools.Eps));
    E = new SortedDictionary<double, Matrix>(new Tools.DoubleComparer(Tools.Eps));
    double t = T;
    while (Tools.GE(t, t0)) {
      Matrix Xstar = ProjMatr * cauchyMatrix[t];
      D[t] = Xstar * B;
      E[t] = Xstar * C;

      t -= dt;
    }

    StableBridge2D PTube  = new StableBridge2D(ProblemName, "PTube", 0.0, TubeType.Vectogram1st)
                 , QTube  = new StableBridge2D(ProblemName, "QTube", 0.0, TubeType.Vectogram2nd)
                 , QTube1 = new StableBridge2D(ProblemName, "Q1Tube", 0.0, TubeType.Vectogram2nd)
                 , QTube2 = new StableBridge2D(ProblemName, "Q2Tube", 0.0, TubeType.Vectogram2nd);

    int           qPartIdx = 0;
    ConvexPolygon cpMain   = new ConvexPolygon(qVertices);
    Debug.Assert(QPart != null, nameof(QPart) + " != null");
    for (t = T; Tools.GE(t, t0); t -= dt) {
      Debug.Assert(pVertices != null, nameof(pVertices) + " != null");
      Debug.Assert(qVertices != null, nameof(qVertices) + " != null");
      PTube.Add(new
                  TimeSection2D(t, new ConvexPolygon(pVertices.Select(pPoint => (Point2D)(-1.0 * D[t] * pPoint)).ToList(), true)));
      QTube.Add(new
                  TimeSection2D(t, new ConvexPolygon(qVertices.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));


      switch (QTypePart) {
        case 0: { }
          break;
        case 1:
        case 2: {
          var forCut = QPart.GetAtCyclic(qPartIdx);
          (var cp1, var cp2) = cpMain.CutConvexPolygon(forCut.Item1, forCut.Item2);
          qVertices1         = Point.List2DTohD(cp1.Vertices);
          qVertices2         = Point.List2DTohD(cp2.Vertices);
          qPartIdx++;
        }
          break;
      }
      Debug.Assert(qVertices1 != null, nameof(qVertices1) + " != null");
      Debug.Assert(qVertices2 != null, nameof(qVertices2) + " != null");
      QTube1.Add(new
                   TimeSection2D(t, new ConvexPolygon(qVertices1.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
      QTube2.Add(new
                   TimeSection2D(t, new ConvexPolygon(qVertices2.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
    }
    // Precomputing the players' vectorgrams 

    // Multiplication of the vectogram tubes by time step
    Ps = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in PTube)
      Ps[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(pPoint => dt * pPoint));
    Qs = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube)
      Qs[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(qPoint => dt * qPoint));
    Qs1 = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube1)
      Qs1[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(qPoint => dt * qPoint));
    Qs2 = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube2)
      Qs2[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(qPoint => dt * qPoint));
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
    int    typeSet;
    switch (set) {
      case 'p': {
        pref     = "P";
        PTypeSet = pr.ReadInt("PTypeSet");
        typeSet  = PTypeSet;
      }
        break;
      case 'q': {
        pref     = "Q";
        QTypeSet = pr.ReadInt("QTypeSet");
        typeSet  = QTypeSet;
      }
        break;
      case 'M': {
        pref     = "M";
        projI    = pr.ReadInt("projI");
        projJ    = pr.ReadInt("projJ");
        MTypeSet = pr.ReadInt("MTypeSet");
        typeSet  = MTypeSet;
      }
        break;
      default: throw new ArgumentException($"{set} must be 'p', 'q' or 'M'!");
    }
    ConvexPolygon? res = null;
    switch (typeSet) {
      case 0: {
        int       Qnt  = pr.ReadInt(pref + "Qnt");
        double[,] Vert = pr.Read2DArray<double>(pref + "Vert", Qnt, 2);

        var orig = Array2DToListPoint2D(Vert, Qnt);
        res = new ConvexPolygon(Convexification.ArcHull2D(orig));
        break;
      }
      case 1: {
        double[] rect = pr.Read1DArray<double>(pref + "RectParallel", 4);
        res = PolygonTools.RectangleParallel(rect[0], rect[1], rect[2], rect[3]);
        break;
      }
      case 2: {
        double[] rect = pr.Read1DArray<double>(pref + "Rect", 4);
        res = PolygonTools.RectangleTurned(rect[0], rect[1], rect[2], rect[3], pr.ReadDouble(pref + "Angle"));
        break;
      }
      case 3: {
        double[] center = pr.Read1DArray<double>(pref + "Center", 2);
        res = PolygonTools.Circle(center[0], center[1], pr.ReadDouble(pref + "Radius"), pr.ReadInt(pref + "QntVert")
                                , pr.ReadDouble(pref + "Angle"));
        break;
      }
      case 4: {
        double[] center = pr.Read1DArray<double>(pref + "Center", 2);
        double[] semi   = pr.Read1DArray<double>(pref + "Semiaxes", 2);
        res = PolygonTools.Ellipse(center[0], center[1], semi[0], semi[1], pr.ReadInt(pref + "QntVert")
                                 , pr.ReadDouble(pref + "Angle"), pr.ReadDouble(pref + "AngleAux"));
        break;
      }
    }

    switch (set) {
      case 'p':
        pVertices = Point.List2DTohD(res?.Vertices ??
                                     throw new InvalidOperationException("First player set is empty!"));
        break;
      case 'q':
        qVertices = Point.List2DTohD(res?.Vertices ??
                                     throw new InvalidOperationException("First player set is empty!"));
        break;
      case 'M':
        M = res ?? throw new InvalidOperationException("Terminal set is Empty!");
        break;
    }
  }

  /// <summary>
  /// Read necessary data to QPartitioning
  /// </summary>
  /// <param name="pr">ParamReader</param>
  /// <exception cref="ArgumentException">Thrown if indices are adjacent</exception>
  /// <exception cref="InvalidDataException">Thrown if QTypePart not in [0, 2]</exception>
  private void ReadQPartioning(ParamReader pr) {
    QTypePart = pr.ReadInt("QTypePart");
    switch (QTypePart) {
      case 0: {
        int       Q1Qnt = pr.ReadInt("Q1Qnt");
        int       Q2Qnt = pr.ReadInt("Q2Qnt");
        double[,] ar1       = pr.Read2DArray<double>("Q1Vert", Q1Qnt, 2);
        qVertices1 = Point.List2DTohD(Array2DToListPoint2D(ar1,Q1Qnt));
        double[,] ar2 = pr.Read2DArray<double>("Q2Vert", Q2Qnt, 2);
        qVertices2 = Point.List2DTohD(Array2DToListPoint2D(ar2,Q2Qnt));
      }
        break;
      case 1: {
        int    qnt = pr.ReadInt("QK");
        int[,] ar  = pr.Read2DArray<int>("QPart", qnt, 2);
        var    res = new List<(int, int)>();
        for (int i = 0; i < qnt; i++) {
          int f = ar[i, 0];
          int s = ar[i, 1];
          QIndexBorderCheck(f);
          QIndexBorderCheck(s);
          if (IsAdjacent(qVertices, f, s)) {
            throw new ArgumentException($"The vertices in 'Q set partitioning' {f} and {s} are adjacent!");
          }
          res.Add(new(ar[i, 0], ar[i, 1]));
        }
        QPart = res;
      }
        break;
      case 2: {
        int origin = pr.ReadInt("QOrigin");
        int step   = pr.ReadInt("QStep");
        QIndexBorderCheck(origin, "Origin");
        if (step < 1 || step > qVertices.Count - 2) {
          throw new ArgumentException($"Step {step} must be from 1 to {qVertices.Count - 2}!");
        }
        var res      = new List<(int, int)>();
        int id       = step;
        int idCycled = step;
        do {
          if (!IsAdjacent(qVertices, origin, idCycled)) {
            res.Add(new(origin, idCycled));
          }
          id       += step;
          idCycled =  id % qVertices.Count;
        } while (idCycled != step);

        QPart = res;
      }
        break;
      default: throw new InvalidDataException("The QTypePart must be from 0 to 2!");
    }
  }

  /// <summary>
  /// Aux. Array --> List
  /// </summary>
  /// <param name="ar">Array</param>
  /// <returns>List of point2D</returns>
  private static List<Point2D> Array2DToListPoint2D(double[,] ar, int qnt) {
    var orig = new List<Point2D>(qnt);
    for (int i = 0; i < qnt; i++) {
      orig.Add(new Point2D(ar[i, 0], ar[i, 1]));
    }
    return orig;
  }

  /// <summary>
  /// Aux, Is two indices is adjacent in a list 
  /// </summary>
  /// <param name="list">List</param>
  /// <param name="f">First index</param>
  /// <param name="s">Second index</param>
  /// <returns>True if adjacent. False otherwise</returns>
  private static bool IsAdjacent(ICollection list, int f, int s) {
    if (Math.Abs(f - s) < 2)
      return true;
    return Math.Abs(f - s) == list.Count - 1;
  }

  /// <summary>
  /// Aux. Check if id in [0, qVert.Count - 1]
  /// </summary>
  /// <param name="id">Id to be checked</param>
  /// <param name="mes">Name of id-field</param>
  /// <exception cref="InvalidEnumArgumentException">Thrown if id not in [0, qVert.Count - 1]</exception>
  private void QIndexBorderCheck(int id, string mes = "Index") {
    if (id < 0 || id >= qVertices.Count) {
      throw new
        InvalidEnumArgumentException($"{mes} {id} in 'Q set partitioning' must be from 0 to {qVertices.Count - 1}!");
    }
  }
#endregion

}
