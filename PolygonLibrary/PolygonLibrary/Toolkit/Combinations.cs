using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public partial class Geometry<TNum, TConv>
  where TNum : class, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class for listing all combinations of length k from n elements. Zero-based.
  /// (3,2) --> [0,1]; [0,2]; [1,2];
  /// </summary>
  public class Combination {

    private readonly int _n;
    private readonly int _k;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private int[] _state;

    /// <summary>
    /// Initializes a combination with init state 0..k-1.
    /// </summary>
    /// <param name="n">The total number of elements.</param>
    /// <param name="k">The number of elements in each combination.</param>
    public Combination(int n, int k) {
#if DEBUG
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(k, 0);
      ArgumentOutOfRangeException.ThrowIfLessThan(n, k);
#endif
      _n     = n;
      _k     = k;
      _state = new int[k];
      for (int i = 0; i < k; i++) {
        _state[i] = i;
      }
    }

    /// <summary>
    /// Gets the element at the specified index in the current combination.
    /// </summary>
    /// <param name="ind">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the current combination.</returns>
    public int this[int ind] {
      get
        {
          Debug.Assert(ind >= 0 && ind < _k, $"Combination.indexer: Index must lie in [0,{_k - 1}]! Found {ind}.");

          return _state[ind];
        }
    }

    /// <summary>
    /// Generates the next combination in the sequence.
    /// </summary>
    /// <returns>True if the next combination was generated; if current combination is maximal one than false.</returns>
    public bool Next() {
      for (int i = _k - 1, l = _n - 2; i >= 0; i--, l--) {
        if (_state[i] <= l) {
          _state[i]++;
          for (int j = i + 1; j < _k; j++) {
            _state[j] = _state[j - 1] + 1;
          }

          return true;
        }
      }

      return false;
    }
  }
}
