using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public interface IDrawer {

  public void SaveFrame(string path, IEnumerable<Visualization.Facet> facets, IEnumerable<Vector> vertices);

}
