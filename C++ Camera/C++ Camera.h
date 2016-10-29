#pragma once

#include "libs\libmmal.so"
#include "libs\libmmal_components.so"
#include "libs\libmmal_core.so"
#include "libs\libmmal_omx.so"
#include "libs\libmmal_omxutil.so"
#include "libs\libmmal_util.so"
#include "libs\libmmal_vc_client.so"

#include <string>

using namespace std;
using namespace System;

namespace CCamera {

	public ref class Camera
	{
		void SetDirectory(string location);

		void SetCameraName(string name);

		void CaptureImage(string id);

		void SetRez(int x, int y);
		
	};
}
