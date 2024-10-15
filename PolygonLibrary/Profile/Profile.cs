using System.Globalization;
using Tests.ToolsTests;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<double, Tests.DConvertor>;

using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    // "E:\\Work\\PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_Computations\\";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    // Tools.Eps = 1e-8;
    // string eps = "1e-008";
    // string ftype = "double";

    Tools.Eps = 1e-16;
    string eps   = "1e-016";
    string ftype = "ddouble";

    bool isDouble = ftype == "double";


    List<HyperPlane> hps = new List<HyperPlane>() // пример из статьи 2020г Accelerating Fourier–Motzkin elimination using bit pattern trees
      {
        new HyperPlane(new Vector(new ddouble[]{0,0,0,-1,0}), 0),
        new HyperPlane(new Vector(new ddouble[]{0,-3,0,4,1}), 0),
        new HyperPlane(new Vector(new ddouble[]{-1,0,1,0,-1}), 0),
        new HyperPlane(new Vector(new ddouble[]{-1,4,-4,0,3}), 0),
        new HyperPlane(new Vector(new ddouble[]{-2,0,0,-1,0}), 0),
        new HyperPlane(new Vector(new ddouble[]{1,0,-3,0,-2}), 0),
        new HyperPlane(new Vector(new ddouble[]{3,0,0,0,2}), 0),
      };
    foreach (var hyperPlane in hps) {
      Console.WriteLine($"{hyperPlane}");
    }
    Console.WriteLine();
    Console.WriteLine();
    // var            hps = ConvexPolytop.Cube01_HRep(3).Hrep;
    // hps.Add(new HyperPlane(Vector.Ones(3), Vector.Ones(3)));
    FourierMotzkin fm   = new FourierMotzkin(hps);
    var            x    = fm.EliminateVariableNaive(1);
    x    = x.EliminateVariableNaive(2);
    x    = x.EliminateVariableNaive(3);
    foreach (var hyperPlane in x.HPs) {
      Console.WriteLine($"{hyperPlane}");
    }







    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string      t   = "6.10";
    // ParamReader prR = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{ftype}/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");


    // ConvexPolytop res = ConvexPolytop.Cube01_VRep(4);
    // ConvexPolytop res = ConvexPolytop.SimplexRND(4);
    // ConvexPolytop res = ConvexPolytop.DistanceToOriginBall_2(3, 4,3, 2);
    // ConvexPolytop res = ConvexPolytop.CreateFromReader(prR);
    //
    // Vector v   = Vector.Ones(res.PolytopDim);
    //
    // string      name = "OrigMoved";
    // ParamWriter prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // Console.WriteLine($"{res.ShiftToOrigin().Hrep.All(hp => hp.ContainsNegative(Vector.Zero(4)))}");
    // res.ShiftToOrigin().WriteIn(prW);
    // name = "RotFromVrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytochp");
    // res.Rotate(rot).WriteIn(prW);
    // name = "RotFromFLrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // ConvexPolytop.CreateFromFaceLattice(res.FLrep).Rotate(rot).WriteIn(prW);
    // name = "RotFromHrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // ConvexPolytop.CreateFromHalfSpaces(res.Hrep).Rotate(rot).WriteIn(prW);

    // ConvexPolytop a   = res;
    // FaceLattice   aFL = HrepToFLrep.HrepToFLrep_Geometric(a.Hrep, res.PolytopDim);
    // Console.WriteLine($"lvl = 0:\t{aFL.Lattice[0].SetEquals(res.FLrep.Lattice[0])}");
    // Console.WriteLine($"lvl = 1:\t{aFL.Lattice[1].SetEquals(res.FLrep.Lattice[1])}");
    // Console.WriteLine($"lvl = 2:\t{aFL.Lattice[2].SetEquals(res.FLrep.Lattice[2])}");
    // Console.WriteLine($"Are equal:\t{aFL.Equals(res.FLrep)}");
    //
    // Console.WriteLine($"lvl 0 vert: aFL = {aFL.Lattice[0].Count}\tres = {res.FLrep.Lattice[0].Count}");
    // Console.WriteLine($"lvl 1 vert: aFL = {aFL.Lattice[1].Count}\tres = {res.FLrep.Lattice[1].Count}");
    // Console.WriteLine($"lvl 2 vert: aFL = {aFL.Lattice[2].Count}\tres = {res.FLrep.Lattice[2].Count}");
    // Console.WriteLine($"lvl 3 vert: aFL = {aFL.Lattice[3].Count}\tres = {res.FLrep.Lattice[3].Count}");


    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");

    // solverLdg.Solve(false, true, isDouble);

  }

}


// string      t   = "3.10";
// ParamReader prP = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{t}) P {solverLdg.fileName}.cpolytop");
// ParamReader prQ = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{t}) Q {solverLdg.fileName}.cpolytop");
// ParamReader prW = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{ftype}/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
//
// ConvexPolytop P = ConvexPolytop.CreateFromReader(prP);
// ConvexPolytop Q = ConvexPolytop.CreateFromReader(prQ);
// ConvexPolytop W = ConvexPolytop.CreateFromReader(prW);
//
// var         x  = solverLdg.DoNextSection(W, P, Q);
// string tNext = "3.00)";
// ParamWriter pr = new ParamWriter($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/ddouble/Geometric/1e-016/{tNext}{solverLdg.fileName}.cpolytop");
// x.WriteIn(pr);
