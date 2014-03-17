#!/bin/bash
# stty -echo

DIR=$PWD
SOURCEDIR="./src"
BINARYDIR="./build_output"
BINARYDIRENDED=$DIR"/build_output/"
DEPLOYDIR="./ReleaseBinaries"

if [ ! -d $DEPLOYDIR ]; then
{
    mkdir $BINARYDIR
}
else
{
    rm -rf $BINARYDIR/*
}
fi

xbuild $SOURCEDIR/BellyRub/BellyRub.csproj /property:OutDir=$BINARYDIRENDED;Configuration=Release

if [ ! -d $DEPLOYDIR ]; then
{
    mkdir $DEPLOYDIR
}
else
{
    rm -rf $DEPLOYDIR/*
}
fi

cp $BINARYDIR/BellyRub.dll $DEPLOYDIR/
cp $BINARYDIR/Nancy.dll $DEPLOYDIR/
cp $BINARYDIR/Nancy.Hosting.Self.dll $DEPLOYDIR/
cp $BINARYDIR/websocket-sharp.dll $DEPLOYDIR/
cp $BINARYDIR/Newtonsoft.Json.dll $DEPLOYDIR/
