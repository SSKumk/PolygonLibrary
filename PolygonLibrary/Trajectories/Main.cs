using System.Globalization;
using LDG;

namespace Trajectories;

public class Trajectory {

  public static void Main() {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    TrajectoryMain<double, DConvertor> traj = new TrajectoryMain<double, DConvertor>(ldgPath, "SimpleMotion.Test1");
    traj.CalcTraj("1");
    traj.CalcTraj("2");
  }

}
