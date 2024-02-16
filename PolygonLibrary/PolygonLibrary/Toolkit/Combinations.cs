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
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class for enumerating all combinations. Supports enumeration.
  /// </summary>
  public class Combinations : IEnumerable<int[]> {

    private readonly int   _n;
    private readonly int   _k;
    private readonly int[] _state;

    /// <summary>
    /// Initializes a combination with init state 0..k-1.
    /// </summary>
    /// <param name="n">The total number of elements.</param>
    /// <param name="k">The number of elements in each combination.</param>
    public Combinations(int n, int k) {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(k, 0);
      ArgumentOutOfRangeException.ThrowIfLessThan(n, k);
      _n     = n;
      _k     = k;
      _state = Enumerable.Range(0, k).ToArray();
    }

    /// <summary>
    /// Gets the element at the specified index in the current combination.
    /// </summary>
    /// <param name="ind">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the current combination.</returns>
    public int this[int ind] {
      get
        {
          Debug.Assert(ind >= 0 && ind < _k, $"Combinations.indexer: Index must lie in [0,{_k - 1}]! Found {ind}.");

          return _state[ind];
        }
    }

    /// <summary>
    /// Generates the next combination in the sequence.
    /// </summary>
    /// <returns>True if the next combination was generated; if current combination is maximal one than false.</returns>
    private bool Next() {
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

    /// <summary>
    /// Returns the current combination and moves to the next one.
    /// </summary>
    /// <returns>A copy of the current combination.</returns>
    public int[] GetCombination() {
      int[] state = (int[])_state.Clone();
      Next();

      return state;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the combinations.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the combinations.</returns>
    public IEnumerator<int[]> GetEnumerator() {
      do {
        yield return (int[])_state.Clone();
      } while (Next());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  }

}
