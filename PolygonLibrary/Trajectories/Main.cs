using System.Globalization;
using DoubleDouble;
using LDG;

namespace Trajectories;

public static class Trajectory {

  public static void Main() {
    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgPath  = "E:\\Work\\LDG\\";

    double eps1 = 1e-08;
    ddouble eps2 = 1e-16;

    TrajectoryMain<double, DConvertor> traj = new TrajectoryMain<double, DConvertor>(ldgPath, "SimpleMotion.Test1", 1e-08);
    traj.CalcTraj("1");
    traj.CalcTraj("2");
  }

}
