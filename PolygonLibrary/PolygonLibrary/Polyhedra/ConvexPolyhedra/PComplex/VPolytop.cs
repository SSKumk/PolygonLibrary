using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

 // todo Надо определиться или сидим на / IEnumerable, IReadOnlyCollection, IReadOnlyList .. / или вообще без них.
 // ToArray(). ToList() -- НЕ круто!
  public class VPolytop {
    public HashSet<Vector> Vertices { get; }

    public VPolytop(HashSet<Vector> Vs) {
      Vertices = Vs;
    }

    private int? _hash;
    public override int GetHashCode() {
      if (_hash is null) {
        List<int> sortedVs = Vertices.Order().Select(v => v.GetHashCode()).ToList();
        int hash = sortedVs.First();
        for (int i = 1; i < sortedVs.Count; i++) {
          hash = HashCode.Combine(hash, sortedVs[i]);
        }
        _hash = hash;
      }
      return _hash.Value;
    }

    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      VPolytop other = (VPolytop)obj;

      return this.Vertices.SetEquals(other.Vertices);
    }

    public override string ToString() => $"Hash = {GetHashCode()}\n" + string.Join('\n', Vertices.Order()) + "\n";
  }

}