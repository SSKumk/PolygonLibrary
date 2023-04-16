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
  /// Type of the first player control constraint:
  ///   0 - a convex hull of a collection of points (now only for the case p = 2)
  ///   1 - box constraint
  ///   2 - circle; only for control dimension equal to 2; 
  ///   3 - ellipse; only for control dimension equal to 2; 
  /// </summary>
  public int pConstrType;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the first player
  /// </summary>
  public List<Point> pVertices = null!;

  // Flag showing whether to write vectograms of the first player;
  // The output file name is standard "pVectograms.bridge" with ContentType = "First player's vectorgram"
  public bool pWrite;

  /// <summary>
  /// Precomputed vectograms of the first player
  /// </summary>
  public readonly SortedDictionary<double, ConvexPolygon> Ps;

  /// <summary>
  /// Type of the second player control constraint:
  ///   0 - a convex hull of a collection of points (now only for the case p = 2)
  ///   1 - box constraint
  /// </summary>
  public int qConstrType;

  /// <summary>
  /// Collection of points, which convex hull defines the constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices = null!;

  /// <summary>
  /// Collection of points, which convex hull defines the first part of constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices1 = null!;

  /// <summary>
  /// Collection of points, which convex hull defines the second part of the constraint for the control of the second player
  /// </summary>
  public List<Point> qVertices2 = null!;

  // Flag showing whether to write vectograms of the second player;
  // The output file name is standard "pVectograms.bridge" with ContentType = "Second player's vectorgram"
  public bool qWrite;

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
  public readonly int typeSet;

  /// <summary>
  /// The terminal set
  /// </summary>
  public readonly ConvexPolygon M = null!;
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
  /// Reading and initializtion of problem data
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
    ReadConstraint(pr, 1, p);

    // Reading data on the second player's control and generating the constraint if necessary
    ReadConstraint(pr, 2, q); //todo Динамические разбиения Q 

    //Reading data of terminal set type
    projI = pr.ReadInt("projI");
    projJ = pr.ReadInt("projJ");

    typeSet = pr.ReadInt("typeSet");
    switch (typeSet) {
      case 0: {
        int       MQnt  = pr.ReadInt("MQnt");
        double[,] MVert = pr.Read2DArray<double>("MVert", MQnt, 2);

        List<Point2D> orig = new List<Point2D>(MQnt);
        for (int i = 0; i < MQnt; i++) {
          orig.Add(new Point2D(MVert[i, 0], MVert[i, 1]));
        }
        M = new ConvexPolygon(Convexification.ArcHull2D(orig));
        break;
      }
      case 1: {
        double[] rect = pr.Read1DArray<double>("MRectParallel", 4);
        M = PolygonTools.RectangleParallel(rect[0], rect[1], rect[2], rect[3]);
        break;
      }
      case 2: {
        double[] rect = pr.Read1DArray<double>("MRect", 4);
        M = PolygonTools.RectangleTurned(rect[0], rect[1], rect[2], rect[3], pr.ReadDouble("MAngle"));
        break;
      }
      case 3: {
        double[] center = pr.Read1DArray<double>("MCenter", 2);
        M = PolygonTools.Circle(center[0], center[1], pr.ReadDouble("MRadius"), pr.ReadInt("MQntVert")
                              , pr.ReadDouble("MAngle"));
        break;
      }
      case 4: {
        double[] center = pr.Read1DArray<double>("MCenter", 2);
        double[] semi   = pr.Read1DArray<double>("MSemiaxes", 2);
        M = PolygonTools.Ellipse(center[0], center[1], semi[0], semi[1], pr.ReadInt("MQntVert"), pr.ReadDouble("MAngle")
                               , pr.ReadDouble("MAngleAux"));

        break;
      }
    }


    if (M is null) { throw new NullReferenceException("The terminal set is empty!"); }


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

    StableBridge2D PTube  = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram1st)
                 , QTube  = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram2nd)
                 , QTube1 = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram2nd)
                 , QTube2 = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram2nd);

    for (t = T; Tools.GE(t, t0); t -= dt) {
      Debug.Assert(pVertices != null, nameof(pVertices) + " != null");
      Debug.Assert(qVertices != null, nameof(qVertices) + " != null");
      Debug.Assert(qVertices1 != null, nameof(qVertices1) + " != null");
      Debug.Assert(qVertices2 != null, nameof(qVertices2) + " != null");

      PTube.Add(new
                  TimeSection2D(t, new ConvexPolygon(pVertices.Select(pPoint => (Point2D)(-1.0 * D[t] * pPoint)).ToList(), true)));
      QTube.Add(new
                  TimeSection2D(t, new ConvexPolygon(qVertices.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
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

  /// <summary>
  /// Method that read and generates 
  /// </summary>
  /// <param name="pr">The parameter reader objects</param>
  /// <param name="plNum">Number of the player</param>
  /// <param name="dim">Dimension of the player's control</param>
  private void ReadConstraint(ParamReader pr, int plNum, int dim) {
    string pref       = plNum == 1 ? "p" : "q";
    string number     = plNum == 1 ? "first" : "second";
    int    ConstrType = pr.ReadInt(pref + "ConstrType");
    List<Point> res = new List<Point>
        { };
    List<Point> qres1 = new List<Point>
        { };
    List<Point> qres2 = new List<Point>
        { };

    // Data for circle and elliptic constraint: coordinates of the center
    double x0;
    double y0;

    // Data for circle constraints: radius of the circle
    double R;

    // Data for circle and elliptic constraints: angle of turn of the initial vertex
    double Alpha0;

    // Data for elliptic constraints: semiaxes of the ellipse and angle of turn of the semiaxis a
    double a;
    double b;
    double Phi;

    // Angle step for circle and elliptic constraints
    double da;

    // Array for coordinates of the next point
    switch (ConstrType) {
      case 0: // Just a convex hull of points in the plane
        if (dim != 2)
          throw new Exception("Reading game data: the " + number +
                              " player's constraint is a convex hull of a collection of points, but the dimension of the control is greater than 2!");
        res = FillArrayCH(pr, pref);
        if (pref == "q") {
          qres1 = FillArrayCH(pr, "q", "1");
          qres2 = FillArrayCH(pr, "q", "2");
        }
        break;

      case 1: // Box constraint
        res = FillArrayBox(pr, dim, pref);
        if (pref == "q") {
          qres1 = FillArrayBox(pr, dim, "q", "1");
          qres2 = FillArrayBox(pr, dim, "q", "2");
        }

        break;
    }

    bool Write = pr.ReadBoolean(pref + "Write");

    if (plNum == 1) {
      pConstrType = ConstrType;
      pVertices   = res;
      pWrite      = Write;
    } else {
      qConstrType = ConstrType;
      qVertices   = res;
      qVertices1  = qres1;
      qVertices2  = qres2;
      qWrite      = Write;
    }
  }

  private static List<Point> FillArrayCH(ParamReader pr, string pref, string indx = "") {
    int       Vqnt   = pr.ReadInt(pref + "Vqnt" + indx);
    double[,] coords = pr.Read2DArray<double>(pref + "Vertices" + indx, Vqnt, 2);
    var       psOrig = new List<Point2D>(Vqnt);
    for (int i = 0; i < Vqnt; i++)
      psOrig.Add(new Point2D(coords[i, 0], coords[i, 1]));
    List<Point2D> ps  = Convexification.ArcHull2D(psOrig);
    List<Point>   res = new(ps.Count);
    foreach (Point2D p in ps)
      res.Add(new Point(p));
    return res;
  }

  private static List<Point> FillArrayBox(ParamReader pr, int dim, string pref, string indx = "") {
    // Array for coordinates of the next point
    double[,]   lims = pr.Read2DArray<double>(pref + "Box" + indx, dim, 2);
    int         pNum = (int)Math.Pow(2, dim);
    List<Point> res  = new(pNum);
    for (int k = 0; k < pNum; k++) {
      var pCoord = new double[dim];
      int temp   = k;
      for (int i = 0; i < dim; i++) {
        pCoord[i] =  lims[i, temp % 2];
        temp      /= 2;
      }

      res.Add(new Point(pCoord));
    }
    return res;
  }
#endregion

}
