@echo off
rem start doc server
docfx --serve --port 4037
rem open in browser
start "" http://localhost:4037/