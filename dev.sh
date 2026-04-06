#!/usr/bin/env bash
# Stale dotnet processes on 5185 have been observed serving empty _framework/dotnet.js (Content-Length: 0),
# which leaves Blazor stuck on the loading spinner. This script frees the port then starts the app.
set -euo pipefail
PORT="${PORT:-5185}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"
if command -v lsof >/dev/null 2>&1; then
  pid="$(lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true)"
  if [[ -n "${pid:-}" ]]; then
    echo "Stopping process on port $PORT (PID $pid)..."
    kill "$pid" 2>/dev/null || true
    sleep 1
  fi
fi
exec dotnet run --launch-profile http "$@"
