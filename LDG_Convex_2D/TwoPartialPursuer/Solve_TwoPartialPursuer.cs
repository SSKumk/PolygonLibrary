using System.Globalization;

namespace TwoPartialPursuer; 

public static class Solve_TwoPartialPursuer {
  public static void Main(string[] args) {
    if (args.Length < 2) {
      Console.WriteLine("The program needs two command line arguments:");
      Console.WriteLine("  - Working Directory: the folder, where the data are located");
      Console.WriteLine("	 - the name of the file with input data");
      return;
    }

    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
    TwoPartialPursuer twoPartialPursuer = new TwoPartialPursuer(args[0], args[1]);
    twoPartialPursuer.Compute();
    twoPartialPursuer.WriteBridges();
    twoPartialPursuer.WriteDat();
    twoPartialPursuer.WritePlt();
    Console.WriteLine("The END!");
  }
}
