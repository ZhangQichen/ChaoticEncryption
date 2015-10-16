/*
Reference:
	"A Wheel-Switch Chaotic System for Image Encryption"
By
	Yue Wu, Joseph P. Noonan, Department of Electrical and Computer Engineering, Tufts University.
	Sos Agaian, Department of Electrical and Computer Engineering line, University of Texas at San Antonio, San Antonio.
*/

#ifndef WHEELSWITCHCHAOTICSYSTEM_H
#define WHEELSWITCHCHAOTICSYSTEM_H

#include <cmath>

namespace WSCS // stands for wheel switch chaotic system.
{
	const double PI = std::asin(1) * 2.0;
	typedef unsigned int UInt;
	/*
	parameters:
	x_n: previous value. Within (0, 1).
	r: contorlling parameter. Within (3.57, 4).
	*/
	double LogisticMap(double x_n, double r);
	/*
	parameters:
	x_n: previous value. Within (0, 1).
	u: contorlling parameter. Within (sqrt(2), 2).
	*/
	double TentMap(double x_n, double u);
	/*
	parameters:
	x_n: previous value. Within (0, 1).
	a: contorlling parameter.
	*/
	double SineMap(double x_n, double a);
	/*
	parameters:
	x_n: previous value. Within (0, 1).
	q: corresponding value of controlling sequence Q. Starts from 0.
	r: controlling parameter of logistic map.
	*/
	double WheelSwitchFunction(double x_n, UInt q, double r);

}

#endif