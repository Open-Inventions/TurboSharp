#!/bin/sh
cd TurboDot
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true
dotnet publish -r win-x64   -p:PublishSingleFile=true --self-contained true
