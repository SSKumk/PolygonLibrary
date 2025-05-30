using System.Globalization;
using DoubleDouble;
using LDG;

namespace Trajectories;

public static class Trajectory {

  public static void Main() {
    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgPath  = "E:\\Work\\LDG\\";

    double epsD = 1e-08;
    double epsDD = 1e-15;

    // TrajectoryMain<double, DConvertor> traj = new TrajectoryMain<double, DConvertor>(ldgPath, "SimpleMotion.Test1", 1e-08);
    // TrajectoryMain<ddouble, DDConvertor> traj = new TrajectoryMain<ddouble, DDConvertor>(ldgPath, "MassDot", epsDD);
    TrajectoryMain<ddouble, DDConvertor> traj = new TrajectoryMain<ddouble, DDConvertor>(ldgPath, "Oscillator", epsDD);
    // TrajectoryMain<ddouble, DDConvertor> traj = new TrajectoryMain<ddouble, DDConvertor>(ldgPath, "Oscillator2D", epsDD);

    // traj.CalcTraj("1", true);
    // traj.CalcTraj("2", true);
    // traj.CalcTraj("3", true);
    // traj.CalcTraj("4", true);
    // traj.CalcTraj("5", true);

    traj.CalcTraj("OO#0.01");
    traj.CalcTraj("ZO#0.01");
    traj.CalcTraj("CO#0.01");
    traj.CalcTraj("RO#0.01");
    traj.CalcTraj("OZ#0.01");
    traj.CalcTraj("OC#0.01");
    traj.CalcTraj("OR#0.01");


  }

}
