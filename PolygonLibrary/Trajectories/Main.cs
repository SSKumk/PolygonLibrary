using System.Globalization;
using DoubleDouble;
using LDG;

namespace Trajectories;

public static class Trajectory {

  public static void Main() {
    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgPath  = "E:\\Work\\LDG\\";

    double epsD = 1e-08;
    ddouble epsDD = 1e-16;

    // TrajectoryMain<double, DConvertor> traj = new TrajectoryMain<double, DConvertor>(ldgPath, "SimpleMotion.Test1", 1e-08);
    TrajectoryMain<ddouble, DDConvertor> traj = new TrajectoryMain<ddouble, DDConvertor>(ldgPath, "Oscillator", epsDD);

    // traj.CalcTraj("1");
    // traj.CalcTraj("2");
    traj.CalcTraj("3");
  }

}
