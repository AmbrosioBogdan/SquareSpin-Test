#!/bin/bash

# Square Spin WebGL Build Script - COMPLETE VERSION
UNITY_PATH="/home/bogdan/Unity/Hub/Editor/6000.3.11f1/Editor/Unity"
PROJECT_PATH="/home/bogdan/Square Spin"
BUILD_PATH="$PROJECT_PATH/Build/WebGL/final"

echo "🎮 WebGL Build - Complete"

# Libera memoria
sync
echo 3 | sudo tee /proc/sys/vm/drop_caches > /dev/null 2>&1

# Rimuovi vecchio build
rm -rf "$BUILD_PATH"
mkdir -p "$BUILD_PATH"

# Build con Unity
"$UNITY_PATH" \
  -projectPath "$PROJECT_PATH" \
  -buildTarget WebGL \
  -quit \
  -batchmode \
  -nographics \
  -logFile - \
  --build-target WebGL \
  --build-path "$BUILD_PATH"

echo "✅ BUILD COMPLETATO!"
ls -lh "$BUILD_PATH"
