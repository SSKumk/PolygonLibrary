using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

// [ShortRunJob]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Sandbox {

  [Params(10)]
  public int spaceDim;

  [Params(4)]
  public int subSpaceDim;

  public LinearBasis lb;

  public Vector v;

  public Matrix m;

  [GlobalSetup]
  public void SetUp() {
    // lb = LinearBasis.GenLinearBasis(spaceDim, subSpaceDim);
    // v = Vector.GenVector(spaceDim);
    m = Matrix.GenMatrix(spaceDim, subSpaceDim, -5, -5);

    // if (!(m * m.Transpose()).Equals(m.MultiplyByTranspose())) {
    //   throw new InvalidOperationException();
    // }
  }


  // [Benchmark]
  // public void MultiplyByTranspose() => _ = m * m.Transpose();

  [Benchmark]
  public void MultiplyByTranspose_OptInd() => MultiplyByTranspose_OptInd(m);

  [Benchmark]
  public void MultiplyByTranspose_OptLin() => MultiplyByTranspose_OptLin(m);


  public Matrix MultiplyByTranspose_OptInd(Matrix m) {
    ddouble[] res = new ddouble[m.Rows * m.Rows];
    for (int row = 0; row < m.Rows; row++) {
      int s = row * m.Cols;
      int t = row * m.Cols;
      for (int j = row; j < m.Rows; j++) {
        ddouble sum = Tools.Zero;
        for (int k = 0; k < m.Cols; k++) {
          sum += m[s] * m[t];
          s++;
          t++;
        }
        res[row * m.Rows + j] = sum;
        if (row != j) { res[j * m.Rows + row] = sum; }
        s -= m.Cols;
      }
    }

    return new Matrix(m.Rows, m.Rows, res, false);
  }


  public Matrix MultiplyByTranspose_OptLin(Matrix m) {
    ddouble[] res = new ddouble[m.Rows * m.Rows];

    int resInd  = 0; // заполняет верхний треугольник матрицы результата
    int resInd2 = 0; // заполняет нижний треугольник матрицы результата
    int s       = 0; // идёт в исходной матрице по строкам
    int t       = 0; // идёт в транспонированной матрице по столбцам (реально по строкам исходной)
    for (int row = 0; row < m.Rows; row++, resInd += row, resInd2 = resInd, s += m.Cols, t = s) {
      for (int col = row; col < m.Rows; col++, resInd++, resInd2 += m.Rows, s -= m.Cols) {
        ddouble sum = Tools.Zero;
        for (int k = 0; k < m.Cols; k++) {
          sum += m[s] * m[t];
          s++;
          t++;
        }
        res[resInd]  = sum;
        res[resInd2] = sum;
      }
    }

    return new Matrix(m.Rows, m.Rows, res, false);
  }

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<Sandbox>();
  //
  //     // Matrix a = Matrix.GenMatrixInt(3,2,-4,4, new GRandomLC(1));
  //     // Matrix a = Matrix.GenMatrix(6,3,-4,4, new GRandomLC(1));
  //
  //     // Console.WriteLine($"{(a * a.Transpose()).Equals(a.MultiplyByTranspose())}");
  //   }
  //
  // }

}


