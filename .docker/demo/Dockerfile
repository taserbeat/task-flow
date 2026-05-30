FROM mcr.microsoft.com/dotnet/sdk:10.0

RUN apt update -y && \
    apt install -y \
    curl \
    git \
    vim \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Node.js 24
RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - \
    && apt-get update \
    && apt-get install -y nodejs

# Yarn
RUN corepack enable

WORKDIR /workspace

COPY entrypoint.sh /entrypoint.sh

RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
