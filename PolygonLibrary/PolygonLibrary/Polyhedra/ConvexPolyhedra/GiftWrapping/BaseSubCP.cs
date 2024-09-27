using System.Collections;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Type of temporary storage of face incidence information
  /// </summary>
  internal class TempIncidenceInfo : SortedDictionary<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> { }

  // /// <summary>
  // /// Type of permanent storage of face incidence information.
  // /// For each pair (F1, F2) of incident faces, it is assumed that HashCode(F1) is less or equal than HashCode(F2)
  // /// </summary>
  // internal class SubIncidenceInfo : SortedDictionary<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> { }

  // /// <para><b>Simplex</b> - the polytop is a simplex.</para>
  /// <summary>
  /// <b>SubPolytop</b> - the polytop is a complex structure.
  /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon.</para>
  /// <b>OneDimensional</b> - the polytop is a 1D-segment.
  /// </summary>
  public enum SubCPType {

    SubSimplex
  , SubPolytop
  , TwoDimensional
  , OneDimensional
  , ZeroDimensional

  }

  /// <summary>
  /// Represents the d-dimensional convex polytop
  /// </summary>
  internal abstract class BaseSubCP : IComparable<BaseSubCP> {

    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim => Vertices.First().SpaceDim;

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public abstract int PolytopDim { get; }

    /// <summary>
    /// <para><b>Simplex</b> - the polytop is a simplex.</para>
    /// <b>SubPolytop</b> - the polytop is a complex structure.
    /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon.</para>
    /// <b>OneDimensional</b> - the polytop is a 1D-segment.
    /// </summary>
    public abstract SubCPType Type { get; }

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public abstract SortedSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Returns set of original points.
    /// </summary>
    public SortedSet<Vector> OriginalVertices {
      get { return _originalVertices ??= Vertices.Select(v => v.GetRootVertex()).ToSortedSet(); }
    }

    private SortedSet<Vector>? _originalVertices;

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the polytop.
    /// </summary>
    public abstract List<BaseSubCP>? Faces { get; }

    /// <summary>
    /// Gets the set of (d+1)-dimensional faces of the polytop.
    /// </summary>
    public virtual List<BaseSubCP> SuperFaces { get; } = new List<BaseSubCP>();

    /// <summary>
    /// The outward normal of the (d-1)-dimensional polytop (face).
    /// </summary>
    public Vector Normal { get; set; }


    /// <summary>
    /// Converts current d-dimensional polytop in d-dimensional space to d-dimensional polytop in (d+1)-dimensional space.
    /// Assumed that the corresponding parents of vertices are exist.
    /// </summary>
    /// <returns>The d-dimensional polytop in (d+1)-dimensional space.</returns>
    public abstract BaseSubCP ToPreviousSpace();


    public abstract BaseSubCP ProjectTo(AffineBasis affBasis);


    /// <summary>
    /// Compares the current instance with another object of type BaseSubCP.
    /// </summary>
    /// <param name="other">The BaseSubCP object to compare with.</param>
    /// <returns>
    /// <para>Returns '-1' if the number of vertices in 'this' is less than that of 'other'.</para>
    /// Returns '+1' if the number of vertices in 'this' is greater than that of 'other'.
    /// <para>Otherwise, it returns the result based on a lexicographical comparison of their elements.</para>
    /// If all corresponding elements are equal, then the sets are considered equal.
    /// </returns>
    public int CompareTo(BaseSubCP? other) {
      if (other is null) { return 1; } // null < this (always)

      // Debug.Assert(this.PolytopDim == other.PolytopDim, "BaseSubCP: The dimensions of the polytopes must be equal.");
      // Debug.Assert(this.SpaceDim == other.SpaceDim, "BaseSubCP: The dimensions of the spaces must be equal.");


      if (this.Vertices.Count < other.Vertices.Count) { // this < other
        return -1;
      }

      if (this.Vertices.Count > other.Vertices.Count) { // this > other
        return 1;
      }


      SortedSet<SubPoint>.Enumerator e1       = this.Vertices.GetEnumerator();
      SortedSet<SubPoint>.Enumerator e2       = other.Vertices.GetEnumerator();
      Comparer<SubPoint>             comparer = Comparer<SubPoint>.Default;

      while (e1.MoveNext() && e2.MoveNext()) {
        int compare = comparer.Compare(e1.Current, e2.Current);
        switch (compare) {
          case < 0: // this < other
            return -1;
          case > 0: // this > other
            return 1;
        }
      }

      return 0;
    }

    public static bool operator <=(BaseSubCP p1, BaseSubCP p2) => p1.CompareTo(p2) <= 0;
    public static bool operator >=(BaseSubCP p1, BaseSubCP p2) => p1.CompareTo(p2) >= 0;

    /// <summary>
    /// Determines whether the specified object is equal to convex polytop.
    /// Two polyhedra are equal if they have the same dimensions and the sets of their vertices are equal.
    /// </summary>
    /// <param name="obj">The object to compare with convex polytop.</param>
    /// <returns>True if the specified object is equal to convex polytop, False otherwise</returns>
    public override bool Equals(object? obj) {
      throw new InvalidOperationException($"BaseSubCP: Equals method is not supported for BaseSubCP");

      // if (obj == null || this.GetType() != obj.GetType()) {
      //   return false;
      // }
      //
      // BaseSubCP other = (BaseSubCP)obj;
      //
      // if (this.PolytopDim != other.PolytopDim) {
      //   return false;
      // }
      //
      // if (this.SpaceDim != other.SpaceDim) {
      //   return false;
      // }
      //
      // return this.Vertices.SetEquals(other.Vertices);
    }

    public override int GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(Vertices.Count);

  }

}
