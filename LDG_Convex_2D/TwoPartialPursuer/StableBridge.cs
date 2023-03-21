using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;
using ParamReaderLibrary;

namespace LDGObjects {
/// <summary>
/// Class of a time section of a stable bridge keeping time stamp and the time section polygon
/// </summary>
public class TimeSection2D : IComparable<TimeSection2D> {
  /// <summary>
  /// Property of the time stamp
  /// </summary>
  public double t { get; protected set; }

  /// <summary>
  /// Property of the polygon
  /// </summary>
  public ConvexPolygon section { get; protected set; }

  /// <summary>
  /// The data constructor of the time section
  /// </summary>
  /// <param name="nt">The time stamp</param>
  /// <param name="np">The polygon of the section</param>
  public TimeSection2D(double nt, ConvexPolygon np) {
    t       = nt;
    section = np;
  }

  /// <summary>
  /// The constructor that reads the time section from a parameter reader
  /// in the same format as the section is written (see WriteToFile)
  /// </summary>
  /// <param name="pr">The parameter reader, wherefrom to read the time section</param>
  public TimeSection2D(ParamReader pr) {
    t = pr.ReadDouble("t");
    int           qntV       = pr.ReadInt("qntV");
    double[,]     vertCoords = pr.Read2DArray<double>("Vertices", qntV, 2);
    List<Point2D> vertices   = new List<Point2D>();
    for (int i = 0; i < qntV; i++)
      vertices.Add(new Point2D(vertCoords[i, 0], vertCoords[i, 1]));
    section = new ConvexPolygon(vertices);
  }

  /// <summary>
  /// Method for writing the data about the section to the file in the format
  ///   // ----------------------------------------------------
  ///   t = ...;
  ///   qntV = ...;
  ///   Vertices = { 
  ///     {..., ...}, 
  ///     {..., ...}, 
  ///     ... 
  ///     {..., ...} 
  ///   };
  ///   // ----------------------------------------------------
  /// </summary>
  /// <param name="sw">The stream whereto write the data</param>
  public void WriteToFile(StreamWriter sw) {
    string comment = "// ----------------------------------------------------";
    int    qntV    = section.Contour.Count;

    sw.WriteLine(comment);

    sw.WriteLine("t = " + t + ";");
    sw.WriteLine("qntV = " + qntV + ";");

    sw.WriteLine("Vertices = {");
    for (int i = 0; i < qntV; i++) {
      sw.Write("  { " + section.Contour[i].x + ", " + section.Contour[i].y + " }");
      if (i == qntV - 1)
        sw.WriteLine("");
      else
        sw.WriteLine(",");
    }
    sw.WriteLine("};");

    sw.WriteLine(comment);
  }

  /// <summary>
  /// Comparer of two time sections in the sense of their time instants
  /// </summary>
  /// <param name="other">The time section to be compared with</param>
  /// <returns>Standard -1, 0, +1 comparison result</returns>
  public int CompareTo(TimeSection2D other) { return Tools.CMP(this.t, other.t); }
}

/// <summary>
/// Type of the tube
/// </summary>
public enum TubeType {
  /// <summary>
  /// This tube is a stable bridge
  /// </summary>
  Bridge

 ,

  /// <summary>
  /// This tube is a tube of first player's vectograms
  /// </summary>
  Vectogram1st

 ,

  /// <summary>
  /// This tube is a tube of second player's vectograms
  /// </summary>
  Vectogram2nd
}

/// <summary>
/// Class of a stable bridge that contains the payoff value and a list of time sections
/// </summary>
public class StableBridge2D : List<TimeSection2D>, IComparable<StableBridge2D> {

#region Signatures
  /// <summary>
  /// Signature of a stable bridge file
  /// </summary>
  static public string SignatureBridge = "Stable bridge";

  /// <summary>
  /// Signature of a file with a first player's vectigram tube
  /// </summary>
  static public string Signature1stTube = "First player's vectograms";

  /// <summary>
  /// Signature of a file with a second player's vectigram tube
  /// </summary>
  static public string Signature2ndTube = "Second player's vectograms";
#endregion

#region Bridge data
  /// <summary>
  /// The type of the tube
  /// </summary>
  public TubeType Type { get; protected set; }

  /// <summary>
  /// The value of the payoff
  /// </summary>
  public double c { get; protected set; }

  /// <summary>
  /// Full name of the problem
  /// </summary>
  public string ProblemName { get; protected set; }

