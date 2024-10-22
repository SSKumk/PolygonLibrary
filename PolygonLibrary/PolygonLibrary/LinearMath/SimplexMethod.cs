namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // Ax <= b, A \in R^m x R^d; x \in R^d; b \in R^m
  public class SimplexMethod {

    private          TNum[,] _A;
    private          TNum[]  _b;
    private          TNum[]  _c;
    private readonly int     _d;
    private readonly int     _dOrig;
    private readonly int     _m;

    public SimplexMethod(List<HyperPlane> HPs, Func<int, TNum> fc) : this
      ((i, j) => HPs[i].Normal[j], HPs.Count, HPs.First().Normal.SpaceDim, i => HPs[i].ConstantTerm, fc) { }

    public SimplexMethod(Func<int, int, TNum> fA, int m, int d, Func<int, TNum> fb, Func<int, TNum> fc) {
      _m     = m;
      _dOrig = d;
      _d     = 2 * d + _m;
      _b     = new TNum[_m];
      for (int i = 0; i < _m; i++) {
        _b[i] = fb(i);
      }
      _A = new TNum[_m, _d];
      for (int i = 0; i < _m; i++) {
        for (int j = 0, l = 0; j < d; j++, l += 2) {
          _A[i, l]     = fA(i, j);
          _A[i, l + 1] = -fA(i, j);
        }
      }
      _c = new TNum[_d];
      for (int i = 0, l = 0; i < d; i++, l += 2) {
        _c[l]     = fc(i);
        _c[l + 1] = -fc(i);
      }
    }

    public SimplexMethodResult Solve() {
      //todo А как правильно активные неравенства находить?! Сейчас activeInequalities работает неверно!
      (SimplexMethodResultStatus status, TNum value, TNum[]? x, IEnumerable<int> activeInequalities) = SimplexInAugmentForm();
      if (status is not SimplexMethodResultStatus.Ok) {
        return new SimplexMethodResult(status);
      }

      TNum[] res = new TNum[_dOrig];
      int    nN  = 2 * _dOrig;
      for (int i = 0, l = 0; l < nN; i++, l += 2) {
        res[i] = x![l] - x[l + 1];
      }

      return new SimplexMethodResult(status, value, res, activeInequalities);
    }

    // Ax = b, x >= 0
    private SimplexMethodResult SimplexInAugmentForm() {
      HashSet<int> N = new HashSet<int>();
      int          d = _d - _m;
      for (int i = 0; i < d; i++) {
        N.Add(i);
      }
      HashSet<int> B  = new HashSet<int>();
      int[]        id = new int[_d + 1];
      for (int i = d; i < _d; i++) {
        B.Add(i);
        id[i] = i - d;
      }

      int     l;
      int     e;
      TNum    v = Tools.Zero;
      TNum[,] ANew;
      TNum[]  cCur;
      TNum[]  bNew = new TNum[_m];
      if (!_b.All(Tools.GE)) {
        ANew = new TNum[_m, _d + 1];
        for (int i = 0; i < _m; i++) {
          for (int j = 0; j < _d; j++) {
            ANew[i, j] = _A[i, j];
          }
          ANew[i, _d] = Tools.MinusOne;
        }
        _A         = new TNum[_m, _d + 1];
        (_A, ANew) = (ANew, _A);

        cCur     = new TNum[_d + 1];
        cCur[_d] = Tools.MinusOne;
        TNum[] cNew = new TNum[_d + 1];
        cNew[_d] = Tools.MinusOne;

        N.Add(_d);

        e = _d;
        l = -1;
        foreach (int i in B) {
          if (Tools.LT(_b[id[i]]) && (l == -1 || _b[id[l]] > _b[id[i]])) {
            l = i;
          }
        }

        v = Tools.Zero;
        Pivot
          (
           N
         , B
         , ref _A
         , ref _b
         , ref cCur
         , ref ANew
         , ref bNew
         , ref cNew
         , id
         , ref v
         , l
         , e
          );

        while (N.Any(i => Tools.GT(cCur[i]))) {
          e = -1;
          foreach (int i in N) {
            if (Tools.GT(cCur[i])) {
              e = i;

              break;
            }
          }

          l = -1;
          foreach (int i in B) {
            if (Tools.GT(_A[id[i], e]) && (l == -1 || _b[id[l]] / _A[id[l], e] > _b[id[i]] / _A[id[i], e])) {
              l = i;
            }
          }

          if (l == -1) {
            return new SimplexMethodResult(SimplexMethodResultStatus.Unlimited);
          }
          Pivot
            (
             N
           , B
           , ref _A
           , ref _b
           , ref cCur
           , ref ANew
           , ref bNew
           , ref cNew
           , id
           , ref v
           , l
           , e
            );
        }

        if (Tools.LT(v)) {
          return new SimplexMethodResult(SimplexMethodResultStatus.NoSolution);
        }

        if (B.Contains(_d)) {
          B.Remove(_d);
          e = -1;
          foreach (int j in N) {
            if (!Tools.EQ(_A[id[_d], j])) {
              e = j;

              break;
            }
          }
          TNum s = _A[id[_d], e];
          for (int j = 0; j < _d; j++) {
            _A[id[_d], j] /= s;
          }
          _A[id[_d], e] = Tools.Zero;
          foreach (int i in B) {
            s = -_A[id[i], e];
            for (int j = 0; j < _d; j++) {
              _A[id[i], j] += s * _A[id[_d], j];
            }
            _A[id[i], e] = Tools.Zero;
          }
          N.Remove(e);
          B.Add(e);
          id[e] = id[_d];
        }
        else { N.Remove(_d); }

        cCur = new TNum[_d];
        for (int i = 0; i < _d; i++) {
          if (!B.Contains(i)) {
            cCur[i] += _c[i];
          }
          else {
            for (int j = 0; j < _d; j++) {
              cCur[j] -= _c[i] * _A[id[i], j];
            }
            v += _c[i] * _b[id[i]];
          }
        }
        _c = cCur;

        ANew = new TNum[_m, _d];
        for (int i = 0; i < _m; i++) {
          for (int j = 0; j < _d; j++) {
            ANew[i, j] = _A[i, j];
          }
        }
        _A = ANew;
      }


      ANew = new TNum[_m, _d];
      cCur = new TNum[_d];
      while (N.Any(i => Tools.GT(_c[i]))) {
        e = -1;
        foreach (int i in N) {
          if (Tools.GT(_c[i])) {
            e = i;

            break;
          }
        }

        l = -1;
        foreach (int i in B) {
          if (Tools.GT(_A[id[i], e]) && (l == -1 || _b[id[l]] / _A[id[l], e] > _b[id[i]] / _A[id[i], e])) {
            l = i;
          }
        }

        if (l == -1) {
          return new SimplexMethodResult(SimplexMethodResultStatus.Unlimited);
        }
        Pivot
          (
           N
         , B
         , ref _A
         , ref _b
         , ref _c
         , ref ANew
         , ref bNew
         , ref cCur
         , id
         , ref v
         , l
         , e
          );
      }

      TNum[] x = CalcPoint(_b, _d - _m, B, id);
      IEnumerable<int> activeInq = CalcActiveInequalities(_d - _m, B, id);

      return new SimplexMethodResult(SimplexMethodResultStatus.Ok, v, x, activeInq);
    }

    private static TNum[] CalcPoint(TNum[] b, int k, HashSet<int> B, int[] id) {
      TNum[] x = new TNum[k];
      for (int i = 0; i < k; i++) {
        if (B.Contains(i)) {
          x[i] = b[id[i]];
        }
      }

      return x;
    }

    private static IEnumerable<int> CalcActiveInequalities(int k, HashSet<int> B, int[] id) {
      List<int> activeInq = new List<int>();
      for (int i = 0; i < k; i++) {
        if (B.Contains(i)) {
          activeInq.Add(id[i]);
        }
      }

      return activeInq;
    }

    private static void Pivot(
        HashSet<int> N
      , HashSet<int> B
      , ref TNum[,]  A
      , ref TNum[]   b
      , ref TNum[]   c
      , ref TNum[,]  ANew
      , ref TNum[]   bNew
      , ref TNum[]   cNew
      , IList<int>   id
      , ref TNum     v
      , int          l
      , int          e
      ) {
      id[e] = id[l];

      bNew[id[e]] = b[id[l]] / A[id[l], e];

      foreach (int j in N) {
        if (j != e) {
          ANew[id[e], j] = A[id[l], j] / A[id[l], e];
        }
      }
      ANew[id[e], l] = Tools.One / A[id[l], e];

      foreach (int i in B) {
        if (i != l) {
          bNew[id[i]] = b[id[i]] - A[id[i], e] * bNew[id[e]];
          foreach (int j in N) {
            if (j != e) {
              ANew[id[i], j] = A[id[i], j] - A[id[i], e] * ANew[id[e], j];
            }
          }
          ANew[id[i], l] = -A[id[i], e] * ANew[id[e], l];
        }
      }

      v += c[e] * bNew[id[e]];
      foreach (int j in N) {
        if (j != e) {
          cNew[j] = c[j] - c[e] * ANew[id[e], j];
        }
      }
      cNew[l] = -c[e] * ANew[id[e], l];

      // swap
      (A, ANew) = (ANew, A);
      (b, bNew) = (bNew, b);
      (c, cNew) = (cNew, c);

      N.Remove(e);
      N.Add(l);
      B.Remove(l);
      B.Add(e);
    }

    public class SimplexMethodResult {

      public SimplexMethodResultStatus Status { get; }

      public TNum Value { get; }

      public TNum[]? Solution { get; } = null;

      public IEnumerable<int> ActiveInequalitiesID { get; }

      public SimplexMethodResult(SimplexMethodResultStatus status, TNum value, TNum[]? solution, IEnumerable<int> activeInequalitiesId) {
        Status             = status;
        Value              = value;
        Solution           = solution;
        ActiveInequalitiesID = activeInequalitiesId;
      }

      public SimplexMethodResult(SimplexMethodResultStatus status) { Status = status; }

      public void Deconstruct(out SimplexMethodResultStatus status, out TNum value, out TNum[]? solution, out IEnumerable<int>
                                activeInequalities
        ) {
        status             = Status;
        value              = Value;
        solution           = Solution;
        activeInequalities = ActiveInequalitiesID;
      }

    }

    public enum SimplexMethodResultStatus { Ok, NoSolution, Unlimited }

  }

}
// public static SimplexMethodResult Simplex(TNum[,] A, TNum[] b, TNum[] c) {
//   int    m    = A.GetLength(0);
//   int    n    = A.GetLength(1);
//   TNum[] bNew = new TNum[m];
//   for (int i = 0; i < m; i++) {
//     bNew[i] = b[i];
//   }
//   int     nNew = 2 * n + m;
//   TNum[,] ANew = new TNum[m, nNew];
//   for (int i = 0; i < m; i++) {
//     for (int j = 0, l = 0; j < n; j++, l += 2) {
//       ANew[i, l]     = A[i, j];
//       ANew[i, l + 1] = -A[i, j];
//     }
//   }
//   TNum[] cNew = new TNum[nNew];
//   for (int i = 0, l = 0; i < n; i++, l += 2) {
//     cNew[l]     = c[i];
//     cNew[l + 1] = -c[i];
//   }
//
//   (SimplexMethodResultStatus status, TNum value, TNum[]? x) = SimplexInAugmentForm(ANew, bNew, cNew);
//   if (status is not SimplexMethodResultStatus.Ok) {
//     return new SimplexMethodResult(status);
//   }
//
//   TNum[] res = new TNum[n];
//   int    nN  = 2 * n;
//   for (int i = 0, l = 0; l < nN; i++, l += 2) {
//     res[i] = x![l] - x[l + 1];
//   }
//
//   return new SimplexMethodResult(status, value, res);
// }
