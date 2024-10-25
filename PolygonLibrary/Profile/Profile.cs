using System.Diagnostics;
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

    // int dim = 5;

    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string    t         = "5.10";
    // ParamReader prR = new ParamReader
    // ($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{ftype}/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
    // ConvexPolytop polytop = ConvexPolytop.CreateFromReader(prR);
    // ConvexPolytop polytop = ConvexPolytop.Cube01_VRep(5);
    // ConvexPolytop polytop = ConvexPolytop.Cube01_VRep(5);
    // ConvexPolytop polytop = MinkowskiSum.BySandipDas
    // (ConvexPolytop.Cube01_VRep(dim).RotateRND(), ConvexPolytop.SimplexRND(dim));

    // var x = polytop.GetInHrep();
    // var y = ConvexPolytop.HrepToVrep_Geometric(x.Hrep);
    // var z = HrepToFLrep.HrepToFLrep_Geometric(x.Hrep,dim);
    // Console.WriteLine($"{polytop.Vrep.SetEquals(y)}");
    // Console.WriteLine($"P: {polytop.Vrep.Count}");
    // Console.WriteLine($"y: {y.Count}");
    // Console.WriteLine($"z: {z.Vertices.Count}");

    // ConvexPolytop polytop = ConvexPolytop.CreateFromPoints
    //   (
    //    new List<Vector>()
    //      {
    //        new Vector(new ddouble[] { 1, 0, 0 })
    //      , new Vector(new ddouble[] { 0, 1, 0 })
    //      , new Vector(new ddouble[] { 0, 0, 1 })
    //      , new Vector(new ddouble[] { 0, 0, 0 })
    //       ,
    //      }
    //   );


    // List<HyperPlane> hps = new List<HyperPlane>()
    //   {
    //     new HyperPlane(new Vector(new ddouble[] { 1, 0 }), 0),
    //     new HyperPlane(new Vector(new ddouble[] { 0, 1 }), 2),
    //     new HyperPlane(new Vector(new ddouble[] { 0, -1 }), 3),
    //     new HyperPlane(new Vector(new ddouble[] { -1, 0 }), 0),
    //     new HyperPlane(new Vector(new ddouble[] { -1, 1 }), -5),
    //
    //   };
    //
    // Console.WriteLine($"{ConvexPolytop.FindInitialVertex_Simplex(hps, out _)}");


    // Console.WriteLine($"{polytop.GetInHrep().InnerPoint}");


    // List<HyperPlane> hps = new List<HyperPlane>()
    //   {
    //     new HyperPlane(new Vector(new ddouble[] { 1, 0 }), 1)
    //   , new HyperPlane(new Vector(new ddouble[] { 0, 1 }), 1)
    //   , new HyperPlane(new Vector(new ddouble[] { -1, 0 }), 1)
    //   , new HyperPlane(new Vector(new ddouble[] { 0, -1 }), 1)
    //   , new HyperPlane(new Vector(new ddouble[] { 1, 1 }), new Vector(new ddouble[] { 1, 1 }))
    //   , new HyperPlane(new Vector(new ddouble[] { 0, 1 }), 5)
    //   };
    // ConvexPolytop    cube = ConvexPolytop.CreateFromHalfSpaces(hps);
    // // ConvexPolytop cube = ConvexPolytop.Cube01_HRep(2);
    // // var           x    = cube.ShiftToOrigin().Polar();
    // var           x    = cube.Polar();
    // Console.WriteLine($"{string.Join('\n', x.Vrep)}");
    // Console.WriteLine();
    // Console.WriteLine();
    // Console.WriteLine($"{string.Join('\n', x.Polar().Vrep)}");

    //
    // int constant = 1;
    // List<HyperPlane> hps = new List<HyperPlane>() // пример из статьи 2020г Accelerating Fourier–Motzkin elimination using bit pattern trees
    //   {
    //     new HyperPlane(new Vector(new ddouble[]{0,0,0,-1,0}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{0,-3,0,4,1}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{-1,0,1,0,-1}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{-1,4,-4,0,3}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{-2,0,0,-1,0}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{1,0,-3,0,-2}), constant),
    //     new HyperPlane(new Vector(new ddouble[]{3,0,0,0,2}), constant),
    //   };
    //
    // ConvexPolytop p = ConvexPolytop.CreateFromHalfSpaces(hps);
    // Console.WriteLine($"{string.Join('\n',p.Vrep)}");
    //
    // // var            hps = ConvexPolytop.Cube01_HRep(3).Hrep;
    // // hps.Add(new HyperPlane(Vector.Ones(3), Vector.Ones(3)));
    // FourierMotzkin fm   = new FourierMotzkin(hps);
    // var            x    = fm.EliminateVariableNaive(1);
    // x    = x.EliminateVariableNaive(2);
    // x    = x.EliminateVariableNaive(3);
    // Console.WriteLine($"Eq after 3 steps of elimination: {x.HPs.Count}");
    // ConvexPolytop doRed = ConvexPolytop.CreateFromHalfSpaces(x.HPs).Polar().GetInFLrep();
    //
    // Console.WriteLine();
    // Console.WriteLine();
    // foreach (var hyperPlane in ConvexPolytop.HRedundancyByGW(x.HPs, 0.1*Vector.Ones(5))) {
    //   Console.WriteLine($"{hyperPlane}");
    // }
    // var _ = doRed.Vrep;
    // ConvexPolytop afterRed = doRed.Polar();
    //
    //
    // Console.WriteLine();
    // Console.WriteLine();
    // foreach (var hyperPlane in afterRed.Hrep) {
    //   Console.WriteLine($"{hyperPlane}");
    // }


    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string      t   = "6.10";
    // ParamReader prR = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{ftype}/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");


    int dim = 5;
    string      name = "Cube5D_RND+SimplexRND5D";
    // ConvexPolytop res = ConvexPolytop.Cube01_VRep(4);
    // ConvexPolytop res = ConvexPolytop.SimplexRND(4);
    // ConvexPolytop res = ConvexPolytop.DistanceToOriginBall_2(3, 4,3, 2);
    // ConvexPolytop res = ConvexPolytop.CreateFromReader(prR);
    ConvexPolytop res = MinkowskiSum.BySandipDas(ConvexPolytop.SimplexRND(dim), ConvexPolytop.SimplexRND(dim));
    // ParamReader prR = new ParamReader($"{pathData}/Other/{name}.cpolytop");
    // ConvexPolytop res = ConvexPolytop.CreateFromReader(prR);

    // ParamWriter prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // res.WriteIn(prW, ConvexPolytop.Rep.FLrep);
    // Console.WriteLine($"{res.ShiftToOrigin().Hrep.All(hp => hp.ContainsNegative(Vector.Zero(4)))}");
    // res.ShiftToOrigin().WriteIn(prW);
    // name = "RotFromVrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // res.Rotate(rot).WriteIn(prW);
    // name = "RotFromFLrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // ConvexPolytop.CreateFromFaceLattice(res.FLrep).Rotate(rot).WriteIn(prW);
    // name = "RotFromHrep";
    // prW  = new ParamWriter($"{pathData}/Other/{name}.cpolytop");
    // ConvexPolytop.CreateFromHalfSpaces(res.Hrep).Rotate(rot).WriteIn(prW);

    // var x = ConvexPolytop.HrepToVrep_Geometric(res.Hrep);
    // Console.WriteLine($"{x.Count}");


    FaceLattice   aFL = HrepToFLrep.HrepToFLrep_Geometric(res.Hrep, res.PolytopDim);
    ConvexPolytop x   = ConvexPolytop.CreateFromFaceLattice(aFL);

    Console.WriteLine($"{x.Equals(res)}");

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
