using static CGLibrary.Geometry<double, Sandbox.DConvertor>;


namespace Sandbox;

class Sandbox {

  static bool Orthg(Matrix A) {
    bool   res = true;
    Matrix eye = Matrix.Eye(A.Rows);
    res &= (A.Transpose() * A).Equals(eye);
    res &= (A * A.Transpose()).Equals(eye);

    return res;
  }


  static void Main(string[] args) {
    var rnd = new GRandomLC(1);
    int dim = 5;

    // Генерируем независимые векторы
    var v1 = Vector.GenVector(dim, rnd);
    var v2 = Vector.GenVector(dim, rnd);
    var v3 = Vector.GenVector(dim, rnd);

    // var v1 = 2*Vector.MakeOrth(dim, 1);
    // var v2 = 3*Vector.MakeOrth(dim, 2);
    // var v3 = 0.5*Vector.MakeOrth(dim, 3);

    // Зависимый (вставлен между v2 и v3)
    var dep = v1 + v2;

    // Составляем список в нужном порядке:
    var vectors =
      new List<(Vector vec, string name)>
        {
          (v1, "v1")
        , (v2, "v2")
        , (dep, "dep")
        , (v2, "v2 снова")
        , (v3, "v3")
        };

    var lb = LinearBasis.GenLinearBasis(dim, 0, rnd);
    int nk = 0;
    int ns = 0;
    var q  = new MutableMatrix(Matrix.Eye(dim));
    var w  = new MutableMatrix(Matrix.Eye(dim));

    // Локальный метод для обработки одного вектора
    void Process((Vector vec, string name) item) {
      nk = Decomposition.LQ_FullUpdate(ref q, nk, item.vec);
      ns = Decomposition.QR_FullUpdate(ref w, ns, item.vec);

      bool added = lb.AddVector(item.vec);
      Console.WriteLine($"{item.name}: nk={nk}, ns={ns}, added={added}");
      Console.WriteLine(lb);
      Console.WriteLine(q);
      Console.WriteLine(w.Transpose());
    }

    // Проходим по списку ровно в том порядке, в котором он задан
    foreach (var item in vectors)
      Process(item);
  }

}


// int dim = 5;
//
//
// ConvexPolytop polytop1 = ConvexPolytop.SimplexRND(dim);
// //ConvexPolytop polytop2 = ConvexPolytop.SimplexRND(dim);
//
// ConvexPolytop polytop2 = ConvexPolytop.Cube01_VRep(dim);
//
// ConvexPolytop res = MinkowskiSum.BySandipDas(polytop1, polytop2);
//
// _ = res.Hrep;
// Stopwatch timer = new Stopwatch();
//
// timer.Restart();
// // previous approach
// var x = ConvexPolytop.HrepToVrep_Geometric(res.Hrep);
// var y = ConvexPolytop.CreateFromPoints(x!);
// _ = y.FLrep;
// timer.Stop();
// Console.WriteLine($"old: {timer.Elapsed.Seconds}");
// Console.WriteLine($"old f-vec: {string.Join(", ", y.fVector)}");
//
//
// timer.Restart();
// // new approach
// ConvexPolytop nwe = ConvexPolytop.CreateFromFaceLattice(HrepToFLrep.HrepToFLrep_Geometric(res.Hrep, 5)!);
//
// timer.Stop();
// Console.WriteLine($"new: {timer.Elapsed.Seconds}");
// Console.WriteLine($"new f-vec: {string.Join(", ", nwe.fVector)}");
//
// Console.WriteLine($"res = {string.Join(", ", res.fVector)}");
// Console.WriteLine($"{y.FLrep.Equals(nwe.FLrep)}");

// Matrix m = new Matrix(new double[,]{ { 2.0, 3, 4 }});
// Console.WriteLine($"{m.ToRREF()}");
// LinearBasis basis = LinearBasis.GenLinearBasis(3, 2);
//
// Vector n = basis.FindOrthonormalVector();
//
// HyperPlane hp = new HyperPlane(n, Vector.Zero(3));
//
//
// Console.WriteLine($"{basis}");
// LinearBasis linBasis = hp.AffBasis.LinBasis;
// Console.WriteLine($"{linBasis}");
// Console.WriteLine($"same space? {linBasis.SpanSameSpace(basis)}\n");
// Console.WriteLine($"{basis.Basis.ToRREF()}\n");
// Console.WriteLine($"{linBasis.Basis.ToRREF()}");
