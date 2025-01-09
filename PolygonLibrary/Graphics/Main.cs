using LDG;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics;

public class Visualization {

  public struct Traj {

    public List<Vector> Ps;

    public double scale;
    public int    red;
    public int    green;
    public int    blue;

    public Traj(List<Vector> ps, double scale, int red, int green, int blue) {
      Ps         = ps;
      this.scale = scale;
      this.red   = red;
      this.green = green;
      this.blue  = blue;
    }

  }

  public class Facet {

    public readonly IEnumerable<Vector> Vs;
    public readonly Vector              Normal;

    public int red;
    public int green;
    public int blue;


    public Facet(IEnumerable<Vector> vs, Vector normal, int red = 192, int green = 192, int blue = 192) {
      Vs         = vs;
      Normal     = normal;
      this.red   = red;
      this.green = green;
      this.blue  = blue;
    }

  }


  public readonly string VisData;
  public readonly string VisConf;

  public readonly string OutFolderName;
  public readonly string GameDirName;

  public readonly string NumericalType;
  public readonly string Precision;

  public readonly List<string>          BrNames;
  public readonly List<Traj>            Trajs  = new List<Traj>();
  public readonly Dictionary<int, Traj> AimsFp = new Dictionary<int, Traj>();
  public readonly Dictionary<int, Traj> AimsSp = new Dictionary<int, Traj>();

  public Visualization(string pathLdg, string visData, string visConf) {
    VisData = visData;
    VisConf = visConf;


    string confPath = Path.Combine(pathLdg, "Visualization", "!Configs");
    {
      ParamReader prD = new ParamReader(Path.Combine(confPath, VisData + ".visdata"));

      OutFolderName = prD.ReadString("Name");
      GameDirName   = prD.ReadString("GameDirName");
      NumericalType = prD.ReadString("NumericalType");
      Precision     = prD.ReadString("Precision");
    }

    ParamReader prC     = new ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
    List<int>   brNames = prC.ReadList<int>("Bridges");
    BrNames = brNames.Select(i => i.ToString()).ToList();

    int trajCount = prC.ReadNumber<int>("TrajectoryCount");
    for (int i = 1; i <= trajCount; i++) {
      string name = prC.ReadString("Name");
      {
        int[]  color = prC.Read1DArray<int>("Color", 3);
        double scale = prC.ReadNumber<double>("Scale");
        ParamReader prTr =
          new ParamReader(Path.Combine(new string[] { pathLdg, "_Out", GameDirName, "Trajectories", name, "game.traj" }));
        List<Vector> traj = prTr.ReadVectors("Trajectory");
        Trajs.Add(new Traj(traj, scale, color[0], color[1], color[2]));
      }
      bool drawAimFp = prC.ReadBool("DrawAimFP");
      if (drawAimFp) {
        int[]  color = prC.Read1DArray<int>("Color", 3);
        double scale = prC.ReadNumber<double>("Scale");
        ParamReader prTr =
          new ParamReader(Path.Combine(new string[] { pathLdg, "_Out", GameDirName, "Trajectories", name, "fp.aim" }));
        List<Vector> traj = prTr.ReadVectors("FPAims");
        AimsFp.Add(i, new Traj(traj, scale, color[0], color[1], color[2]));
      }
      bool drawAimSp = prC.ReadBool("DrawAimSP");
      if (drawAimSp) {
        int[]  color = prC.Read1DArray<int>("Color", 3);
        double scale = prC.ReadNumber<double>("Scale");
        ParamReader prTr =
          new ParamReader(Path.Combine(new string[] { pathLdg, "_Out", GameDirName, "Trajectories", name, "sp.aim" }));
        List<Vector> traj = prTr.ReadVectors("SPAims");
        AimsSp.Add(i, new Traj(traj, scale, color[0], color[1], color[2]));
      }
    }
  }

  public void WritePly() { // используя BrNames Trajs AimsFp AimsSp сначала собрать у каждого точки и грани, потом всё налепить в один файл!!!

  }



