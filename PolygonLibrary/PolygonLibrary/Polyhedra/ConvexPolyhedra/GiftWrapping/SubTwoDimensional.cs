using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents convex two-dimensional polygon in a two-dimensional space.
  /// </summary>
  public class SubTwoDimensional : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the polygon. It equal to 2.
    /// </summary>
    public override int PolytopDim => 2;

    /// <summary>
    /// Gets the type of the polygon. It is TwoDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.TwoDimensional;

    /// <summary>
    /// Gets the vertices of the polygon.
    /// </summary>
    public override HashSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the list of vertices of the polygon.
    /// </summary>
    public List<SubPoint> VerticesList { get; }

    /// <summary>
    /// Gets the faces of the polygon.
    /// </summary>
    public override HashSet<BaseSubCP>? Faces { get; }

    /// <summary>
    /// There are no such information needed: if necessary, the neighborhood of 2d faces is established trivially.
    /// For GW-algorithm this information is needless.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensional"/> class.
    /// </summary>
    /// <param name="Vs">The list of vertices of the polygon given in clockwise or counter-clockwise order.</param>
    public SubTwoDimensional(List<SubPoint> Vs) {
      Debug.Assert
        (
         Vs.Count > 2
       , $"GW-->SubTwoDimensional: At least three points must be used to construct a TwoDimensional! Found {Vs.Count}"
        );

      List<SubPoint> vertices = new List<SubPoint>(Vs);
      Vertices = new HashSet<SubPoint>(vertices);
      VerticesList = vertices;

      HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>() { new SubTwoDimensionalEdge(Vs[^1], Vs[0]) };

      for (int i = 0; i < Vs.Count - 1; i++) {
        faces.Add(new SubTwoDimensionalEdge(Vs[i], Vs[i + 1]));
      }
      Faces = faces;
    }

    /// <summary>
    /// Converts the polygon to the previous space.
    /// </summary>
    /// <returns>The converted polygon in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() {
      List<SubPoint> Vs = new List<SubPoint>(VerticesList.Select(v => v.Parent)!);

      return new SubTwoDimensional(Vs);
    }

    /// <summary>
    /// Projects the polygon to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis to project to.</param>
    /// <returns>The projected polygon.</returns>
    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      return new SubTwoDimensional(VerticesList.Select(s => s.ProjectTo(aBasis)).ToList());
    }

  }

}
