namespace TwoPartialPursuer; 

public static class TwoPartialPursuers {
  public static void Main(string[] args) {
    if (args.Length < 2) {
      Console.WriteLine("The program needs two command line arguments:");
      Console.WriteLine("  - Working Directory: the folder, where the data are located");
      Console.WriteLine("	 - the name of the file with input data");
      return;
    }

    TwoPartialPursuer twoPartialPursuer = new TwoPartialPursuer(args[0], args[1]);
    twoPartialPursuer.Compute();
    Console.WriteLine("The END!");
  }
}
