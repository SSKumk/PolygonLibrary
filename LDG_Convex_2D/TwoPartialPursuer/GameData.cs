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

/*
 * todo 1) Переименовать режимы чтения ограничений ( с 0 у P ) ?
 * 
 */
/*todo 1) Читаем P-параллелотоп  ?+
  todo 2) Чтение P,Q,M в виде списка вершин ?+ 
  todo 3) Чтение разбиения в виде явного представления разбиения множеств Q1 Q2 (для dim > 0) ?+
  todo 4) Вращающиеся разбиение ?+
  todo 5) Комментарии подправить в файле данных ?+
 */

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

public enum TypeSet {
  /// <summary>
  /// Box-hD constraints
  /// </summary>
  Box

 ,

  /// <summary>
  /// List of vertices
  /// </summary>
  ListOfVert

 ,

  /// <summary>
  /// Rectangle axices parallel
  /// </summary>
  RectParallel

 ,

  /// <summary>
  /// Turned rectangle
  /// </summary>
  RectTurned

 ,

  /// <summary>
  /// Circle
  /// </summary>
  Circle

 ,

  /// <summary>
  /// Ellipse
  /// </summary>
  Ellipse
}

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
  public List<(int, int)>? QAltParttitiong;

  //For rotating partitioning
  /// <summary> First index </summary>
  private int QInd1;

  /// <summary> Second index </summary>
  private int QInd2;

  /// <summary> Step </summary>
  private int QStep;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices = null!;

  /// <summary>
  /// Collection of points, which convex hull defines the first part of constraint for the control of the second player
  /// </summary>
  public List<Point>? qVertices1;

  /// <summary>
  /// Collection of points, which convex hull defines the second part of the constraint for the control of the second player
  /// </summary>
  public List<Point>? qVertices2;


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
  /// The first partial vectogram tube of the second player 
  /// </summary>
  public StableBridge2D QTube1;

  /// <summary>
  /// The second partial vectogram tube of the second player 
  /// </summary>
  public StableBridge2D QTube2;

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
    path = pr.ReadString("path");

    if (path[^1] == '/') {
      StringBuilder sb = new StringBuilder(path);
      sb[^1] = '/';
      path = sb.ToString();
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
    T = pr.ReadDouble("T");
    dt = pr.ReadDouble("dt");

    // The Cauchy matrix
    cauchyMatrix = new CauchyMatrix(A, T, dt);

    // Reading data on the first player's control and generating the constraint if necessary
    ReadSets(pr, 'p');

    // Reading data on the second player's control and generating the constraint if necessary
    ReadSets(pr, 'q');

    ReadQPartioning(pr);

    WriteQTubes = pr.ReadBoolean("WriteQTubes");

    //Reading data of terminal set type
    ReadSets(pr, 'M');

    // The projection matrix
    double[,] ProjMatrArr = new double[2, n];
    ProjMatrArr[0, projI] = 1.0;
    ProjMatrArr[1, projJ] = 1.0;
    ProjMatr = new Matrix(ProjMatrArr);

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

    PTube = new StableBridge2D(ProblemName, "PTube", 0.0, TubeType.Vectogram1st);
    QTube = new StableBridge2D(ProblemName, "QTube", 0.0, TubeType.Vectogram2nd);
    QTube1 = new StableBridge2D(ProblemName, "Q1Tube", 0.0, TubeType.Vectogram2nd);
    QTube2 = new StableBridge2D(ProblemName, "Q2Tube", 0.0, TubeType.Vectogram2nd);

    int qPartIdx = 0;
    ConvexPolygon cpMain = null!;
    if (QTypePart != 0) {
      cpMain = new ConvexPolygon(qVertices);
    }

    if (QTypePart == 1) {
      Debug.Assert(QAltParttitiong != null, nameof(QAltParttitiong) + " != null");
    }

    for (t = T; Tools.GE(t, t0); t -= dt) {
      Debug.Assert(pVertices != null, nameof(pVertices) + " != null");
      Debug.Assert(qVertices != null, nameof(qVertices) + " != null");
      PTube.Add(new TimeSection2D(t
      , new ConvexPolygon(pVertices.Select(pPoint => (Point2D)(-1.0 * D[t] * pPoint)).ToList(), true)));
      QTube.Add(new TimeSection2D(t
      , new ConvexPolygon(qVertices.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
      switch (QTypePart) {
        case 0:
          { }
          break;
        case 1:
          {
            (QInd1, QInd2) = QAltParttitiong!.GetAtCyclic(qPartIdx);
            (var cp1, var cp2) = cpMain.CutConvexPolygon(QInd1, QInd2);
            qVertices1 = Point.List2DTohD(cp1.Vertices);
            qVertices2 = Point.List2DTohD(cp2.Vertices);
            qPartIdx++;
          }
          break;
        case 2:
          {
            (var cp1, var cp2) = cpMain.CutConvexPolygon(QInd1, QInd2);
            qVertices1 = Point.List2DTohD(cp1.Vertices);
            qVertices2 = Point.List2DTohD(cp2.Vertices);
            QInd1 = (QInd1 + QStep) % qVertices.Count;
            QInd2 = (QInd2 + QStep) % qVertices.Count;
          }
          break;
      }

      Debug.Assert(qVertices1 != null, nameof(qVertices1) + " != null");
      Debug.Assert(qVertices2 != null, nameof(qVertices2) + " != null");
      QTube1.Add(new TimeSection2D(t
      , new ConvexPolygon(qVertices1.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
      QTube2.Add(new TimeSection2D(t
      , new ConvexPolygon(qVertices2.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
    }

    // Multiplication of the vectogram tubes by time step
    Ps = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in PTube) Ps[ts.t] = new ConvexPolygon(ts.section.Vertices.Select(pPoint => dt * pPoint));
    Qs = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube) Qs[ts.t] = new ConvexPolygon(ts.section.Vertices.Select(qPoint => dt * qPoint));
    Qs1 = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube1)
      Qs1[ts.t] = new ConvexPolygon(ts.section.Vertices.Select(qPoint => dt * qPoint));
    Qs2 = new SortedDictionary<double, ConvexPolygon>();
    foreach (TimeSection2D ts in QTube2)
      Qs2[ts.t] = new ConvexPolygon(ts.section.Vertices.Select(qPoint => dt * qPoint));
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
    int typeSetInt
    , dim;
    switch (set) {
      case 'p':
        {
          pref = "P";
          PTypeSet = pr.ReadInt("PTypeSet");
          typeSetInt = PTypeSet;
          dim = p;
        }
        break;
      case 'q':
        {
          pref = "Q";
          QTypeSet = pr.ReadInt("QTypeSet");
          typeSetInt = QTypeSet;
          dim = q;
        }
        break;
      case 'M':
        {
          pref = "M";
          projI = pr.ReadInt("projI");
          projJ = pr.ReadInt("projJ");
          MTypeSet = pr.ReadInt("MTypeSet");
          typeSetInt = MTypeSet;
          dim = 2;
        }
        break;
      default:
        throw new ArgumentException($"{set} must be 'p', 'q' or 'M'!");
    }

    TypeSet typeSet = typeSetInt switch
      {
        0 => TypeSet.Box, 1 => TypeSet.ListOfVert, 2 => TypeSet.RectParallel, 3 => TypeSet.RectTurned
      , 4 => TypeSet.Circle, 5 => TypeSet.Ellipse
      , _ => throw new ArgumentOutOfRangeException($"{typeSetInt} must be [1, 5]! And + '0' for P."),
      };


    if (set != 'p' && typeSet == TypeSet.Box) {
      throw new ArgumentException("TypeSet = -1 can be only in P-section!");
    }


    // Array for coordinates of the next point
    List<Point>? res = null;
    switch (typeSet) {
      //P-only
      case TypeSet.Box:
        {
          double[,] lims = pr.Read2DArray<double>("PBox", dim, 2);
          int pNum = (int)Math.Pow(2, dim);
          res = new List<Point>(pNum);
          for (int k = 0; k < pNum; k++) {
            var pCoord = new double[dim];
            int temp = k;
            for (int i = 0; i < dim; i++) {
              pCoord[i] = lims[i, temp % 2];
              temp /= 2;
            }

            res.Add(new Point(pCoord));
          }
        }
        break;
      case TypeSet.ListOfVert:
        {
          int Qnt = pr.ReadInt(pref + "Qnt");
          double[,] Vert = pr.Read2DArray<double>(pref + "Vert", Qnt, dim);
          res = Array2DToList(Vert, Qnt, dim);

          break;
        }
      case TypeSet.RectParallel:
        {
          double[] rect = pr.Read1DArray<double>(pref + "RectParallel", 4);
          res = Point.List2DTohD(PolygonTools.RectangleParallel(rect[0], rect[1], rect[2], rect[3]).Vertices);
          break;
        }
      case TypeSet.RectTurned:
        {
          double[] rect = pr.Read1DArray<double>(pref + "Rect", 4);
          res = Point.List2DTohD(PolygonTools
            .RectangleTurned(rect[0], rect[1], rect[2], rect[3], pr.ReadDouble(pref + "Angle")).Vertices);
          break;
        }
      case TypeSet.Circle:
        {
          double[] center = pr.Read1DArray<double>(pref + "Center", 2);
          res = Point.List2DTohD(PolygonTools.Circle(center[0], center[1], pr.ReadDouble(pref + "Radius")
          , pr.ReadInt(pref + "QntVert"), pr.ReadDouble(pref + "Angle")).Vertices);
          break;
        }
      case TypeSet.Ellipse:
        {
          double[] center = pr.Read1DArray<double>(pref + "Center", 2);
          double[] semi = pr.Read1DArray<double>(pref + "Semiaxes", 2);
          res = Point.List2DTohD(PolygonTools.Ellipse(center[0], center[1], semi[0], semi[1]
          , pr.ReadInt(pref + "QntVert"), pr.ReadDouble(pref + "Angle"), pr.ReadDouble(pref + "AngleAux")).Vertices);
          break;
        }
    }

    switch (set) {
      case 'p':
        pVertices = res ?? throw new InvalidOperationException("First players set is empty!");
        break;
      case 'q':
        qVertices = res ?? throw new InvalidOperationException("Second players set is empty!");
        break;
      case 'M':
        M = new ConvexPolygon(res ?? throw new InvalidOperationException("Terminal set is empty!"));
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
      case 0:
        {
          int Q1Qnt = pr.ReadInt("Q1Qnt");
          double[,] ar1 = pr.Read2DArray<double>("Q1Vert", Q1Qnt, q);
          qVertices1 = Array2DToList(ar1, Q1Qnt, q);
          int Q2Qnt = pr.ReadInt("Q2Qnt");
          double[,] ar2 = pr.Read2DArray<double>("Q2Vert", Q2Qnt, q);
          qVertices2 = Array2DToList(ar2, Q2Qnt, q);
        }
        break;
      case 1:
        {
          int qnt = pr.ReadInt("QK");
          int[,] ar = pr.Read2DArray<int>("QAltParttitiong", qnt, 2);
          var res = new List<(int, int)>();
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

          QAltParttitiong = res;
        }
        break;
      case 2:
        {
          QInd1 = pr.ReadInt("QInd1");
          QInd2 = pr.ReadInt("QInd2");
          QStep = pr.ReadInt("QStep");
          QIndexBorderCheck(QInd1, "Index1");
          QIndexBorderCheck(QInd2, "Index2");
          QIndexBorderCheck(QStep - 1, $"Step {QStep} must be from 1 to qVert.Count - 2 ... NOT READ NEXT ...");
        }
        break;
      default:
        throw new InvalidDataException("The QTypePart must be from 0 to 2!");
    }
  }


  /// <summary>
  /// Converts a two-dimensional array to a list of points.
  /// </summary>
  /// <param name="ar">The two-dimensional array to convert.</param>
  /// <param name="row">The number of rows in the array.</param>
  /// <param name="col">The number of columns in the array.</param>
  /// <returns>A list of points obtained from the two-dimensional array.</returns>
  private static List<Point> Array2DToList(double[,] ar, int row, int col) {
    var list = new List<Point>();
    for (int i = 0; i < row; i++) {
      var point = new double[col];
      for (int j = 0; j < col; j++) {
        point[j] = ar[i, j];
      }

      list.Add(new Point(point));
    }

    return list;
  }


  /// <summary>
  /// Aux, Is two indices is adjacent in a list 
  /// </summary>
  /// <param name="list">List</param>
  /// <param name="f">First index</param>
  /// <param name="s">Second index</param>
  /// <returns>True if adjacent. False otherwise</returns>
  private static bool IsAdjacent(ICollection list, int f, int s) {
    if (Math.Abs(f - s) < 2) return true;
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
      throw new InvalidEnumArgumentException(
        $"{mes} {id} in 'Q set partitioning' must be from 0 to {qVertices.Count - 1}!");
    }
  }

  #endregion
}