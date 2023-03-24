using LDGObjects;
using PolygonLibrary;
using ParamReaderLibrary;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;

namespace TwoPartialPursuer;

/// <summary>
/// This class holds information about TwoPartialPursuer-problem and can solve it.
/// </summary>
class TwoPartialPursuer {

  /// <summary>
  /// The main directory before solving TPP-problem 
  /// </summary>
  private readonly string origDir;

  /// <summary>
  /// The directory where source file and result folder are placed
  /// </summary>
  private string workDir;

  /// <summary>
  /// The name of source file
  /// </summary>
  private string fileName;

  /// <summary>
  /// Holds internal information about task
  /// </summary>
  private readonly GameData gd;

  /// <summary>
  /// Game-whole bridge
  /// </summary>
  private StableBridge2D br;

  /// <summary>
  /// One-partial bridge 
  /// </summary>
  private StableBridge2D W1;

  /// <summary>
  /// Second-partial bridge
  /// </summary>
  private StableBridge2D W2;

  /// <summary>
  /// Constructor which creates GameData and init three StableBridges
  /// </summary>
  /// <param name="workDir">The directory where source file and result folder are placed</param>
  /// <param name="fileName">The name of source file</param>
  /// <exception cref="ArgumentException">If there are no path to working directory this exception is thrown</exception>
  public TwoPartialPursuer(string workDir, string fileName) {
    origDir = Directory.GetCurrentDirectory();
    try {
      Directory.SetCurrentDirectory(workDir);
    }
    catch {
      throw new ArgumentException("A problem to switch to the folder WorkDir:", workDir);
    }
    this.workDir  = Directory.GetCurrentDirectory();
    this.fileName = fileName;
    gd            = new GameData(this.workDir + "/" + fileName, ComputationType.StableBridge);
    br            = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[0], TubeType.Bridge);
    W1            = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[0], TubeType.Bridge);
    W2            = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[0], TubeType.Bridge);
  }

  /// <summary>
  /// Computes LDG TwoPartialPursuers
  /// </summary>
  public void Compute() {
    //"../../../Computations/SourceData/ex00-VerySimple.c"

    double t = gd.T;
    br.Add(new TimeSection2D(t, new ConvexPolygon(gd.payVertices))); //
    W1.Add(new TimeSection2D(t, new ConvexPolygon(gd.payVertices))); // W1 = W2 = M at time = T
    W2.Add(new TimeSection2D(t, new ConvexPolygon(gd.payVertices))); // W1 = W2 = M at time = T

    double tPred = gd.T;
    t -= gd.dt;                                                                     // next step
    br.Add(new TimeSection2D(t, br.Last().section + gd.Ps[tPred] - gd.Qs[tPred]));  //br.Last().section == M
    W1.Add(new TimeSection2D(t, W1.Last().section + gd.Ps[tPred] - gd.Qs1[tPred])); //W1.Last().section == M
    W2.Add(new TimeSection2D(t, W2.Last().section + gd.Ps[tPred] - gd.Qs2[tPred])); //W2.Last().section == M

    // Main computational loop
    bool flag = true;
    while (flag && Tools.GT(t, gd.t0)) {
      tPred =  t;
      t     -= gd.dt;
      ConvexPolygon? S1 = W1.Last().section + gd.Ps[tPred] - gd.Qs1[tPred];
      ConvexPolygon? S2 = W1.Last().section + gd.Ps[tPred] - gd.Qs2[tPred];
      ConvexPolygon? T1 = W2.Last().section + gd.Ps[tPred] - gd.Qs1[tPred];
      ConvexPolygon? T2 = W2.Last().section + gd.Ps[tPred] - gd.Qs2[tPred];

      ConvexPolygon? I1 = ConvexPolygon.IntersectionPolygon(S1, T2);
      ConvexPolygon? I2 = ConvexPolygon.IntersectionPolygon(S2, T1);

      if (I1 == null || I2 == null) {
        flag = false;
        Console.WriteLine("    empty interior of the intersection at t = " + t.ToString("#0.0000"));
      } else {
        W1.Add(new TimeSection2D(t, I1));
        W2.Add(new TimeSection2D(t, I2));
      }

      ConvexPolygon? R = br.Last().section + gd.Ps[tPred] - gd.Qs[tPred];
      if (R == null || R.Contour.Count < 3) {
        flag = false;
        Console.WriteLine("    empty interior of the section at t = " + t.ToString("#0.0000"));
      } else
        br.Add(new TimeSection2D(t, R));
    }
  }
}


// Writing the bridge
// StreamWriter sr = new StreamWriter(gd.path + StableBridge2D.GenerateFileName(gd.cValues[0]));
// br.WriteToFile(sr);
// sr.Close();
//
// Directory.SetCurrentDirectory(origDir);
// Console.WriteLine("That's all!");
