// Name of the problem
ProblemName = "Inertial point; square target set";

/* =================================================================================
  Block of data defining the initial point
*/
  
// Initial time instant 
ts = 0.0;

/* Initial point type:
     0 - original coordinates (its dimension is equal to the dimension
         of the phase vector given in the basic data of the example)
     1 - equivalent (its dimension is equal to 2)
*/
initPointType = 1;

// The initial vector
xs = { 0.0, 0.0 };

// =================================================================================
// The first player control data

/* The first player control type:
     0 - extremal shift control on the basis of computed system of bridges
     1 - switching line control (if the control is one dimensional or the constraints
         are of box type)
     2 - constant
     3 - random within the given constraint multiplied by a coefficient
*/
pApplContrType = 1;

// =================================================================================
// The second player control data

/* The second player control type:
     0 - extremal shift control on the basis of computed system of bridges
     1 - switching line control (if the control is one dimensional or the constraints
         are of box type)
     2 - constant
     3 - random within the given constraint multiplied by a coefficient
*/
qApplContrType = 1;
