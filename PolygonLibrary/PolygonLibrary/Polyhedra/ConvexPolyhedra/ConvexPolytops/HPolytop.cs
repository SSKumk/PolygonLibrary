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

  public class HPolytop {

    public int SpaceDim => Faces.First().Normal.Dim;

    public List<HyperPlane> Faces { get; } // если будет надо, то AVL-Сергей Сергеевича

    public HPolytop(List<HyperPlane> Fs) { Faces = Fs; }

  }

}
