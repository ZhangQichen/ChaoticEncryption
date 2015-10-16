#include "WheelSwitchChaoticSystem.h"
#include <cmath>

double WSCS::LogisticMap(double x_n, double r)
{
	return r * x_n * (1 - x_n);
}

double WSCS::TentMap(double x_n, double u)
{
	return x_n < 0.5 ? u * x_n : u * (1 - x_n);
}

double WSCS::SineMap(double x_n, double a)
{
	return a*std::sin(x_n * PI);
}

double WSCS::WheelSwitchFunction(double x_n, UInt q, double r)
{
	double value = 0;
	switch (q)
	{
	case 0:
		value = LogisticMap(x_n, r);
		break;
	case 1:
		value = TentMap(x_n, r / 2);
		break;
	case 2:
		value = SineMap(x_n, r / 4);
		break;
	default:
		break;
	}
	return value;
}