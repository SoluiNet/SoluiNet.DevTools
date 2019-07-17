$wixToolsetVersion = "3.14";

$scriptPath = Split-Path $MyInvocation.MyCommand.Path;
$releaseFolder = "${scriptPath}/../SoluiNet.DevTools.UI/bin/net461/AnyCPU/Release";
# $releaseFolder = Resolve-Path "../SoluiNet.DevTools.UI/bin/net461/AnyCPU/Release";

& "${env:ProgramFiles(x86)}\WiX Toolset v${wixToolsetVersion}\bin\heat.exe" dir "${releaseFolder}" -gg -sfrag -template:fragment -out "${scriptPath}/GeneratedFromHeatDirectory.wxs"