#include <vector>
#include <iostream>
#include <string>
#include "Scheduler.h"

using namespace std;

void printVector(vector<int> vec) {
	for (const auto& element : vec) {
		cout << element << " ";
	}
	cout << endl;
}

vector<int> createVectorOfSize(size_t size) {
	vector<int> vec(size);
	int contents = 0;
	for (auto& i : vec) {
		i = contents++;
	}
	return vec;
}

class Element {
public:
	Element(int i, const string& str)
		: mI(i), mStr(str) {}

	string getStr() const {
		return mStr;
	}
private:
	int mI;
	string mStr;
};

void testVector() {
	vector<Element> vec;
	Element myElement(12, "Twelve");
	vec.push_back(move(myElement));
	vec.push_back({ 13, "Thirty" });
	vec.emplace_back(14, "Fourty");
	for (const auto& element : vec) {
		cout << element.getStr() << endl;
	}
}

int main() {
	vector<Process> processes = { Process("1"), Process("2"), Process("3") };
	Scheduler sched(processes);
	for (int i = 0; i < 4; i++) {
		sched.scheduleTimeSlice();
	}
	sched.removeProcess(processes[1]);
	cout << "Removed second process" << endl;
	for (int i = 0; i < 4; ++i) {
		sched.scheduleTimeSlice();
	}
	return 0;
}