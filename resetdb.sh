#!/bin/bash

rm -rf Migrations
rm web_project.db
dotnet ef migrations add InitialCreate
dotnet ef database update
