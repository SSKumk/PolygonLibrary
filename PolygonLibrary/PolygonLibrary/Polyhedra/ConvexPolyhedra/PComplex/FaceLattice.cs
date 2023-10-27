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

  public class FaceLattice {

    public HashSet<Point> Points { get; init; }

    public ConvexPolytop Maximum { get; init; }


    public FaceLattice(HashSet<Point> Ps, ConvexPolytop Max) {
      Points  = Ps;
      Maximum = Max;
    }


  }

  // /// <summary>
  // /// The node of the face lattice.
  // /// </summary>
  // public class FaceLatticeNode {
  //
  //   /// <summary>
  //   /// The dimensional of the associated polytop.
  //   /// </summary>
  //   public int Dim => Polytop.PolytopDim;
  //
  //   /// <summary>
  //   /// Reference to associated polytop to this node.
  //   /// </summary>
  //   public ConvexPolytop Polytop { get; } //todo Может ли быть null? если нужен узел с пустым многогранником, то как его представлять?
  //
  //   /// <summary>
  //   ///
  //   /// </summary>
  //   public HashSet<Point> Vertices => Polytop.Vertices;
  //
  //   /// <summary>
  //   /// The list of the super-nodes, which Dim = this.Dim + 1.
  //   /// </summary>
  //   public List<FaceLatticeNode> Super { get; set; }
  //
  //   /// <summary>
  //   /// The list of the sub-nodes, which Dim = this.Dim - 1.
  //   /// </summary>
  //   public List<FaceLatticeNode> Sub { get; set; }
  //
  //   /// <summary>
  //   /// Construct the node of the face lattice.
  //   /// </summary>
  //   /// <param name="P">The associated polytop.</param>
  //   public FaceLatticeNode(ConvexPolytop P) {
  //     Polytop = P;
  //     Sub     = new List<FaceLatticeNode>();
  //     Super   = new List<FaceLatticeNode>();
  //   }
  //
  // }

}
