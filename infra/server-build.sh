#!/bin/sh

# Script to configure production server

# Create a clean VM
# Proxmox, shutdown VM, backup, restore

# connect to the VM
# ssh pfsense -p 30 

# git clone https://github.com/djhmateer/auto-archiver ;  sudo chmod +x ~/auto-archiver/infra/server-build.sh ; ./auto-archiver/infra/server-build.sh

# Use Filezilla to copy secrets 

sudo apt update -y
sudo apt upgrade -y

wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

## Install .NET Runtime
## This is the SDK
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0

cd /home/dave



# **USE same strategy as https://osr4rightstools.org/
# cron to restart app

sudo reboot now
