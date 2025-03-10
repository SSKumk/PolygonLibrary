// Name of the problem
ProblemName = "Simple Motion";


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
T = 10;

// The time step
dt = 0.1;

// The dimension of projected space
d = 3;

// The indices to project onto
projJ = {0, 1, 2};

// ==================================================
// PSetType = "Sphere";
// PTheta = 7;
// PPhi = 12;
// PCenter = {0,0,0};
// PRadius = 1;

PSetType = "RectAxisParallel";
PRectPLeft = {-1,-1,-1};
PRectPRight = {1,1,1};

// ==================================================
QSetType = "Sphere";
QTheta = 7;
QPhi = 12;
QCenter = {0,0,0};
QRadius = 0.9;

// QSetType = "RectAxisParallel";
// QRectPLeft = {-1.5,-1.5,-1.5};
// QRectPRight = {1.5,1.5,1.5};

// ==================================================
// The goal type of the game
// "Itself" - the game itself
// "Epigraph" - the game with epigraph of the payoff function
GoalType = "Itself";

// The type of the M
// "TerminalSet" - the explicit terminal set assignment. In Rd if goal type is "Itself", in R{d+1} if goal type is "Epigraph"
// "DistToOrigin" - the game with epigraph of the payoff function as distance to the origin.
// "DistToPolytop" - the game with epigraph of the payoff function as distance to the given polytop.
MType = "TerminalSet";

MSetType = "RectAxisParallel";
MRectPLeft = {-1,-1,-1};
MRectPRight = {1,1,1};
// ==================================================
