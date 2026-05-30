#!/bin/bash

set -e

echo "=== Frontend ==="

cd /workspace/frontend

yarn install
yarn build

echo "=== Backend ==="

cd /workspace/backend/Web

dotnet tool restore
dotnet restore

dotnet ef database update \
	-p ../Infrastructure/Infrastructure.csproj \
	-s .

exec dotnet run --urls=http://0.0.0.0:5000
