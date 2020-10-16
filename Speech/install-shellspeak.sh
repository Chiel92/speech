#!/bin/bash

# Move to the directory of this script
script_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$script_path"

rm -r ~/bin/ShellSpeak
cp -r ./ShellSpeak/bin/Debug/net48/ ~/bin/ShellSpeak
cp ./ShellSpeak/shellspeak.sh ~/bin/shellspeak.sh
