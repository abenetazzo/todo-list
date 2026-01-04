#!/bin/bash
dotnet test tests/Web.Api.Tests --no-build --collect:"XPlat Code Coverage" && \
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html \
  -title:"TodoList API Coverage" && \
open coveragereport/index.html
