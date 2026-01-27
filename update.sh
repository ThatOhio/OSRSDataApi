#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

echo "Stopping the API stack..."
docker compose down

echo "Pulling the latest changes from Git..."
git pull

echo "Building and starting the API stack..."
docker compose up -d --build

echo "Cleaning up old images..."
docker image prune -f

echo "Update complete!"
