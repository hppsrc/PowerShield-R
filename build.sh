#!/bin/bash
CONFIG="Debug"
[ "$1" = "r" ] && CONFIG="Release"

echo "$CONFIG build..."
dotnet publish -c "$CONFIG" -r win-x64 --self-contained true -o "out"