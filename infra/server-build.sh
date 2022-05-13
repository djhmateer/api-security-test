#!/bin/sh

# Script to configure production server

# Create a clean VM
# Proxmox, shutdown VM, backup, restore

# need nopasswd enabled
# https://superuser.com/questions/138893/scp-to-remote-server-with-sudo
# copy secrets onto machine

# rsync -e 'ssh -p 31' --rsync-path="sudo rsync" secrets/kestrel.service  dave@pfsense:/etc/systemd/system/kestrel.service

# connect to the VM
# ssh pfsense -p 30 

# git clone https://github.com/djhmateer/api-security-test ; sudo chmod +x ~/api-security-test/infra/server-build.sh ; ./api-security-test/infra/server-build.sh

sudo apt update -y
sudo apt upgrade -y

# Install packages for .NET for Ubutu 20.04 LTS
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

## Install .NET SDK  as we're doing a build
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0

# root for webapi
# sudo mkdir /var/www

cd /home/dave/api-security-test

# compile and publish the webapp
sudo dotnet publish /home/dave/api-security-test/ --configuration Release --output /var/www

# change ownership of the published files to what it will run under
sudo chown -R www-data:www-data /var/www
# allow exective permissions
sudo chmod +x /var/www

### OTHER
## kesrel.sevice needs to be copied by now from filezilla
## /etc/systemd/system/kestrel.service



# auto start on machine reboot
# sudo systemctl enable kestrel.service

# logs are in /var/log/syslog
# look for dotnet-api


# rename the machine
sudo hostnamectl set-hostname api-security-test

# sudo reboot now
