FROM mariadb:10.11

RUN apt-get update \
    && apt-get install -y git curl \
    && curl -L https://dl.min.io/client/mc/release/linux-amd64/mc -o /usr/local/bin/mc \
    && chmod +x /usr/local/bin/mc \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /workspace
