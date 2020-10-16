#!/bin/bash
script_folder=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
exec "$script_folder/ShellSpeak/ShellSpeak.exe" 0</dev/null &

