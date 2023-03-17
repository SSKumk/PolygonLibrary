using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

using ParamReaderLibrary;

using AVLUtils;

namespace LDGObjects
{
  /// <summary>
  /// Element of a switching lines along the boundary of stable bridge: 
  /// time instant and two pairs of points (which can coincide inside pairs) 
  /// describing the local indifference zones. In each pair, the first point 
  /// has less dot product with the control vector than the second one.
  /// The points of first pair has less dot product with the control vector 
  /// turned orthogonally clockwise than the points of the second one
  /// </summary>
  public class SwitchingLineElement : 
    IEquatable<SwitchingLineElement>, IComparable<SwitchingLineElement>
  {
    #region Data of the element
    /// <summary>
    /// Payoff value, to which this element corresponds
    /// </summary>
    public double c;

    /// <summary>
    /// The first point of the zone (giving less dot product with the control vector)
    /// </summary>
    public Point2D p1 { get; protected set; }

    /// <summary>
    /// The second point of the zone (giving greater dot product with the control vector)
    /// </summary>
    public Point2D p2 { get; protected set; }

    /// <summary>
    /// Reference to the parent switching line
    /// </summary>
    public SwitchingLine line { get; protected set; }

    /// <summary>
    /// Dot product of the first point from the indifference zone with the control vector
    /// </summary>
    public double val1 { get; protected set; }

    /// <summary>
    /// Dot product of the second point from the indifference zone with the control vector
    /// </summary>
    public double val2  { get; protected set; }

    /// <summary>
    /// Dot product of a point from the indifference zone with the control vector 
    /// turned orthogonally clockwise
    /// </summary>
    public double valOrder  { get; protected set; }
    #endregion

    #region Interface implementations
    /// <summary>
    /// Check whether two switching line elements are equal (in the sence of their order
    /// along the control vector turned orthogonally clockwise)
    /// </summary>
    /// <param name="other">The element to be checked with</param>
    /// <returns>true, if equal; false, otherwise</returns>
    public bool Equals(SwitchingLineElement other)
    {
      return Tools.EQ(this.valOrder, other.valOrder);
    }

    /// <summary>
    /// Compare two switching line elements (in the sence of their order
    /// along the control vector turned orthogonally clockwise)
    /// </summary>
    /// <param name="other">The element to be compared with</param>
    /// <returns>
    /// -1, if this element is less than the other; 
    /// 0, if the elements are equal;
    /// +1, if this element is greater than the other
    /// </returns>
    public int CompareTo(SwitchingLineElement? other)
    {
      return Tools.CMP(this.valOrder, other.valOrder);
    }
    #endregion

    #region Constructor
    
    /// <summary>
    /// Two point constructor
    /// </summary>
    /// <param name="line">The line for taking the control vector and the vector defining the order along the line</param>
    /// <param name="cVal">The payoff value to which the element corresponds</param>
    /// <param name="p1">One boundary point of the indifference zone</param>
    /// <param name="p2">Other boundary point of the indifference zone (can be null if points coincide)</param>
    public SwitchingLineElement(SwitchingLine line, double cVal, Point2D p1, Point2D? p2 = null)
    {
      this.line = line;
      c = cVal;

      Vector2D v1 = (Vector2D)p1, v2 = null;
			valOrder = this.line.orderVector * v1;

      // Duplicating points if coincide and, if do not, right order inside groups
      if (p2 is null)
      {
        p2 = p1;
				val1 = val2 = v1 * this.line.controlVector;
      }
      else
      {
        v2  = (Vector2D)p2;
        if (Tools.GT(v1 * this.line.controlVector, v2 * this.line.controlVector))
        {
					(p1, p2) = (p2, p1);
          (v1, v2) = (v2, v1);
        }
				val1 = v1 * this.line.controlVector;
				val2 = v2 * this.line.controlVector;
      }
      this.p1 = p1;
      this.p2 = p2;
    }
    #endregion

    /// <summary>
    /// Method that determines at what side from the switching line  a given point is located
    /// with respect to the control vector of the line. It is assumed that the result
    /// will be used is just with the control vector of the line
    /// </summary>
    /// <param name="p">The point, which location should be determined</param>
    /// <returns>
    /// +1, if the ray starting from the point and oriented along the control vector intersects the line
    /// 0, if the point is in the indifference zone,
    /// -1, if the ray starting from the point and oriented along the control vector does not intersect the line
    /// </returns>
    public int GetSide(Point2D p)
    {
      double pVal = (Vector2D)p * line.controlVector;
      if (Tools.LT(pVal, val1))
        return +1;
      else if (Tools.GT(pVal, val2))
        return -1;
      else
        return 0;
    }
  }

  /// <summary>
  /// Class of a switching line
  /// </summary>
  public class SwitchingLine
  {
    #region Storages and accessors
    /// <summary>
    /// Time instant of the line
    /// </summary>
    public double t { get; protected set; }

    /// <summary>
    /// Elements of the line ordered along the order vector of the line 
    /// </summary>
    private AVLSet<SwitchingLineElement> _elems;

