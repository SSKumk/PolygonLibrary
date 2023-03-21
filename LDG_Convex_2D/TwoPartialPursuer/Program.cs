using LDGObjects;
using PolygonLibrary;
using ParamReaderLibrary;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;


public static class TwoPartialPursuer {
  public static void Main() {
    GameData       gd = new GameData("../../../SourceData/ex00-VerySimple.c", ComputationType.StableBridge);
    StableBridge2D br;

    // Loop on C values
      Console.WriteLine(gd.cValues[0]);

      double t = gd.T;
      br = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[0], TubeType.Bridge);

      ConvexPolygon curSec = new ConvexPolygon(gd.payVertices);

      br.Add(new TimeSection2D(t, curSec));

      // Main computational loop
      bool flag = true;
      while (flag && Tools.GT(t, gd.t0)) {
        curSec =  (curSec + gd.Ps[t]) - gd.Qs[t];
        t      -= gd.dt;

        if (curSec == null || curSec.Contour.Count < 3) {
          flag = false;
          Console.WriteLine("    empty interior of the section at t = " + t.ToString("#0.0000"));
        } else
          br.Add(new TimeSection2D(t, curSec));
      }

      // Writing the bridge
      StreamWriter sr = new StreamWriter(gd.path + StableBridge2D.GenerateFileName(gd.cValues[0]));
      br.WriteToFile(sr);
      sr.Close();
    }
}
