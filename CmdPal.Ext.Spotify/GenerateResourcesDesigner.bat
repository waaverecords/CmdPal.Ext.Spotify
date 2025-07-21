 call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
 cd "%~dp0\Properties"
 resgen Resources.resx CmdPal.Ext.Spotify.Properties.Resources.resources /str:CSharp,CmdPal.Ext.Spotify.Properties,Resources,Resources.Designer.cs