using System.Globalization;
using LDG;

namespace Trajectories;

public static class Trajectory {

  public static void Main() {
    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgPath  = "E:\\Work\\LDG\\";

    TrajectoryMain<double, DConvertor> traj = new TrajectoryMain<double, DConvertor>(ldgPath, "SimpleMotion.Test1");
    traj.CalcTraj("1");
    traj.CalcTraj("2");
  }

}
