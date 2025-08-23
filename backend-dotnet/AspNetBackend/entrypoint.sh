#!/bin/sh
echo "waiting for db to become ready"
until pg_isready -h postgres-db -p 5432 -U quiz-db; do
  sleep 2
done
echo "database is ready to connect"

exec dotnet AspNetBackend.dll
