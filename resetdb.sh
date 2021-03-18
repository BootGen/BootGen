#!/bin/bash
if test -d "Migrations"; then
  rm -rf Migrations
fi
if test -f "editor.db"; then
  rm editor.db
fi
dotnet ef migrations add InitialCreate
dotnet ef database update
