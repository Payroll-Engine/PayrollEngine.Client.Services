@echo off

rem start doc build
:docfxBuild
echo Building static html reference...
echo.
docfx -t statictoc

rem open in browser
:open
start "" %~dp0_site/index.html