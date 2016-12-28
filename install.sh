#!/bin/bash

sudo apt-get update
sudo apt-get upgrade -y

sudo apt-get update
sudo apt-get install ssh mono-complete -y --force-yes

sudo apt-get update
sudo mkdir /scanimage
sudo chown pi:pi /scanimage
