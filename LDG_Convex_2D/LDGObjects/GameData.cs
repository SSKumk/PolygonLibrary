using System.Text;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;
using ParamReaderLibrary;

namespace LDGObjects {
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

   ,

    /// <summary>
    /// This exemplar of GameData is initialized for computation of switching surfaces.
    /// All files of switching surfaces and motions should be deleted 
    /// </summary>
    SwitchingSurfaces

   ,

    /// <summary>
    /// This exemplar of GameData is initialized for computation of singular surfaces.
    /// All files of singular surfaces and motions should be deleted 
    /// </summary>
    SingularSurfaces

   ,

    /// <summary>
    /// This exemplar of GameData is initialized for computation of switching surfaces.
    /// All files of motions should be deleted 
    /// </summary>
    Motions

   ,

    /// <summary>
    /// This exemplar of GameData is initialized for some other computation 
    /// No files of results should be deleted 
    /// </summary>
    Other
  }

  /// <summary>
  /// Class for keeping game parameter data
  /// </summary>
  public class GameData {
    #region Information about the initial clearance of the problem folder

    /// <summary>
    /// Extensions of files with certain objects (correspond to the ComputationType enumeration)
    /// </summary>
    static public SortedDictionary<ComputationType, string> Extensions = new SortedDictionary<ComputationType, string>()
      {
        { ComputationType.StableBridge, "bridge" }
      , { ComputationType.SwitchingSurfaces, "swsurf" }
      , { ComputationType.SingularSurfaces, "singsurf" }
      , { ComputationType.Motions, "motion" }
      };

    /// <summary>
    /// Information what types of files to delete during some certain computation
    /// </summary>
    static protected SortedDictionary<ComputationType, List<ComputationType>> toDelete =
      new SortedDictionary<ComputationType, List<ComputationType>>()
        {
          // what to delete when stable bridges are copmuted
          {
            ComputationType.StableBridge, new List<ComputationType>()
              {
                ComputationType.StableBridge
              , ComputationType.SwitchingSurfaces
              , ComputationType.SingularSurfaces
              , ComputationType.Motions
              }
          }
         ,
          // what to delete when switching surfaces are computed
          {
            ComputationType.SwitchingSurfaces, new List<ComputationType>()
              {
                ComputationType.SwitchingSurfaces
              , ComputationType.Motions
              }
          }
         ,
          // what to delete when singular surfaces are computed 
          {
            ComputationType.SingularSurfaces, new List<ComputationType>()
              {
                ComputationType.SingularSurfaces
              , ComputationType.Motions
              }
          }
         ,
          // what to delete when motions are computed
          {
            ComputationType.Motions, new List<ComputationType>()
                { }
          }
         ,
          // what to delete when other computations are performed
          {
            ComputationType.Other, new List<ComputationType>()
                { }
          }
        };

    #endregion

    #region Input data

    /// <summary>
    /// Name of the problem; to be written in all resultant files for checking their consistency
    /// </summary> 
    public string ProblemName;

    /// <summary>
    /// Short name of the problem; to be used in SharpEye
    /// </summary>
    public string ShortProblemName;

    /// <summary>
    /// Path to the folder whereto the result should be written
    /// </summary> 
    public string path;

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
    public int p;

    /// <summary>
    /// The useful control matrix
    /// </summary>
    public Matrix B;

    /// <summary>
    /// Dimension of the disturbance
    /// </summary>
    public int q;

    /// <summary>
    /// The disturbance matrix
    /// </summary>
    public Matrix C;

    /// <summary>
    /// The initial instant
    /// </summary>
    public double t0;

    /// <summary>
    /// The final instant
    /// </summary>
    public double T;

    /// <summary>
    /// The time step
    /// </summary>
    public double dt;

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
    /// Collection of points, which convex hull defins the constraint for the control of the first player
    /// </summary>
    public List<Point> pVertices;

    // Flag showing whether to write vectograms of the first player;
    // The output file name is standard "pVectograms.bridge" with ContentType = "First player's vectorgram"
    public bool pWrite;

    /// <summary>
    /// Precomputed vectograms of the first player
    /// </summary>
    public SortedDictionary<double, ConvexPolygon> Ps;

    /// <summary>
    /// Type of the second player control constraint:
    ///   0 - a convex hull of a collection of points (now only for the case p = 2)
    ///   1 - box constraint
    ///   2 - circle; only for control dimension equal to 2; 
    ///   3 - ellipse; only for control dimension equal to 2; 
    /// </summary>
    public int qConstrType;

    /// <summary>
    /// Collection of points, which convex hull defins the constraint for the control of the second player
    /// </summary>
    public List<Point> qVertices;

    // Flag showing whether to write vectograms of the second player;
    // The output file name is standard "pVectograms.bridge" with ContentType = "Second player's vectorgram"
    public bool qWrite;

    /// <summary>
    /// Precomputed vectograms of the first player
    /// </summary>
    public SortedDictionary<double, ConvexPolygon> Qs;

    #endregion

    #region Data defining the payoff function

    /// <summary>
    /// Type of the payoff:
    ///   0 - distance to a given point in some given subspace;  
    ///       this payoff is described by integer variables payI, payJ defining the indices 
    ///       of the subspace coordinates and double variables payX, payY defining the coordinates
    ///       of the ponit
    ///   1 - Minkowski functional of a convex set in some given subspace         
    ///       this payoff is described by integer variables payI, payJ defining the indices 
    ///       of the subspace coordinates, an integer variable payVqnt - number of points, which convex hull 
    ///       defines the set, and payVqnt x 2 double array payVertices - these points
    ///   2 - distance to a convex set in some given subspace         
    ///       this payoff is described by integer variables payI, payJ defining the indices 
    ///       of the subspace coordinates, an integer variable payVqnt - number of points, which convex hull 
    ///       defines the set, and payVqnt x 2 double array payVertices - these points
    /// </summary>
    public int payoffType;

    /// <summary>
    /// The index of the first of two coordinates defining the payoff function (counted from zero)
    /// </summary>
    public int payI;

    /// <summary>
    /// The index of the second of two coordinates defining the payoff function (counted from zero)
    /// </summary>
    public int payJ;

    /// <summary>
    /// Data for the case of distance to a point - the first coordinate of the point
    /// </summary>
    public double payX;

    /// <summary>
    /// Data for the case of distance to a point - the second coordinate of the point
    /// </summary>
    public double payY;

    /// <summary>
    /// Number of vertices in the approximation of circles in the case of distance to a point payoff
    /// </summary>
    public int payVqnt;

    /// <summary>
    /// Data for the case of Minkowski functional or distance to a set: coordinates of vertices sof the target set
    /// </summary>
    public List<Point2D> payVertices;

    /// <summary>
    /// Polygon of the set for the case of the payoff taken as the distance to a set
    /// </summary>
    private ConvexPolygon _basic = null;

    /// <summary>
    /// Lazy property for the pPolygon of the set for the case of the payoff 
    /// taken as the distance to a set
    /// </summary>
    private ConvexPolygon basic {
      get {
        if (_basic == null) _basic = new ConvexPolygon(payVertices);
        return _basic;
      }
    }

    /// <summary>
    /// Method for generating a level set of the payoff corresponding to the given value
    /// </summary>
    /// <param name="c">The value of the payoff</param>
    /// <param name="gridNum">In the case of distance to a set, number of vertices in approximations of arcs</param>
    /// <returns>The corresponding convex polygon</returns>
    public ConvexPolygon PayoffLevelSet(double cOrig, int gridNum = 20) {
      double c = cOrig >= 0 ? cOrig : 0;
      switch (payoffType) {
        case 0:
          return PolygonTools.Circle(payX, payY, c, payVqnt);

        case 1:
          return new ConvexPolygon(payVertices.Select(p => c * p), true);

        case 2:
          if (gridNum < 1)
            throw new Exception(
              "Internal: generating level set of the payoff as a distance to a set - gridNum less than 1");

          if (Tools.EQ(c)) return basic;

          List<GammaPair> gps = new List<GammaPair>();
          int i
          , j
          , k;
          double da;
          for (i = 0, j = 1; i < basic.Contour.Count; i++, j = (j + 1) % basic.Contour.Count) {
            da = basic.SF[j].Normal.PolarAngle - basic.SF[i].Normal.PolarAngle;
            if (Tools.LT(da)) da += 2 * Math.PI;
            da /= gridNum;

            Vector2D vert = (Vector2D)GammaPair.CrossPairs(basic.SF[i], basic.SF[j]);

            for (k = 0; k < gridNum; k++) {
              Vector2D norm = basic.SF[i].Normal.Turn(k * da);
              double val = norm * vert + c;
              gps.Add(new GammaPair(norm, val));
            }
          }

          return new ConvexPolygon(new SupportFunction(gps));

        default:
          throw new Exception("Internal: generating level set of the payoff for unknown type of the payoff");
      }
    }

    // The array of values of the payoff to compute bridges
    public List<double> cValues;

    #endregion

    /// <summary>
    /// The fundamental Cauchy matrix of the corresponding system
    /// </summary>
    public CauchyMatrix cauchyMatrix;

    /// <summary>
    /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
    /// </summary>
    public Matrix ProjMatr;

    /// <summary>
    /// Collection of matrices D for the instants from the time grid
    /// </summary>
    public SortedDictionary<double, Matrix> D;

    /// <summary>
    /// Collection of matrices E for the instants from the time grid
    /// </summary>
    public SortedDictionary<double, Matrix> E;

    #endregion

    #region Constructor

    /// <summary>
    /// Reading and initializtion of problem data
    /// </summary>
    /// <param name="inFName">File with the data</param>
    /// <param name="compObj">Computation type (to delete correctly files of previous computations)</param>
    public GameData(string inFName, ComputationType compObj) {
      ParamReader pr = new ParamReader(inFName);

      ProblemName = pr.ReadString("ProblemName");
      ShortProblemName = pr.ReadString("ShortProblemName");
      path = pr.ReadString("path");

      // Creation or clearing the problem folder
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      else {
        // Clearing old files
        foreach (ComputationType delObj in toDelete[compObj]) {
          string[] files = Directory.GetFiles(path, "*" + Extensions[delObj]);
          foreach (string file in files) File.Delete(file);
        }
      }

      if (path[^1] == '\\') {
        StringBuilder sb = new StringBuilder(path);
        sb[^1] = '/';
        path = sb.ToString();
      } else if (path[^1] != '/') {
        path += '/';
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
      ReadConstraint(pr, 1, p);

      // Reading data on the second player's control and generating the constraint if necessary
      ReadConstraint(pr, 2, q);

      // Payoff
      payoffType = pr.ReadInt("payoffType");
      payI = pr.ReadInt("payI");
      payJ = pr.ReadInt("payJ");

      switch (payoffType) {
        case 0: // Distance to a point
          payX = pr.ReadDouble("payX");
          payY = pr.ReadDouble("payY");
          payVqnt = pr.ReadInt("payVqnt");
          break;

        case 1:
        case 2:
          payVqnt = pr.ReadInt("payVqnt");
          double[,] rawCoord = pr.Read2DArray<double>("payVertices", payVqnt, 2);
          List<Point2D> psOrig = new List<Point2D>(payVqnt);
          for (int i = 0; i < payVqnt; i++) psOrig.Add(new Point2D(rawCoord[i, 0], rawCoord[i, 1]));
          payVertices = Convexification.ArcHull2D(psOrig);
          break;
      }

      // C grid
      int cGridType = pr.ReadInt("cGridType")
      , cQnt = pr.ReadInt("cQnt");
      if (cGridType == 0) {
        if (cQnt == 1)
          throw new Exception("Reading game data: in the case of uniform grid there cannot be one point only");

        double cMin = pr.ReadDouble("cMin")
        , cMax = pr.ReadDouble("cMax")
        , dc = (cMax - cMin) / (cQnt - 1);

        cValues = new List<double>(cQnt);

        for (int i = 0; i < cQnt; i++) cValues.Add(cMin + i * dc);
      } else
        cValues = new List<double>(pr.Read1DArray<double>("cValues", cQnt));

      // The projection matrix
      double[,] ProjMatrArr = new double[2, n];
      ProjMatrArr[0, payI] = 1.0;
      ProjMatrArr[1, payJ] = 1.0;
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

      StableBridge2D PTube = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram1st)
      , QTube = new StableBridge2D(ProblemName, ShortProblemName, 0.0, TubeType.Vectogram2nd);

      // Precomputing the players' vectorgrams 
      for (t = T; Tools.GE(t, t0); t -= dt) {
        PTube.Add(new TimeSection2D(t
        , new ConvexPolygon(pVertices.Select(pPoint => (Point2D)((-1.0) * D[t] * pPoint)).ToList(), true)));
        QTube.Add(new TimeSection2D(t
        , new ConvexPolygon(qVertices.Select(qPoint => (Point2D)(E[t] * qPoint)).ToList(), true)));
      }

      // Writing the players' vectorgram if necessary
      if (pWrite) {
        StreamWriter sw = new StreamWriter(path + "pVectograms.bridge");
        PTube.WriteToFile(sw);
        sw.Close();
      }

      if (qWrite) {
        StreamWriter sw = new StreamWriter(path + "qVectograms.bridge");
        QTube.WriteToFile(sw);
        sw.Close();
      }

      // Multiplication of the vectogram tubes by time step
      Ps = new SortedDictionary<double, ConvexPolygon>();
      foreach (TimeSection2D ts in PTube)
        Ps[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(pPoint => dt * pPoint));
      Qs = new SortedDictionary<double, ConvexPolygon>();
      foreach (TimeSection2D ts in QTube)
        Qs[ts.t] = new ConvexPolygon(ts.section.Contour.Vertices.Select(qPoint => dt * qPoint));
    }

    /// <summary>
    /// Method that read and generates 
    /// </summary>
    /// <param name="pr">The parameter reader objects</param>
    /// <param name="plNum">Number of the player</param>
    /// <param name="dim">Dimension of the player's control</param>
    private void ReadConstraint(ParamReader pr, int plNum, int dim) {
      string pref = plNum == 1 ? "p" : "q"
      , number = plNum == 1 ? "first" : "second";
      int ConstrType = pr.ReadInt(pref + "ConstrType")
      , Vqnt;
      List<Point> res = null;

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
      double[] pCoord;

      switch (ConstrType) {
        case 0: // Just a convex hull of points in the plane
          if (dim != 2)
            throw new Exception("Reading game data: the " + number +
                                " player's constraint is a convex hull of a collection of points, but the dimension of the control is greater than 2!");
          Vqnt = pr.ReadInt(pref + "Vqnt");
          double[,] coords = pr.Read2DArray<double>(pref + "Vertices", Vqnt, 2);
          List<Point2D> psOrig = new List<Point2D>(Vqnt);
          for (int i = 0; i < Vqnt; i++) psOrig.Add(new Point2D(coords[i, 0], coords[i, 1]));
          List<Point2D> ps = Convexification.ArcHull2D(psOrig);
          res = new List<Point>(ps.Count);
          foreach (Point2D p in ps) res.Add(new Point(p));
          break;

        case 1: // Box constraint
          double[,] lims = pr.Read2DArray<double>(pref + "Box", dim, 2);
          int pNum = (int)Math.Pow(2, dim)
          , temp;
          res = new List<Point>(pNum);
          for (int k = 0; k < pNum; k++) {
            pCoord = new double[dim];
            temp = k;
            for (int i = 0; i < dim; i++) {
              pCoord[i] = lims[i, (int)(temp % 2)];
              temp /= 2;
            }

            res.Add(new Point(pCoord));
          }

          break;

        case 2: // Circle
          if (dim != 2)
            throw new Exception("Reading game data: the " + number +
                                " player's constraint is a circle, but the dimension of the control is greater than 2!");
          Vqnt = pr.ReadInt(pref + "Vqnt");
          x0 = pr.ReadDouble(pref + "x0");
          y0 = pr.ReadDouble(pref + "y0");
          R = pr.ReadDouble(pref + "R");
          Alpha0 = pr.ReadDouble(pref + "Alpha0");

          res = new List<Point>(Vqnt);

          da = 2 * Math.PI / Vqnt;
          for (int i = 0; i < Vqnt; i++) {
            pCoord = new double[2]
              {
                x0 + R * Math.Cos(Alpha0 + i * da)
              , y0 + R * Math.Sin(Alpha0 + i * da)
              };
            res.Add(new Point(pCoord));
          }

          break;

        case 3: // Ellipse
          if (dim != 2)
            throw new Exception("Reading game data: the " + number +
                                " player's constraint is an ellipse, but the dimension of the control is greater than 2!");
          Vqnt = pr.ReadInt(pref + "Vqnt");
          x0 = pr.ReadDouble(pref + "x0");
          y0 = pr.ReadDouble(pref + "y0");
          a = pr.ReadDouble(pref + "a");
          b = pr.ReadDouble(pref + "b");
          Phi = pr.ReadDouble(pref + "Phi");
          Alpha0 = pr.ReadDouble(pref + "Alpha0");

          res = new List<Point>(Vqnt);

          double cPhi = Math.Cos(Phi)
          , sPhi = Math.Sin(Phi);

          da = 2 * Math.PI / Vqnt;
          for (int i = 0; i < Vqnt; i++) {
            double x = a * Math.Cos(Alpha0 + i * da)
            , y = b * Math.Sin(Alpha0 + i * da)
            , x1 = x * cPhi - y * sPhi
            , y1 = x * sPhi + y * cPhi;
            pCoord = new double[2]
              {
                x1
              , y1
              };
            res.Add(new Point(pCoord));
          }

          break;
      }

      bool Write = pr.ReadBoolean(pref + "Write");

      if (plNum == 1) {
        pConstrType = ConstrType;
        pVertices = res;
        pWrite = Write;
      } else {
        qConstrType = ConstrType;
        qVertices = res;
        qWrite = Write;
      }
    }

    #endregion
  }
}