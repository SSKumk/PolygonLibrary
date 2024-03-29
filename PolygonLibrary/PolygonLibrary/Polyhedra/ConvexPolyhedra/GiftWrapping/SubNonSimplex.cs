using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The polytop that is not a simplex in d-dimensional space (3 and higher dimension).
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
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public override HashSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the polytop.
    /// </summary>
    public override HashSet<BaseSubCP> Faces { get; }


    /// <summary>
    /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
    /// The second face can be equal to null if it is not constructed yet.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence { get; }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override BaseSubCP ToPreviousSpace() {
      HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>(Faces.Select(F => F.ToPreviousSpace()));
      // SubIncidenceInfo      info  = new SubIncidenceInfo();

      // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in FaceIncidence!) {
      //   info.Add(pair.Key.ToPreviousSpace(), (pair.Value.F1.ToPreviousSpace(), pair.Value.F2.ToPreviousSpace()));
      // }

      return new SubNonSimplex(faces, FaceIncidence!);
    }

    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      IEnumerable<SubPoint> Vs = Vertices.Select(s => s.ProjectTo(aBasis));
      HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>(Faces.Select(F => F.ProjectTo(aBasis)));

      return new SubNonSimplex(faces, FaceIncidence!, new HashSet<SubPoint>(Vs));
    }

    /// <summary>
    /// Construct a new instance of the <see cref="SubNonSimplex"/> class based on it's faces.
    /// </summary>
    /// <param name="faces">Faces to construct the convex polytop</param>
    /// <param name="incidence">Information about face incidence.</param>
    /// <param name="Vs">Vertices of this convex polytop. If null then its construct base on faces.</param>
    public SubNonSimplex(HashSet<BaseSubCP> faces, SubIncidenceInfo incidence, HashSet<SubPoint>? Vs = null) {
      PolytopDim = faces.First().PolytopDim + 1;
      Faces = faces;

      // SubIncidenceInfo faceIncidence = new SubIncidenceInfo();
      //
      // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in incidence) {
      //   faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
      // }

      if (Vs is null) {
        Vs = new HashSet<SubPoint>();

        foreach (BaseSubCP face in faces) {
          Vs.UnionWith(face.Vertices);
        }
      }

      FaceIncidence = incidence;
      Vertices = Vs;
    }

  }

}