  /// <summary>
  /// Short name of the problem
  /// </summary>
  public string ShortProblemName { get; protected set; }
#endregion

#region Constructors
  /// <summary>
  /// The only constructor of the class setting the value of the payoff and clearing the list
  /// </summary>
  /// <param name="problemName">Full name of the problem, for which this bridge is computed</param>
  /// <param name="shortProblemName">Short name of the problem, for which this bridge is computed</param>
  /// <param name="nc">The value of the payoff</param>
  /// <param name="type">Type of the tube: stable bridge or a tube of vectograms</param>
  public StableBridge2D(string problemName, string shortProblemName, double nc, TubeType type) : base() {
    ProblemName      = problemName;
    ShortProblemName = shortProblemName;
    c                = nc;
    Type             = type;
  }

  /// <summary>
  /// Constructor that read a stable bridge from a file
  /// assuming that the file is of correct format
  /// </summary>
  /// <param name="bridgeFile">File name with the stable bridge</param>
  public StableBridge2D(string bridgeFile) : this(new ParamReader(bridgeFile)) { }

  /// <summary>
  /// Constructor, which reads the bridge data from a parameter reader
  /// </summary>
  /// <param name="pr">The parameter reader, wherefrom the data should be read</param>
  public StableBridge2D(ParamReader pr) : base() {
    ProblemName      = pr.ReadString("ProblemName");
    ShortProblemName = pr.ReadString("ShortProblemName");
    string ContentType = pr.ReadString("ContentType");
    if (ContentType != SignatureBridge && ContentType != Signature1stTube && ContentType != Signature2ndTube)
      throw new ArgumentException("Reading a stable bridge: wrong content type of the BRIDGE-file");

    if (ContentType == SignatureBridge)
      Type = TubeType.Bridge;
    else if (ContentType == Signature1stTube)
      Type = TubeType.Vectogram1st;
    else // if (ContentType == Signature2ndTube)
      Type = TubeType.Vectogram2nd;

    c = pr.ReadDouble("c");

    int secQnt = pr.ReadInt("secQnt");
    for (int i = 0; i < secQnt; i++)
      Add(new TimeSection2D(pr));

    Sort();
  }
#endregion

  /// <summary>
  /// Method for writing the data about the bridge to the file in the format
  ///   ProblemName = "...";
  ///   ShortProblemName = "...";
  ///   ContentType = "Stable bridge" / "First player's vectogram" / "Second player's vectogram";
  ///   
  ///   c = ...;
  ///   
  ///   secQnt = ...;
  ///   
  ///   // ----------------------------------------------------
  ///   t = T;
  ///   ...
  ///   // ----------------------------------------------------
  ///   
  ///   // ----------------------------------------------------
  ///   t = T-dt;
  ///   ...
  ///   // ----------------------------------------------------
  ///   
  ///   ...
  /// </summary>
  /// <param name="sw">The stream whereto write the data</param>
  /// <param name="ProblemName">Name of the problem</param>
  /// <param name="ShortProblemName">Short name of the problem</param>
  public void WriteToFile(StreamWriter sw) {
    sw.WriteLine("ProblemName = \"" + ProblemName + "\";");
    sw.WriteLine("ShortProblemName = \"" + ShortProblemName + "\";");

    switch (Type) {
      case TubeType.Bridge:
        sw.WriteLine("ContentType = \"Stable bridge\";");
        break;
      case TubeType.Vectogram1st:
        sw.WriteLine("ContentType = \"First player's vectograms\";");
        break;
      case TubeType.Vectogram2nd:
        sw.WriteLine("ContentType = \"Second player's vectograms\";");
        break;
    }
    sw.WriteLine("");
    sw.WriteLine("c = " + c + ";");
    sw.WriteLine("");
    sw.WriteLine("secQnt = " + Count + ";");
    sw.WriteLine("");

    for (int i = 0; i < Count; i++) {
      this[i].WriteToFile(sw);
      sw.WriteLine("");
    }
  }

  /// <summary>
  /// Get index of a time section of the bridge, which time instant equals or is greater 
  /// than the given instant
  /// </summary>
  /// <param name="t">The time instant, which the time section should be taken</param>
  /// <returns>The time section or -1, if there is not time section at the corresponding instant or later</returns>
  public int GetSectionIndex(double t) { return this.BinarySearchByPredicate(sec => Tools.GE(sec.t, t)); }

  /// <summary>
  /// Comparison with other bridge in the sense of c values
  /// </summary>
  /// <param name="other">The bridge to be compared with</param>
  /// <returns>Standard -1,0,+1 comparison result</returns>
  public int CompareTo(StableBridge2D other) { return Tools.CMP(this.c, other.c); }

  /// <summary>
  /// Generating bridge file name on the basis of paramtere c value
  /// </summary>
  /// <param name="c">The value of the parameter</param>
  /// <returns>The name of the file</returns>
  static public string GenerateFileName(double c) {
    return c.ToString("+000000.000000") + "." + GameData.Extensions[ComputationType.StableBridge];
  }
}
}
