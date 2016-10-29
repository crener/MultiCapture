#pragma once
#include <string>

using namespace std;

class CameraObject
{
public:
	CameraObject();

	string CaptureImage(string id);

	void SetRes(int X, int Y);
	void SetLocation(string);

private:
	string name;
	string loc;
	int imgX, imgY;
};