using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

public static class AffineBasisAssert {

  public static void IsBasisOrthonormal(AffineBasis basis) => LinearBasisAssert.IsOrthonormal(basis.LinBasis);

}
