using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public class TwoDimensional : BaseConvexPolyhedron {

  public override int                           Dim      => 2;
  public override ConvexPolyhedronType          Type     => ConvexPolyhedronType.TwoDimensional;
  public override HashSet<Point>                Vertices { get; }
  public override HashSet<BaseConvexPolyhedron> Faces    => throw new ArgumentException("Can not take faces of a plane polygon!");

  public TwoDimensional(HashSet<Point> Ps) { Vertices = Ps; }

  
  
}
