// Name of the problem
ProblemName = "Oscillator-1-1";


// ==================================================

// Block of data defining the dynamics of the game
// Dimension of the phase vector
n = 2;

// The main matrix
A = {{0, 1},{-1, 0}};

// Dimension of the useful control
p = 1;

// The useful control matrix
B = {{0}, {1}};

// Dimension of the disturbance
q = 1;

// The disturbance matrix
C = {{1}, {0}};

// The initial instant
t0 = 0;

// The final instant
T = 1;

// The time step
dt = 0.2;

// The dimension of projected space
d = 2;

// The indices to project onto
projJ = {0, 1};

// ==================================================
PSetType = "RectParallel";
PRectPLeft = {-1};
PRectPRight = {1};

// ==================================================
QSetType = "RectParallel";
QRectPLeft = {-1};
QRectPRight = {1};

// ==================================================
GoalType = Epigraph;
MType = "DistToOrigin";

MBallType = "Ball_2";
MTheta = 4;
MPhi = 15;
MCMax = 5;

// ==================================================
