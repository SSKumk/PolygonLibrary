using CGLibrary;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public interface IDrawer {

  public void SaveFrame(Visualization.Facet Fs, List<Vector> Vs);

}