    /// <summary>
    /// Number of elements in the line
    /// </summary>
    public int Count { get { return _elems.Count; } }

    /// <summary>
    /// Get an element of the line by its index
    /// </summary>
    /// <param name="i">The index of the desired element</param>
    /// <returns>The desired element</returns>
    public SwitchingLineElement this[int i] { get { return _elems[i]; } }

    /// <summary>
    /// Add new two point element
    /// </summary>
    /// <param name="cVal">The value of c</param>
    /// <param name="p1">One point of the new element</param>
    /// <param name="p2">Other point of the new element (null if the points coincide)</param>
    public void AddElement(double cVal, Point2D p1, Point2D p2 = null)
    {
      _elems.Add(new SwitchingLineElement(this, cVal, p1, p2));
    }

    /// <summary>
    /// Vector along the vectogram of the player is located.
    /// Defines the positive and negative sides of the line
    /// </summary>
    public Vector2D controlVector { get; protected set; }

    /// <summary>
    /// Vector orthogonal to the control vector.
    /// Defines the order of elemens along the line
    /// </summary>
    public Vector2D orderVector { get; protected set; }
    #endregion

    #region Constructors
    /// <summary>
    /// The default constructor
    /// </summary>
    /// <param name="nt">The time instant of the line</param>
    /// <param name="contrVect">The control vector of the line</param>
    public SwitchingLine(double nt, Vector2D contrVect) 
    {
      t = nt;
      controlVector = contrVect;
      orderVector = controlVector.TurnCW().Normalize();
      _elems = new AVLSet<SwitchingLineElement>();
    }

    /// <summary>
    /// Constructor that reads the switching line from a parameter reader 
    /// in the format compatible with the one, in which a line is written
    /// </summary>
    /// <param name="pr">The parameter reader, wherefrom the line should be read</param>
    public SwitchingLine(ParamReader pr)
    {
      t = pr.ReadDouble("t");
      double[] coords = pr.Read1DArray<double>("controlVector", 2);
      controlVector = new Vector2D(coords[0], coords[1]);
      orderVector = controlVector.Normalize().TurnCW();
      int elemQnt = pr.ReadInt("elemQnt");
      _elems = new AVLSet<SwitchingLineElement>();
      for (int i = 0; i < elemQnt; i++)
      {
        double cVal = pr.ReadDouble("cVal");
        double[,] cs = pr.Read2DArray<double>("element", 2, 2);
        _elems.Add(new SwitchingLineElement(this, cVal,
          new Point2D(cs[0, 0], cs[0, 1]), new Point2D(cs[1, 0], cs[1, 1])));
      }
    }
    #endregion

    ///   //-------------------------------------------------------
    ///   t = ...;
    ///   controlVector = { ..., ... };
    ///   elemQnt = ...;
    ///   cVal = ...;
    ///   element = { {..., ...}, {..., ...} };
    ///   cVal = ...;
    ///   element = { {..., ...}, {..., ...} };
    ///   ............
    ///   //-------------------------------------------------------
    public void WriteToFile(StreamWriter sw)
    {
      sw.WriteLine("//-------------------------------------------------------");
      sw.WriteLine("t = " + t + ";");
      sw.WriteLine("controlVector = { " + controlVector.x + " , " + controlVector.y + " };");
      sw.WriteLine("elemQnt = " + Count + ";");
      foreach (SwitchingLineElement elem in _elems)
      {
        sw.WriteLine("cVal = " + elem.c + ";");
        sw.WriteLine("element = { " +
          "{ " + elem.p1.x + ", " + elem.p1.y + " }, " +
          "{ " + elem.p2.x + ", " + elem.p2.y + " } };");
      }
      sw.WriteLine("//-------------------------------------------------------");
    }

    /// <summary>
    /// Method that determines at what side from the switching line a given point is located
    /// with respect to the control vector of the line. It is assumed that the result
    /// will be used is just with the control vector of the line
    /// </summary>
    /// <param name="p">The point, which location should be determined</param>
    /// <returns>
    /// +1, if the ray starting from the point and oriented along the control vector intersects the line
    /// 0, if the point is in the indifference zone,
    /// -1, if the ray starting from the point and oriented along the control vector does not intersect the line
    /// </returns>
    public int GetSide(Point2D p)
    {
      SwitchingLineElement pElem = new SwitchingLineElement(this, 0.0, p);

      // The point is located before or at the minimal element of the line.
      // Generate control as if the point is located in the straight line
      // passing through the minimal element of the line
      SwitchingLineElement minElem = _elems.Min();
      if (pElem.CompareTo(minElem) <= 0) 
        return minElem.GetSide(p);

      // The point is located after the maximal element of the line.
      // Generate control as if the point is located in the straight line
      // passing through the maximal element of the line
      SwitchingLineElement maxElem = _elems.Max();
      if (pElem.CompareTo(maxElem) > 0) 
        return maxElem.GetSide(p);

      // Search for the line
      IEnumerator<SwitchingLineElement> it = _elems.GetEnumerator(pElem);
      SwitchingLineElement nextElem = it.Current, prevElem;
      _elems.CyclicPrev(nextElem, out prevElem);

      double
        alpha = (pElem.valOrder - nextElem.valOrder) / (prevElem.valOrder - nextElem.valOrder),
        pVal1 = alpha * prevElem.val1 + (1 - alpha) * nextElem.val1,
        pVal2 = alpha * prevElem.val2 + (1 - alpha) * nextElem.val2;

      if (Tools.LT(pElem.val1, pVal1))
        return +1;
      else if (Tools.GT(pElem.val1, pVal2))
        return -1;
      else
        return 0;
    }
  }

