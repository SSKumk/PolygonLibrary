// Name of the problem
ProblemName = "Cubes3D";


// ==================================================

// Block of data defining the dynamics of the game
// Dimension of the phase vector
n = 3;

// The main matrix
A = {{0, 0, 0},{0, 0, 0},{0, 0, 0}};

// Dimension of the useful control
pDim = 3;

// The useful control matrix
B = {{1, 0, 0},{0, 1, 0},{0, 0, 1}};

// Dimension of the disturbance
qDim = 3;

// The disturbance matrix
C = {{1, 0, 0},{0, 1, 0},{0, 0, 1}};

// The initial instant
t0 = 0;

// The final instant
T = 1;

// The time step
dt = 0.2;

// The dimension of projected space
d = 3;

// The indices to project onto
projJ = {0, 1, 2};

// ==================================================
PSetType = "RectParallel";
PRectPLeft = {-1,-1,-1};
PRectPRight = {1,1,1};

// ==================================================
QSetType = "RectParallel";
QRectPLeft = {-0.5,-0.5,-0.5};
QRectPRight = {0.5,0.5,0.5};

// ==================================================
// The goal type of the game
// "Itself" - the game itself
// "Epigraph" - the game with epigraphic of the payoff function
GoalType = "Itself";

// The type of the M
// "TerminalSet" - the explicit terminal set assigment. In Rd if goal type is "Itself", in R{d+1} if goal type is "Epigraph"
// "DistToOrigin" - the game with epigraph of the payoff function as distance to the origin.
// "DistToPolytop" - the game with epigraph of the payoff function as distance to the given polytop.
MType = "TerminalSet";

MSetType = "Sphere";
MTheta = 4;
MPhi = 14;
MCenter = {0,0,0};
MRadius = 1;
// ==================================================
