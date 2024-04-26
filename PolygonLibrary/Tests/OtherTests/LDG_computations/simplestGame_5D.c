// Name of the problem
ProblemName = "Cubes5D";


// ==================================================

// Block of data defining the dynamics of the game
// Dimension of the phase vector
n = 5;

// The main matrix
A = {{0, 0, 0, 0, 0},{0, 0, 0, 0, 0},{0, 0, 0, 0, 0},{0, 0, 0, 0, 0},{0, 0, 0, 0, 0}};

// Dimension of the useful control
pDim = 5;

// The useful control matrix
B = {{1, 0, 0, 0, 0},{0, 1, 0, 0, 0},{0, 0, 1, 0, 0},{0, 0, 0, 1, 0},{0, 0, 0, 0, 1}};

// Dimension of the disturbance
qDim = 5;

// The disturbance matrix
C = {{1, 0, 0, 0, 0},{0, 1, 0, 0, 0},{0, 0, 1, 0, 0},{0, 0, 0, 1, 0},{0, 0, 0, 0, 1}};

// The initial instant
t0 = 0;

// The final instant
T = 1;

// The time step
dt = 0.2;

// The dimension of projected space
d = 5;

// The indices to project onto
projJ = {0, 1, 2, 3, 4};

// ==================================================
PSetType = "RectParallel";
PRectPLeft = {-1,-1,-1,-1,-1};
PRectPRight = {1,1,1,1,1};

// ==================================================
QSetType = "Ellipsoid";
QTheta = 20;
QPhi = 50;
QCenter = {0,0,0,0,0};
QSemiaxesLength = {1,0.5,2,3,0.75};

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

MSetType = "RectParallel";
MRectPLeft = {-2,-2,-2,-2,-2};
MRectPRight = {2,2,2,2,2};

// ==================================================
