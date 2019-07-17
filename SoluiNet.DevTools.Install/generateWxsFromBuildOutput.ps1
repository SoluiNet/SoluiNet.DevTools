$wixToolsetVersion = "3.14";

$scriptPath = Split-Path $MyInvocation.MyCommand.Path;
$releaseFolder = "${scriptPath}/../SoluiNet.DevTools.UI/bin/net461/AnyCPU/Release";
# $releaseFolder = Resolve-Path "../SoluiNet.DevTools.UI/bin/net461/AnyCPU/Release";

& "${env:ProgramFiles(x86)}\WiX Toolset ${wixToolsetVersion}\bin\heat.exe" project "${releaseFolder}" -pog:Binaries -ag -template:fragment -out "${scriptPath}/GeneratedFromHeatProject.wxs"