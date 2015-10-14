#pragma once
#include "RoundRobin.h"
#include "Process.h"

class Scheduler {
public:
	Scheduler(const std::vector<Process>& processes);
	void scheduleTimeSlice();
	void removeProcess(const Process& process);
private:
	RoundRobin<Process> rr;
};

Scheduler::Scheduler(const std::vector<Process>& processes) {
	for (auto& process : processes) {
		rr.add(process);
	}
}

void Scheduler::scheduleTimeSlice() {
	try {
		rr.getNext().doWorkDuringTimeSlice();
	} catch (const std::out_of_range&) {
		std::cerr << "no more processes to schedule" << std::endl;
	}
}

void Scheduler::removeProcess(const Process& process) {
	rr.remove(process);
}

