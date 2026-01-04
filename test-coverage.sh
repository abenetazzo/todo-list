#!/bin/bash
dotnet test --no-build --collect:"XPlat Code Coverage" && \
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html \
  -title:"TodoList API Coverage" && \
open coveragereport/index.html
