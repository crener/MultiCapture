import picamera

camera = picamera.PiCamera()

name = raw_input("Name: ")
camera.resolution = (1920, 1080)

run = True;
while run:
    command = raw_input("command: ")
    if command == "exit":
        camera.close();
        run = False;

    elif command == "flip":
        direction = raw_input("xf or vf? ")
        option = raw_input("setting: ")
        setting = True;

        if option == "true":
            setting = True;
        elif option == "false":
            setting = False;

        if direction == "xf":
            camera.hflip = setting
        elif direction == "vf":
            camera.vflip = setting

    elif command == "name":
        name = raw_input("name: ")

    elif command == "res":
        x = int(raw_input("x: "))
        y = int(raw_input("y: "))
        camera.resolution = (x, y)

    elif command == "cap":
        ident = raw_input("identity: ")
        camera.capture(name + ident + ".jpg")