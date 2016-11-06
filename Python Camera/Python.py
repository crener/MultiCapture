import clr
clr.AddReference("using/IronPython.Modules");

from ctypes import *
from time import sleep
import picamera

class python:
    camera = picamera.PiCamera()
    imgEnd = ".jpg"
    imgLocation = "/scanimage/"
    piName = "unknown"

    def captureAutomatic(self, codeName):
        camera.capture(imgLocation + piName + codeName + imgEnd)
        return imgLocation + piName + codeName + imgEnd

    def changeLocation(self, location):
        imgLocation = location
        return

    def setCamName(self, name):
        piName = name
        return name

    def sleepCamera(self, time):
        sleep(time)
        return

    def setResulution(self, x, y):
        camera.resolution = (x, y)
        return

    def shutdown(self):
        camera.close()
        return
