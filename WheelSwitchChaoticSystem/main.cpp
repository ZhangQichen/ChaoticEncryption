#include "WheelSwitchChaoticSystem.h"
#include <fstream>

int main()
{
	std::ofstream out("test.csv");
	double x_0 = 0.5;
	double r = 3.9;
	WSCS::UInt cnt = 1;
	WSCS::UInt Q[3] = { 2, 0, 1 };
	while (cnt--)
	{
		out << (x_0 = WSCS::WheelSwitchFunction(x_0, Q[cnt % 3], r)) << '\n';
	}
}