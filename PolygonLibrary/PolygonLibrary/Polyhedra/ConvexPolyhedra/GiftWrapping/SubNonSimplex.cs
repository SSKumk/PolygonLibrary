using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The polytop that is not a simplex in k-dimensional space (3 and higher dimension).
  /// </summary>
  public class SubNonSimplex : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public override int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the convex polytop.
    /// </summary>
    public override SubCPType Type => SubCPType.NonSimplex;

    /// <summary>
    /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
    /// The second face can be equal to null if it is not constructed yet.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence { get; }

    /// <summary>
    /// Construct a new instance of the <see cref="SubNonSimplex"/> class based on it's faces.
    /// </summary>
    /// <param name="faces">Faces to construct the convex polytop</param>
    /// <param name="incidence">Information about face incidence.</param>
    /// <param name="Vs"></param>
    public SubNonSimplex(HashSet<BaseSubCP> faces, SubIncidenceInfo incidence, HashSet<SubZeroDimensional>? Vs = null) {
      PolytopDim = faces.First().PolytopDim + 1;
      _faces     = faces;
      _vertices  = Vs; //todo возможно ли, что _vertices != null, а Vs = null и всё сломалось?

      // SubIncidenceInfo faceIncidence = new SubIncidenceInfo();
      //
      // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in incidence) {
      //   faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
      // }

      FaceIncidence = incidence;
    }

    // /// <summary>
    // /// Converts the polytop to the previous space.
    // /// </summary>
    // /// <returns>The converted polytop in the previous space.</returns>
    // public override BaseSubCP ToPreviousSpace() {
    //   HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>(Faces.Select(F => F.ToPreviousSpace()));
    //   // SubIncidenceInfo      info  = new SubIncidenceInfo();
    //
    //   // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in FaceIncidence!) {
    //   //   info.Add(pair.Key.ToPreviousSpace(), (pair.Value.F1.ToPreviousSpace(), pair.Value.F2.ToPreviousSpace()));
    //   // }
    //
    //   return new SubNonSimplex(faces, FaceIncidence!);
    // }


    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      IEnumerable<SubZeroDimensional> Vs = Vertices.Select
        (s => new SubZeroDimensional(new SubPoint(s.Vertex.ProjectTo(aBasis), s.Vertex, s.Vertex.Original), s.Primal));
      HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>(Faces.Select(F => F.ProjectTo(aBasis)));

      return new SubNonSimplex(faces, FaceIncidence!, new HashSet<SubZeroDimensional>(Vs));
    }

  }

}
