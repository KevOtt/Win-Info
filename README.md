# Win-Info
Remote System Info Tool For Windows and Windows Server

## About

This is a small tool intended as a slightly more graphical version of the Windows System Information tool, used for gathering configuration information on a remote Windows system.

The tool uses a remote WMI connection to gather configuration and hardware information on a specific remote Windows system and display it in an easy to read format, with emphasis on things admins usually care about the most (i.e. free disk space, uptime, etc.).  Additional configuration information on running processes, services, and installed Windows updates can also be pulled.


## Download
The tool is provided as a standalone .exe file in a .zip archive.

|Release|Link                |
|-------|--------------------|
|v1.0   |[Win-Info-v1.0.zip][Win-Info-v1.0]|

[Win-Info-v1.0]: https://github.com/KevOtt/Win-Info/releases/download/v1.0/Win-Info.v1.0.zip

## How to Use

The .exe can be launched as a standalone application. Any ip address or system name can be entered under "Target System" and click "Connect" to initiat the WMI call to the system.  If you want to connect with alternate credentials, change the credential option to "Used Saved" and either initiate the conenction or click "Prompt" to be prompted for credentials to use. Depending on the system, the conneciton and data querying may take some time. Additional queries for running processes, services, and installed updates can be initiated from the top menu bar once connected. Once connected, the relevant system info is displayed. The tool should work with both Windows desktop and server systems.

The tool does require that WMI ports be open to the end points and that the user has access to run basic WMI queries against the system. WMI connections require port 135 and RPC ports to be opened, if you have no firewalls enabled you should have no issues.

## Screenshots

Example server connection:
<p align="center">
  <img src="/docs/Screenshots/Screenshot1.png" width="700" title="Screenshot">
</p>

Another Example:
<p align="center">
  <img src="/docs/Screenshots/Screenshot2.png" width="700" title="Screenshot">
</p>


Example services query
<p align="center">
  <img src="/docs/Screenshots/Screenshot3.png.png" width="700" title="Screenshot">
</p>

## License

Win Info is licenced under the [MIT license][].

[MIT license]: https://github.com/KevOtt/AD-Lab-Generator/blob/master/LICENSE