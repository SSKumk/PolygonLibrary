using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a one-dimensional edge of a convex polytope expressed in k-space.
  /// </summary>
  public class SubTwoDimensionalEdge : BaseSubCP {

    /// <summary>
    /// The first vertex of the edge.
    /// </summary>
    private SubPoint fst;

    /// <summary>
    /// The second vertex of the edge.
    /// </summary>
    private SubPoint snd;

    /// <summary>
    /// Gets the dimension of the edge. It equal to 1.
    /// </summary>
    public override int PolytopDim => 1;

    /// <summary>
    /// Gets the type of the edge. It is OneDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.OneDimensional;

    /// <summary>
    /// There are no Faces of the 1-dimensional edge.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence => null;

    /// <summary>
    /// There are two vertices of the 2D-edge.
    /// </summary>
    public override HashSet<SubPoint> Vertices => _vertices ??= new HashSet<SubPoint>() { fst, snd };

    /// <summary>
    /// There are two faces that matches with Vertices of the 2D-edge.
    /// </summary>
    public override HashSet<BaseSubCP> Faces =>
      _faces ??= new HashSet<BaseSubCP>() { new SubZeroDimensional(fst), new SubZeroDimensional(snd) };

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensionalEdge"/> class base on ZeroDimensional.
    /// </summary>
    /// <param name="first">The first vertex of the edge.</param>
    /// <param name="second">The second vertex of the edge.</param>
    public SubTwoDimensionalEdge(SubPoint first, SubPoint second) {
      fst = first;
      snd = second;
    }

    // /// <summary>
    // /// Converts the edge to the previous space.
    // /// </summary>
    // /// <returns>The converted edge in the previous space.</returns>
    // public override BaseSubCP ToPreviousSpace() => new SubTwoDimensionalEdge
    //   ((SubZeroDimensional)Faces.First().ToPreviousSpace(), (SubZeroDimensional)Faces.Last().ToPreviousSpace());

    /// <summary>
    /// Projects the edge to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis to project to.</param>
    /// <returns>The projected edge.</returns>
    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      return new SubTwoDimensionalEdge
        (new SubPoint(fst.ProjectTo(aBasis), fst, fst.Original), new SubPoint(snd.ProjectTo(aBasis), snd, snd.Original));
    }

  }

}
