#include "stdafx.h"
#include "CameraObject.h"

CameraObject::CameraObject()
{
	name = "ZeroCam";
	loc = "\pic";

	imgX = 1920;
	imgY = 1080;
}

string CameraObject::CaptureImage(string id)
{
	
}

void CameraObject::SetRes(int X, int Y)
{
	imgX = X;
	imgY = Y;
}

void CameraObject::SetLocation(string location)
{
	loc = location;
}