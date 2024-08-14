# Warning

This is very much an alpha program and power tool. Use with caution.

## Description

GUI Utility to set up virtual file systems and launch a given executable with it.  
Uses https://github.com/MildlyFootwear/usvfsWrap which was built to use https://github.com/ModOrganizer2/usvfs  



## Usage

I made the UI as intuitive as I could.  
Launch Instance-Manager.exe.  
If it doesn't detect any saved profiles, it will create a default profile.  
Click the Link Directory button in the center upper portion of the UI and set it up as needed for your desired game/usecase.  
Go to Manage Executables, then add your desired executable.  
Return to the main UI and click Launch at the center bottom to launch the executable with the VFS set up of the selected/displayed profile.  

If you intend to run the program through Steam, configure the launch arguments in Steam to be...  

``"(insert path to Instance Manager)\Instance-Manager.exe" -quicklaunch %command%``  

This will change Instance Manager's behavior to just give you a dropdown to select a profile then proceed to launch the game with the selected profile.

### Launch Arguments

* ```-debug``` will have IM launch with a console window to show various printouts as various functions are executed.  
* ```-quicklaunch``` will replace the regular UI with up to two dropdowns, one for selecting a profile and another for selecting an executable, before launching the selected executable.  
* When using quicklaunch, you can pass the name of a profile to skip the profile drop down. You can also pass the path of an executable to skip the executable dropdown.

## Credits

MO2 team and all contributors to usvfs.  
Rosiecow on Discord for the application icon.  
