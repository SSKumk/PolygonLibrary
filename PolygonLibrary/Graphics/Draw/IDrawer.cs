using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public interface IDrawer {



  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<Visualization.Facet> facets);

  // public void AddComment(();

}
