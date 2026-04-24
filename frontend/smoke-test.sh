#!/usr/bin/env bash
set -e

BASE_URL="${BASE_URL:-https://amia-lise-cdfb4395c796.herokuapp.com}"

echo "Testing $BASE_URL"

echo "1. Front root..."
curl -fsS "$BASE_URL/" > /dev/null
echo "OK /"

echo "2. Swagger..."
curl -fsS "$BASE_URL/swagger/index.html" > /dev/null
echo "OK swagger"

echo "3. Swagger JSON..."
curl -fsS "$BASE_URL/swagger/v1/swagger.json" > /dev/null
echo "OK swagger json"

echo "Smoke tests passed."