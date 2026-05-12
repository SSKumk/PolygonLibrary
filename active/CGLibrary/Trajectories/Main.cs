using System.Globalization;
using DoubleDouble;
using LDG;

namespace Trajectories;

public static class Trajectory {

  public static void Main() {
    // string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string ldgDir  = "E:\\Work\\LDG\\";

    // double eps = double.Parse("1e-8");
    ddouble eps = ddouble.Parse("1e-15");

    // string problem = "Oscillator-cone6";
    // string problem = "Oscillator-mass-cone6";
    string problem = "Oscillator3D-mass-cone6";

    // TrajectoryMain<double, DConvertor>   traj = new TrajectoryMain<double, DConvertor>(ldgDir, "SimpleMotion.Test1", eps);
    TrajectoryMain<ddouble, DDConvertor> traj = new TrajectoryMain<ddouble, DDConvertor>(ldgDir, problem, eps);

    traj.CalcTraj("OO#0.01");
    // traj.CalcTraj("ZO#0.01");
    // traj.CalcTraj("CO#0.01");
    // traj.CalcTraj("RO#0.01");
    // traj.CalcTraj("OZ#0.01");
    // traj.CalcTraj("OC#0.01");
    // traj.CalcTraj("OR#0.01");


  }

}
