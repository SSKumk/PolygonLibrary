// Name of the problem
ProblemName = "Cubes2D";


// ==================================================

// Block of data defining the dynamics of the game
// Dimension of the phase vector
n = 2;

// The main matrix
A = {{0, 0},{0, 0}};

// Dimension of the useful control
pDim = 2;

// The useful control matrix
B = {{1, 0},{0, 1}};

// Dimension of the disturbance
qDim = 2;

// The disturbance matrix
C = {{1, 0},{0, 1}};

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
PSetType = "RectAxisParallel";
PRectPLeft = {-1,-1};
PRectPRight = {1,1};

// ==================================================
QSetType = "RectAxisParallel";
QRectPLeft = {-0.5,-0.5};
QRectPRight = {0.5,0.5};

// ==================================================
// The goal type of the game
// "Itself" - the game itself
// "Epigraph" - the game with epigraphic of the payoff function
GoalType = "Epigraph";

// The type of the M
// "TerminalSet" - the explicit terminal set assigment. In Rd if goal type is "Itself", in R{d+1} if goal type is "Epigraph"
// "DistToOrigin" - the game with epigraph of the payoff function as distance to the origin.
// "DistToPolytop" - the game with epigraph of the payoff function as distance to the given polytop.
MType = "DistToOrigin";

// MSetType = "RectAxisParallel";
// MRectPLeft =  {-1,-1,-1};
// MRectPRight = {1,1,1};

// MVsQnt = 4;
// MPolytop = {{0,0},{1,0},{0,1},{1,1}};

MBallType = "Ball_oo";
// MTheta = 4;
// MPhi = 20;
MCMax = 5;

// ==================================================
