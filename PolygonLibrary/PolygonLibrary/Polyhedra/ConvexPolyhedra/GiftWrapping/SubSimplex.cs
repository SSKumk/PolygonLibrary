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
  /// Represents a simplex, which is a convex polytop with (d + 1) vertices in d-dimensional space.
  /// </summary>
  public class SubSimplex : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the simplex.
    /// </summary>
    public override int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the convex polytop.
    /// </summary>
    public override SubCPType Type => SubCPType.Simplex;

    /// <summary>
    /// Gets the set of vertices of the simplex.
    /// </summary>
    public override HashSet<SubPoint> Vertices { get; }


    /// <summary>
    /// The set of (d-1)-dimensional faces of a simplex, which are in turn a Simplex.
    /// </summary>
    private HashSet<BaseSubCP>? _faces = null;

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the simplex, which are in turn a Simplex.
    /// </summary>
    public override HashSet<BaseSubCP> Faces {
      get
        {
          if (_faces is null) {
            _faces = new HashSet<BaseSubCP>();

            foreach (SubPoint vertex in Vertices) {
              SubPoint[] faceVert = Vertices.Where(v => !v.Equals(vertex)).ToArray();

              switch (PolytopDim) {
                case 2: _faces.Add(new SubTwoDimensionalEdge(faceVert[0], faceVert[1]));

                  break;
                case 3: {
                  AffineBasis plane = new AffineBasis(faceVert);
//todo А надо ли? Если да, то как правильно задать обход, ведь в пространстве высокой размерности уже как-то не так?

                  SubPoint2D       fst   = new SubPoint2D(faceVert[0].ProjectTo(plane));
                  SubPoint2D       snd   = new SubPoint2D(faceVert[1].ProjectTo(plane));
                  SubPoint2D       frd   = new SubPoint2D(faceVert[2].ProjectTo(plane));
                  _faces.Add
                    (
                     Convexification.IsLeft(fst,snd,frd)
                       ? new SubTwoDimensional(new List<SubPoint>() { faceVert[0], faceVert[1], faceVert[2] })
                       : new SubTwoDimensional(new List<SubPoint>() { faceVert[0], faceVert[2], faceVert[1] })
                    );

                  break;
                }
                default: _faces.Add(new SubSimplex(faceVert));

                  break;
              }
            }
          }

          return _faces;
        }
    }

    /// <summary>
    /// Information about face incidence.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence { get; }


    /// <summary>
    /// Lifts a simplex to the previous space.
    /// </summary>
    /// <returns>A simplex in (d+1)-space.</returns>
    public override BaseSubCP ToPreviousSpace() => new SubSimplex(Vertices.Select(v => v.Parent)!);

    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      return new SubSimplex(Vertices.Select(s => new SubPoint(s.ProjectTo(aBasis), s, s.Original)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubSimplex"/> class with the specified set of vertices and dimension.
    /// </summary>
    /// <param name="simplex">The set of vertices of the simplex.</param>
    /// <param name="faces">The set of faces of the simplex.</param>
    /// <param name="incidence">Information about face incidence.</param>
    public SubSimplex(IEnumerable<SubPoint> simplex, HashSet<BaseSubCP>? faces = null, SubIncidenceInfo? incidence = null) {
      Debug.Assert(simplex.Count() >= 3, $"The simplex must have at least three points! Found {simplex.Count()}.");

      PolytopDim    = simplex.Count() - 1;
      Vertices      = new HashSet<SubPoint>(simplex);
      _faces        = faces;
      FaceIncidence = incidence;
    }

  }

}