/*

| Method                                          | spaceDim | subSpaceDim | Mean      | Error    | StdDev   |
|------------------------------------------------ |--------- |------------ |----------:|---------:|---------:|
| ProjectVectorToSubSpace__MultiplyColumnByVector | 3        | 1           |  72.47 ns | 0.176 ns | 0.147 ns |
| ProjectVectorToSubSpace__lb                     | 3        | 1           |  93.26 ns | 0.378 ns | 0.295 ns |
| ProjectVectorToSubSpace__MultiplyColumnByVector | 3        | 2           | 126.66 ns | 1.207 ns | 1.070 ns |
| ProjectVectorToSubSpace__lb                     | 3        | 2           | 174.91 ns | 0.605 ns | 0.472 ns |
| ProjectVectorToSubSpace__MultiplyColumnByVector | 3        | 3           | 182.65 ns | 2.159 ns | 2.020 ns |
| ProjectVectorToSubSpace__lb                     | 3        | 3           | 266.02 ns | 1.124 ns | 0.877 ns |


  public Vector ProjectVectorToSubSpace__MultiplyColumnByVector(LinearBasis lb, Vector v) {
    ddouble[] proj = new ddouble[lb.SubSpaceDim];
    for (int col = 0; col < lb.SubSpaceDim; col++) {
      proj[col] = lb.Basis.MultiplyColumnByVector(col, v);
    }

    return new Vector(proj, false);
  }

  public Vector ProjectVectorToSubSpace__lb(LinearBasis lb, Vector v) {
    ddouble[] proj = new ddouble[lb.SubSpaceDim];
    for (int i = 0; i < lb.SubSpaceDim; i++) {
      proj[i] = lb[i] * v; // bvec * v
    }
---------------------------------------------------------------------------------------------------------------------


| Method     | spaceDim | subSpaceDim | Mean     | Error   | StdDev  |
|----------- |--------- |------------ |---------:|--------:|--------:|
| NaiveMult  | 3        | 1           | 239.5 ns | 4.70 ns | 5.41 ns |
| MultTransp | 3        | 1           | 149.7 ns | 0.72 ns | 0.56 ns |
| NaiveMult  | 3        | 2           | 370.2 ns | 5.03 ns | 4.71 ns |
| MultTransp | 3        | 2           | 233.3 ns | 1.05 ns | 0.98 ns |


| Method     | spaceDim | subSpaceDim | Mean     | Error    | StdDev   |
|----------- |--------- |------------ |---------:|---------:|---------:|
| NaiveMult  | 4        | 1           | 394.7 ns |  7.49 ns | 13.88 ns |
| MultTransp | 4        | 1           | 241.5 ns |  4.17 ns |  3.70 ns |
| NaiveMult  | 4        | 2           | 637.5 ns | 12.04 ns | 12.36 ns |
| MultTransp | 4        | 2           | 383.6 ns |  4.73 ns |  4.19 ns |
| NaiveMult  | 4        | 3           | 892.8 ns |  6.17 ns |  5.47 ns |
| MultTransp | 4        | 3           | 541.4 ns | 10.25 ns | 10.97 ns |



| Method        | spaceDim | subSpaceDim | Mean     | Error    | StdDev   |
|-------------- |--------- |------------ |---------:|---------:|---------:|
| MultTranspOpt | 3        | 1           | 166.2 ns |  3.77 ns | 10.95 ns |
| MultTranspOpt | 3        | 2           | 238.0 ns |  4.43 ns |  3.93 ns |
| MultTranspOpt | 3        | 3           | 328.6 ns |  5.41 ns |  4.80 ns |
| MultTranspOpt | 4        | 1           | 249.6 ns |  4.46 ns |  3.95 ns |
| MultTranspOpt | 4        | 2           | 380.4 ns |  3.02 ns |  2.52 ns |
| MultTranspOpt | 4        | 3           | 541.8 ns | 10.75 ns | 14.35 ns |


    public Matrix MultiplyByTranspose_OptInd(Matrix m) {
    ddouble[] res = new ddouble[m.Rows * m.Rows];
    for (int row = 0; row < m.Rows; row++) {
      int s = row * m.Cols;
      int t = row * m.Cols;
      for (int j = row; j < m.Rows; j++) {
        ddouble sum = Tools.Zero;
        for (int k = 0; k < m.Cols; k++) {
          sum += m[s] * m[t];
          s++;
          t++;
        }
        res[row * m.Rows + j] = sum;
        if (row != j) { res[j * m.Rows + row] = sum; }
        s -= m.Cols;
      }
    }

    return new Matrix(m.Rows, m.Rows, res, false);
  }


  public Matrix MultiplyByTranspose_OptLin(Matrix m) {
    ddouble[] res = new ddouble[m.Rows * m.Rows];

    int resInd  = 0; // заполняет верхний треугольник матрицы результата
    int resInd2 = 0; // заполняет нижний треугольник матрицы результата
    int s       = 0; // идёт в исходной матрице по строкам
    int t       = 0; // идёт в транспонированной матрице по столбцам (реально по строкам исходной)
    for (int row = 0; row < m.Rows; row++, resInd += row, resInd2 = resInd, s += m.Cols, t = s) {
      for (int col = row; col < m.Rows; col++, resInd++, resInd2 += m.Rows, s -= m.Cols) {
        ddouble sum = Tools.Zero;
        for (int k = 0; k < m.Cols; k++) {
          sum += m[s] * m[t];
          s++;
          t++;
        }
        res[resInd]  = sum;
        res[resInd2] = sum;
      }
    }

    return new Matrix(m.Rows, m.Rows, res, false);
  }

      [Benchmark]
  public void MultiplyByTranspose() => _ = m * m.Transpose();

  [Benchmark]
  public void MultiplyByTranspose_OptInd() => MultiplyByTranspose_OptInd(m);

  [Benchmark]
  public void MultiplyByTranspose_OptLin() => MultiplyByTranspose_OptLin(m);



| Method                     | spaceDim | subSpaceDim | Mean     | Error    | StdDev   |
|--------------------------- |--------- |------------ |---------:|---------:|---------:|
| MultiplyByTranspose        | 3        | 1           | 237.7 ns |  3.23 ns |  2.86 ns |
| MultiplyByTranspose_OptInd | 3        | 1           | 127.9 ns |  1.17 ns |  0.98 ns |
| MultiplyByTranspose_OptLin | 3        | 1           | 122.5 ns |  1.15 ns |  0.96 ns |
| MultiplyByTranspose        | 3        | 2           | 372.0 ns |  5.78 ns |  5.40 ns |
| MultiplyByTranspose_OptInd | 3        | 2           | 212.6 ns |  2.05 ns |  1.82 ns |
| MultiplyByTranspose_OptLin | 3        | 2           | 208.2 ns |  0.75 ns |  0.58 ns |
| MultiplyByTranspose        | 3        | 3           | 529.5 ns | 10.56 ns | 15.14 ns |
| MultiplyByTranspose_OptInd | 3        | 3           | 300.1 ns |  2.98 ns |  2.64 ns |
| MultiplyByTranspose_OptLin | 3        | 3           | 302.5 ns |  5.87 ns |  7.21 ns |
| MultiplyByTranspose        | 4        | 1           | 384.3 ns |  1.47 ns |  1.30 ns |
| MultiplyByTranspose_OptInd | 4        | 1           | 201.0 ns |  2.47 ns |  2.19 ns |
| MultiplyByTranspose_OptLin | 4        | 1           | 199.3 ns |  2.46 ns |  2.30 ns |
| MultiplyByTranspose        | 4        | 2           | 627.1 ns |  2.91 ns |  2.43 ns |
| MultiplyByTranspose_OptInd | 4        | 2           | 348.2 ns |  3.51 ns |  2.93 ns |
| MultiplyByTranspose_OptLin | 4        | 2           | 342.1 ns |  4.78 ns |  3.99 ns |
| MultiplyByTranspose        | 4        | 3           | 899.9 ns |  6.18 ns |  4.83 ns |
| MultiplyByTranspose_OptInd | 4        | 3           | 492.1 ns |  1.39 ns |  1.23 ns |
| MultiplyByTranspose_OptLin | 4        | 3           | 485.8 ns |  2.82 ns |  2.50 ns |

| Method                     | spaceDim | subSpaceDim | Mean     | Error     | StdDev    |
|--------------------------- |--------- |------------ |---------:|----------:|----------:|
| MultiplyByTranspose_OptInd | 10       | 4           | 3.529 us | 0.0666 us | 0.0713 us |
| MultiplyByTranspose_OptLin | 10       | 4           | 3.407 us | 0.0156 us | 0.0138 us |

*/
