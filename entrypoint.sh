#!/bin/sh

if [ -z "$(ls -A /app/Config)" ]; then
  cp -r /tmp/* /app/Config
fi

exec "$@"