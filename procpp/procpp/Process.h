#pragma once
#include <string>
#include <iostream>
class Process {
public:
	Process(const std::string& name) : mName(name) {}
	void doWorkDuringTimeSlice() {
		std::cout << "Process " << mName << " performing work during time slice." << std::endl;
	}
	bool operator==(const Process& rhs) {
		return mName == rhs.mName;
	}
private:
	std::string mName;
};