#!/bin/sh

# Script to configure production server

# Create a clean VM
# Proxmox, shutdown VM, backup, restore

# connect to the VM
# ssh pfsense -p 30 

# git clone https://github.com/djhmateer/api-security-test ; sudo chmod +x ~/api-security-test/infra/server-build.sh ; ./api-security-test/infra/server-build.sh

# Use Filezilla to copy secrets 

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
mkdir /var/www

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



# sudo reboot now