  /// <summary>
  /// Class describing a switching surface
  /// </summary>
  public class SwitchingSurface : SortedDictionary<double, SwitchingLine>
  {
    #region Surface data
    /// <summary>
    /// Full name of the problem
    /// </summary>
    public string ProblemName { get; protected set; }

    /// <summary>
    /// Short name of the problem
    /// </summary>
    public string ShortProblemName { get; protected set; }

    /// <summary>
    /// Number of the player (1 or 2)
    /// </summary>
    public int PlayerNum { get; protected set; }

    /// <summary>
    /// Number of component of the control (counted from 0), to which the switching surface is computed
    /// </summary>
    public int ControlCompNum { get; protected set; }
    #endregion

    #region Constructors
    /// <summary>
    /// The basic constructor
    /// </summary>
    /// <param name="problemName">Full name of the problem, for which the surface is computed</param>
    /// <param name="shortProblemName">Short name of the problem, for which the surface is computed</param>
    /// <param name="player">Number of the player</param>
    /// <param name="component"></param>
    public SwitchingSurface(string problemName, string shortProblemName, int player, int component)
      : base(new Tools.DoubleComparer(Tools.Eps))
    {
      if (player != 1 && player != 2)
        throw new Exception("Constructing an empty switching surface: wrong player number " + player);
      if (component < 0)
        throw new Exception("Constructing an empty switching surface: wrong control component number " + component);

      ProblemName = problemName;
      ShortProblemName = shortProblemName;
      PlayerNum = player;
      ControlCompNum = component;
    }

		/// <summary>
		/// The constructor that reads the switching surface from a given file
		/// (assuming the file has correct format)
		/// </summary>
		public SwitchingSurface(string fileName) : this(new ParamReader(fileName)) { }

    /// <summary>
    /// The constructor that reads the switching surface from a given paramater reader
    /// </summary>
    /// <param name="pr">The parameter reader, wherefrom the surface to be read</param>
    public SwitchingSurface(ParamReader pr)
      : base(new Tools.DoubleComparer(Tools.Eps))
    {
      ProblemName = pr.ReadString("ProblemName");
      ShortProblemName = pr.ReadString("ShortProblemName");
      string player = pr.ReadString("Player");
      if (player == "First")
        PlayerNum = 1;
      else
        PlayerNum = 2;
      ControlCompNum = pr.ReadInt("ControlComponentNum");

      int instantQnt = pr.ReadInt("instantQnt");

      for (int i = 0; i < instantQnt; i++)
      {
        SwitchingLine swLine = new SwitchingLine(pr);
        this[swLine.t] = swLine;
      }
    }
    #endregion

    /// <summary>
    /// Write the surface into a file. The format is the following:
    ///   ProblemName = "...";
    ///   ShortProblemName = "...";
    ///   Player = "First" / "Second";
    ///   ControlComponentNum = ...;
    ///   
    ///   instantQnt = ...;
    ///   
    ///   SwitchingLine
    ///   ............
    ///   SwitchingLine
    /// </summary>
    /// <param name="sw">Stream whereto write the data</param>
    public void WriteToFile(StreamWriter sw)
    {
      sw.WriteLine("ProblemName = \"" + ProblemName + "\";");
      sw.WriteLine("ShortProblemName = \"" + ShortProblemName + "\";");
      sw.WriteLine("Player = \"" + (PlayerNum == 1 ? "First" : "Second") + "\";");
      sw.WriteLine("ControlComponentNum = " + ControlCompNum + ";");
      sw.WriteLine("");
      sw.WriteLine("instantQnt = " + Count + ";");
      sw.WriteLine("");
      foreach (KeyValuePair<double, SwitchingLine> pair in this)
      {
        pair.Value.WriteToFile(sw);
        sw.WriteLine("");
      }
    }

    /// <summary>
    /// Generate file name for a surface of a given component of a given player's control
    /// </summary>
    /// <param name="plNum">The number of the player (1 or 2)</param>
    /// <param name="compNum">The number of control component</param>
    /// <returns>The appropriate file name</returns>
    static public string GenerateFileName(int plNum, int compNum)
    {
      return (plNum == 1 ? "First" : "Second") + " player, component " + compNum + "." +
				GameData.Extensions[ComputationType.SwitchingSurfaces];
    }
  }
}
