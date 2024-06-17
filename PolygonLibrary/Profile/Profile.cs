using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
// using System.Numerics;
// using System.Numerics;
using System.Runtime.InteropServices;
using CGLibrary;
using DoubleDouble;
using Tests.ToolsTests;
using MultiPrecision;
using Tests;

// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;


namespace Profile;

using System;

class Program
{

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args)
  {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // LinearBasis lb = new LinearBasis(3,5);
    // lb.AddVector(Vector.GenVector(5));
    // System.Console.WriteLine(lb[3]);
    // System.Console.WriteLine(lb[3].Length);
    // LinearBasis.CheckCorrectness(lb);

    // Matrix A = new Matrix(new double[,] { { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 1 }, { 1, 0, 0, 0, 0 } });
    // A = A.Transpose();

    // Matrix A = Matrix.GenMatrix(3,3,-10,10,new GRandomLC(10));
    // (var Q, var R) = QRDecomposition.ByReflection(A);
    // Console.WriteLine($"A:\n{A}");
    // Console.WriteLine($"Q:\n{Q}");
    // Console.WriteLine($"R:\n{R}");
    // Console.WriteLine($"Q*R:\n{Q * R}");
    // Console.WriteLine((Q * R - A).Equals(Matrix.Zero(A.Rows, A.Cols)));
    // Console.WriteLine(Q.TakeVector(0) * Q.TakeVector(2));


    // System.Console.WriteLine(LinearBasis.IsContains(Vector.Ones(3).LiftUp(5, 0), new LinearBasis(5, 3)));



    // AffineBasis affb  = new AffineBasis(new Vector[] { origin, B, C });
    // HyperPlane  hpaff = new HyperPlane(affb);
    // Console.WriteLine(hpaff.Normal * (B - origin));

    // Matrix A = Matrix.hcat
    //   (affb.LinBasis, new Matrix(affb.LinBasis.VecDim, 1, Vector.MakeOrth(3, 1).GetAsArray()));
    // QRDecomposition q = new QRDecomposition(A);
    //
    // Console.WriteLine(q.Q.TakeVector(2) * (B - origin));
    //
    // Console.WriteLine(q.R);


    // Console.WriteLine(Vector.OrthonormalizeAgainstBasis(B - origin, affb.Basis));

    // Console.WriteLine(lb.GetMatrix());

    // Matrix A = new Matrix(new double[,] { { 1, 2.5307302532090645 }, { 0, 3.7737420279937193 }, { 0, -2.0523162773000814 } });
    // QRDecomposition q = new QRDecomposition(A);
    // Console.WriteLine(q.Q);


    // Matrix          A = Matrix.Hilbert(15);
    // LinearBasis linearBasis = LinearBasis.FromMatrix(A);

    // Console.WriteLine(q.Q);
    // Console.WriteLine();
    // Console.WriteLine(linearBasis.GetMatrix());

    // Console.WriteLine(q.Q.TakeVector(0) + linearBasis.Basis[0]);


    // Console.WriteLine((q.Q * q.R).Equals(A));

    // string      matDot = "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2";
    //
    // string      t      = "4.60)";
    // ParamReader prSec     = new ParamReader($"{pathData}{matDot}/{t}materialDot1-0.9-supG.tsection");
    // ParamReader prP     = new ParamReader($"{pathData}{matDot}/{t} P materialDot1-0.9-supG.tsection");
    // ParamReader prQ     = new ParamReader($"{pathData}{matDot}/{t} Q materialDot1-0.9-supG.tsection");
    // var x1 = prSec.ReadNumber<ddouble>("t");
    // var x2 = prP.ReadNumber<ddouble>("t");
    // var x3 = prQ.ReadNumber<ddouble>("t");

    // ConvexPolytop sec = ConvexPolytop.AsFLPolytop(prSec);
    // ConvexPolytop P   = ConvexPolytop.AsFLPolytop(prP);
    // ConvexPolytop Q   = ConvexPolytop.AsFLPolytop(prQ);

    // Console.WriteLine(sec.MinDistBtwVs());


    // ConvexPolytop A = ConvexPolytop.AsFLPolytop(pr);
    // Console.WriteLine(A.VRep.Count);
    //
    // pr = new ParamReader($"{pathData}{matDot}/{t} P materialDot1-0.9-supG.tsection");
    // pr.ReadNumber<ddouble>("t");
    // ConvexPolytop P = ConvexPolytop.AsFLPolytop(pr);
    // Console.WriteLine(P.VRep.Count);
    //
    // pr = new ParamReader($"{pathData}{matDot}/{t} Q materialDot1-0.9-supG.tsection");
    // pr.ReadNumber<ddouble>("t");
    // ConvexPolytop Q = ConvexPolytop.AsVPolytop(pr);
    // Console.WriteLine(Q.VRep.Count);
    //
    // ConvexPolytop  x = MinkowskiSum.BySandipDas(A, Q);
    // ConvexPolytop? y = MinkowskiDiff.H2VGeometric(x, Q);
    //
    // Console.WriteLine(y!.VRep.Count);


    // ConvexPolytop A       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop B       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop pp      = MinkowskiSum.BySandipDas(A, B);
    // ConvexPolytop ppc      = MinkowskiSum.BySandipDas(A, B, true);
    // Console.WriteLine(pp.HRep.Count);
    // Console.WriteLine(ppc.HRep.Count);

    // ConvexPolytop polytop = ConvexPolytop.AsVPolytop(ConvexPolytop.Cube01(4).VRep.Union(new List<Vector>()
    //   {
    //     new Vector(new ddouble[]{0.5,0.5,0.5,2}), // - в общем-то, это не нужно, чтобы сломать
    //     new Vector(new ddouble[]{0.5,0.5,0.5,-2})
    //   }));


    // var           g       = ConvexPolytop.HRepToVRep_Geometric(polytop.HRep);
    // Console.WriteLine("polytop == geometricH2V");
    // Console.WriteLine(polytop.VRep.SetEquals(g));
    // Console.WriteLine("polytop \\subset geometricH2V");
    // Console.WriteLine(polytop.VRep.IsSubsetOf(g));
    // Console.WriteLine("geometricH2V \\ polytop");
    // Console.WriteLine(string.Join('\n', g.Except(polytop.VRep)));
    // Console.WriteLine("polytop \\ geometricH2V");
    // Console.WriteLine(string.Join('\n', polytop.VRep.Except(g)));


    // SolverLDG.WriteSimplestTask_TerminalSet_GameItself(5, pathData);

    SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");

    solverLdg.Solve(false);
  }

}
