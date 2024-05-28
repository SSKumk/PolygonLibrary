using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;
using MultiPrecision;
using Tests;

// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;


// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;




namespace Profile;

public class QRDecomposition {

  public Matrix Q { get; private set; }
  public Matrix R { get; private set; }

  public QRDecomposition(Matrix A) { Decompose(A); }

  private void Decompose(Matrix A) {
    int m = A.Rows;
    int n = A.Cols;
    Q = Matrix.Eye(m);
    R = A;

    for (int k = 0; k < n; k++) {
      Vector x  = R.TakeCol(k).SubVector(k, m - k);
      Vector e1 = Vector.MakeOrth(x.Dim, 1);

      Vector u = x + e1 * x.Length * Math.Sign(x[0]);
      u = u.Normalize();

      Matrix Hk           = Matrix.Eye(m);
      Matrix outerProduct = u.OuterProduct(u);
      Hk.SubMatrix(k, k, m - k, m - k, Matrix.Eye(m - k) - 2 * outerProduct);

      R =  Hk * R;
      Q *= Hk.Transpose();
    }
  }

}

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    Matrix a = Matrix.Eye(4);
    Console.WriteLine(a.TakeCol(3));


    int dim     = 3;
    // ConvexPolytop    polytop = ConvexPolytop.Sphere(dim, 2, 4, Vector.Zero(dim), 1);

    QRDecomposition q = new QRDecomposition(Matrix.GenMatrix(4,4,-10,10));
    Console.WriteLine(q.Q);
    Console.WriteLine(q.R);

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

    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");

    // solverLdg.Solve(false);
  }

}
