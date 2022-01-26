#!/usr/bin/env nix-shell
#!nix-shell -i bash -p dotnet-sdk_6 nuget-to-nix mktemp

depsFile=$(realpath ./deps.nix)
nugetPkgs=$(mktemp -d)

dotnet restore ./ProgramRunner --packages "$nugetPkgs"

nuget-to-nix "$nugetPkgs" > "$depsFile"