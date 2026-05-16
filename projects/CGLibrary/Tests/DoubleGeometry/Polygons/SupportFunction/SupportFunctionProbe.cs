using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal sealed class SupportFunctionProbe : SupportFunction {

  public SupportFunctionProbe(IEnumerable<GammaPair> pairs, bool toSort = true) : base(pairs, toSort) { }

  public static bool CheckTripleDirect(GammaPair pm, GammaPair pc, GammaPair pp) => CheckTriple(pm, pc, pp);

}
