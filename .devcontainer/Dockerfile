FROM mcr.microsoft.com/devcontainers/python:1-3.12-bullseye

# Remove broken Yarn apt repo so apt-get update works
RUN rm -f /etc/apt/sources.list.d/yarn.list /etc/apt/sources.list.d/yarn*.list \
 && rm -f /usr/share/keyrings/yarn*.gpg /etc/apt/trusted.gpg.d/yarn*.gpg \
 && apt-get update
