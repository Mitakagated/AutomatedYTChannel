# AutomatedYTChannel
A simple program that searches .mp4 files in a user-defined folder (the default folder is the Videos folder in Windows.)
# How it works
The found .mp4 files are merged into a new .mp4 file in a folder that the user can specify, by default it is the Videos folder. When the .mp4 file is finished, a prompt to connect to the application using a Google account will appear.

After the connection is established successfully, the upload starts with displayed progress with default settings that the user can change. The configuration files are generated in the Documents folder with .json extension.

Be aware that in order to use the program, you need to have FFmpeg installed on your pc, otherwise it will crash. Also currently it can't be used by people that aren't added in the development testing. If you want to test it out for yourself, message me so I can add you.
## Used libraries
Xabe.FFmpeg - Used to integrate FFmpeg functionality in the program - link to the source https://ffmpeg.xabe.net
YouTube API - Used to upload the video to YouTube - link to the source https://developers.google.com/youtube/v3
