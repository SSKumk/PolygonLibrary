using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;

using LDGObjects;

namespace Bridges
{
  class Bridges
  {
    static void Main(string[] args)
    {
      if (args.Length < 2)
      {
				Console.WriteLine("The program needs two command line arguments:");
				Console.WriteLine("  - the folder, where the computation data are located");
				Console.WriteLine("	 - the name of the file with input data");
        return;
      }

      Tools.Eps = 1e-10;
			string origDir = Directory.GetCurrentDirectory();
			try
			{
				Directory.SetCurrentDirectory(args[0]);
			}
			catch
			{
				Console.WriteLine("A problem to switch to the folder '" + args[0] + "'");
				return;
			}
      GameData gd = new GameData(args[1], ComputationType.StableBridge);
      StableBridge2D br;

      // Loop on C values
      for (int ic = 0; ic < gd.cValues.Count; ic++)
      {
        Console.WriteLine("c = " + gd.cValues[ic]);

        double t = gd.T;
        br = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[ic], TubeType.Bridge);

        ConvexPolygon curSec = gd.PayoffLevelSet(gd.cValues[ic]);

        // If the initial section has no interior, pass to the next one
        if (curSec == null || curSec.Contour.Count < 3)
        {
          Console.WriteLine("    empty interior of the initial section");
          continue;
        }

        br.Add(new TimeSection2D(t, curSec));

        // Main computational loop
        bool flag = true;
        while (flag && Tools.GT(t, gd.t0))
        {
          curSec = (curSec + gd.Ps[t]) - gd.Qs[t];
          t -= gd.dt;

          if (curSec == null || curSec.Contour.Count < 3)
          {
            flag = false;
            Console.WriteLine("    empty interior of the section at t = " + t.ToString("#0.0000"));
          }
          else
            br.Add(new TimeSection2D(t, curSec));
        }

        // Writing the bridge
        StreamWriter sr = 
					new StreamWriter(gd.path + StableBridge2D.GenerateFileName(gd.cValues[ic]));
        br.WriteToFile(sr);
        sr.Close();
      }

			Directory.SetCurrentDirectory(origDir);
      Console.WriteLine("That's all!");
      //Console.ReadKey();
    }
  }
}
