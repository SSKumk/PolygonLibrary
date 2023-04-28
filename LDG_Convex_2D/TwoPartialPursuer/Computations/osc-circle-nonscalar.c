// Name of the problem
ProblemName = "Oscillator; circular constraint for the 2nd player";

// Path to the folder whereto the result should be written
path = "./Osc-circle-nonscalar";


/* =================================================================================
  Block of data defining the dynamics of the game
*/
  
// Dimension of the phase vector
n = 2;

// The main matrix
A = { { 0.0, 1.0 }, { -1.0, 0.0 } };

// Dimension of the useful control
p = 1;

// The useful control matrix
B = { { 0.0 }, { 1.0 } };

// Dimension of the disturbance
q = 2;

// The disturbance matrix
C = { { 1.0, 0.0 }, { 0.0, 1.0 } };

// The initial instant
t0 = 0.0;

// The final instant
T = 4.0;

// The time step
dt = 0.05;

/* =================================================================================
  Block of data defining the constraint for the first player's control

  All angles in radians
  Type of the set:
    0 - box constraint; is defined by p x 2 array pBox
   
    1 - List of the vertices: PQnt PVert --> number_of_points and their_coordinates
          PQnt = 4;
          PVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };
          
    2 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
          PRectParallel = {x1, y1, x2, y2};
          
    3 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
          PRect = {x1, y1, x2, y2};
          PAngle = alpha;
          
    4 - Circle: x y R n angle -> abscissa ordinate radius number_of_vertices turn_angle
          PCenter = {x, y};
          PRadius = R;
          PQntVert = n;
          PAngle = alpha;

    5 - Ellipse: x y a b n angle angle_aux -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle
          PCenter = {x, y};
          PSemiaxes = {a, b};
          PQntVert = n;
          PAngle = alpha; 
          PAngleAux = beta; 
*/

// 0 - case example
PTypeSet = 0;
PBox =  { { -1.1, +1.1 } };


// 1 - case example
// PTypeSet = 1;
// PQnt = 4;
// PVert = { { 1.0, 1.0 }, { -1.0, 1.0 }, { -1.0, -1.0 }, { 1.0, -1.0 } };

// 2 - case example
// PTypeSet = 2;
// PRectParallel = {0.0, 0.0, 1.0, 1.0};

// 3 - case example
// PTypeSet = 3;
// PRect = {0.0, 0.0, 1.0, 1.0};
// PAngle = 3.0;

// 4 - case example
// PTypeSet = 4;
// PCenter = {0.0, 0.0};
// PRadius = 1.0;
// PQntVert = 50;
// PAngle = 1.0;

// 5 - case example   x y a b n phi a0
// PTypeSet = 5;
// PCenter = {0.0, 0.0};
// PSemiaxes = {1.0, 1.5};
// PQntVert = 30;
// PAngle = 0.0;
// PAngleAux = 0.0; 



/* =================================================================================
  Block of data defining the constraint for the second player's control

All angles in radians
  Type of the set: 
1 - List of the vertices: PQnt PVert --> number_of_points and their_coordinates
          PQnt = 4;
          PVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };
          
    2 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
          PRectParallel = {x1, y1, x2, y2};
          
    3 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
          PRect = {x1, y1, x2, y2};
          PAngle = alpha;
          
    4 - Circle: x y R n angle -> abscissa ordinate radius number_of_vertices turn_angle
          PCenter = {x, y};
          PRadius = R;
          PQntVert = n;
          PAngle = alpha;

    5 - Ellipse: x y a b n angle angle_aux -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle
          PCenter = {x, y};
          PSemiaxes = {a, b};
          PQntVert = n;
          PAngle = alpha; 
          PAngleAux = beta; 
*/  

// 0 - case example
// QTypeSet = 1;
// QQnt = 4;
// QVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

// 1 - case example
// QTypeSet = 2;
// QRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// QTypeSet = 3;
// QRect = {0.0, 0.0, 1.0, 1.0};
// QAngle = 3.0;

// 3 - case example
QTypeSet = 4;
QCenter = {0.0, 0.0};
QRadius = 1.1;
QQntVert = 100;
QAngle = 0.0;

// 4 - case example   x y a b n phi a0
// QTypeSet = 5;
// QCenter = {0.0, 0.0};
// QSemiaxes = {1.0, 0.5};
// QQntVert = 50;
// QAngle = 0.0;
// QAngleAux = 0.0; 


/*
    Q set partitioning
   
    0 - List of the vertices:  n array -> number_of_points and their coordinates: Q1 and Q2
        Q1Qnt = n1;    
        Q1Vert = {{qDim number}, ... n1-times};    
        Q2Qnt = n2;
        Q2Vert = {{qDim number}, ... n2-times};
    1 - k alternating partitioning: k array -> number_of_parts  [an array of vertex indices of size k x 2]
        QK = k;
        QAltParttitiong = { {ind1, ind2}, ... k-times };
    2 - rotating partitioning: ind1 ind2 step -> index_of_origin1 index_of_origin2 and step of rotating
        QInd1 = ind1;
        QInd2 = ind2;
        QStep = step;
 */

// QTypePart = 0;
// Q1Qnt = 2;
// Q1Vert = { {-1} , {0} };
// Q2Qnt = 2;
// Q2Vert = { {0} , {1} };

// QTypePart = 1;
// QK = 3;
// QAltParttitiong = { {0, 60}, {0, 60}, {25, 85} };

QTypePart = 2;
QInd1 = 0;
QInd2 = 50;
QStep = 1;

/*
 * Flag to write the partial tubes data
 * WriteQTubes = false | true;
*/
WriteQTubes = true;


/* =================================================================================
  Block of data defining the Terminal set

  All angles in radians
  Type of the set:
    1 - List of the vertices: PQnt PVert --> number_of_points and their_coordinates
          PQnt = 4;
          PVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };
          
    2 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
          PRectParallel = {x1, y1, x2, y2};
          
    3 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
          PRect = {x1, y1, x2, y2};
          PAngle = alpha;
          
    4 - Circle: x y R n angle -> abscissa ordinate radius number_of_vertices turn_angle
          PCenter = {x, y};
          PRadius = R;
          PQntVert = n;
          PAngle = alpha;

    5 - Ellipse: x y a b n angle angle_aux -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle
          PCenter = {x, y};
          PSemiaxes = {a, b};
          PQntVert = n;
          PAngle = alpha; 
          PAngleAux = beta; 
*/
//The projection coordinates
projI = 0;
projJ = 1;

// 0 - case example
MTypeSet = 1;
MQnt = 4;
MVert = { { 1.0, 1.0 }, { -1.0, 1.0 }, { -1.0, -1.0 }, { 1.0, -1.0 } };

// 1 - case example
// MTypeSet = 2;
// MRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// MTypeSet = 3;
// MRect = {0.0, 0.0, 1.0, 1.0};
// MAngle = 3.0;

// 3 - case example
// MTypeSet = 4;
// MCenter = {0.0, 0.0};
// MRadius = 1.0;
// MQntVert = 50;
// MAngle = 0.0;

// 4 - case example   x y a b n phi a0
// MTypeSet = 5;
// MCenter = {0.0, 0.0};
// MSemiaxes = {1.0, 0.5};
// MQntVert = 50;
// MAngle = 1.0;
// MAngleAux = 2.0; 




        