  private class VectorMixedProductComparer : IComparer<Vector> {

    private readonly Vector _outerNormal;
    private readonly Vector _firstPoint;

    public VectorMixedProductComparer(Vector outerNormal, Vector firstPoint) {
      _outerNormal = outerNormal;
      _firstPoint  = firstPoint;
    }

    /// <summary>
    /// The cross-product of two 3D-vectors.
    /// </summary>
    /// <param name="v">The first vector.</param>
    /// <param name="u">The second vector.</param>
    /// <returns>The outward normal to the plane of v and u.</returns>
    public static Vector CrossProduct3D(Vector v, Vector u) {
      double[] crossProduct = new double[3];
      crossProduct[0] = v[1] * u[2] - v[2] * u[1];
      crossProduct[1] = v[2] * u[0] - v[0] * u[2];
      crossProduct[2] = v[0] * u[1] - v[1] * u[0];

      return new Vector(crossProduct);
    }

    public int Compare(Vector? v1, Vector? v2) {
      if (v1 is null && v2 is null)
        return 0;
      if (v1 is null)
        return -1;
      if (v2 is null)
        return 1;

      return Tools.CMP(CrossProduct3D(v1 - _firstPoint, v2 - _firstPoint) * _outerNormal);
    }

  }


  private static ConvexPolytop Validate(ConvexPolytop P) {
    switch (P.SpaceDim) {
      case 1: throw new NotImplementedException("If P.Dim == 1. Непонятно, что делать.");
      case 2: P = P.LiftUp(3, 0); break;
      case 3: break;
      default:
        throw new ArgumentException($"The dimension of the space must be equal less or equal 3! Found spaceDim = {P.SpaceDim}.");
    }

    return P;
  }

  public static void WritePLY(ConvexPolytop P, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFList(FList, P);

    WritePLY_File(prW, VList, FList);
  }

  public static void WritePLY(ConvexPolytop P, Vector x, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFList(FList, P);

    ConvexPolytop cube = ConvexPolytop.RectAxisParallel(-0.001 * Vector.Ones(3), 0.001 * Vector.Ones(3)).Shift(x);
    VList.AddRange(cube.Vrep);
    AddToFList(FList, cube);

    WritePLY_File(prW, VList, FList);
  }

  private static void WritePLY_File(ParamWriter prW, List<Vector> VList, List<Facet> FList) {
    // Пишем в файл в формате .ply
    // шапка
    prW.WriteLine("ply");
    prW.WriteLine("format ascii 1.0");
    prW.WriteLine($"element vertex {VList.Count}");
    prW.WriteLine("property float x");
    prW.WriteLine("property float y");
    prW.WriteLine("property float z");
    prW.WriteLine($"element face {FList.Count}");
    prW.WriteLine("property list uchar int vertex_index");
    prW.WriteLine("end_header");
    // вершины
    foreach (Vector v in VList) {
      prW.WriteLine(v.ToStringBraceAndDelim(null, null, ' '));
    }
    // грани
    foreach (Facet F in FList) {
      prW.Write($"{F.Vs.Count()} ");
      foreach (Vector vertex in F.Vs) {
        prW.Write($"{VList.IndexOf(vertex)} ");
      }
      prW.WriteLine();
    }
  }

  public static void AddToFList(List<Facet> FList, ConvexPolytop polytop) {
    foreach (FLNode F in polytop.FLrep.Lattice[2]) {
      HyperPlane hp = new HyperPlane(F.AffBasis, true, (polytop.FLrep.Top.InnerPoint, false));
      FList.Add
        (
         new Facet
           (
            F.Vertices.ToList().OrderByDescending(v => v, new VectorMixedProductComparer(hp.Normal, F.Vertices.First())).ToArray()
          , hp.Normal
           )
        );
    }
  }

}

public class Program {

  private static readonly string pathData =
    // "E:\\Work\\CGLibrary\\CGLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations";

  public static void Main() { Tools.Eps = 1e-16; }

}
