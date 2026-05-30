# * * * * * * * * * * * * * * * * * * * *
# Build Stage
# * * * * * * * * * * * * * * * * * * * *

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

RUN apt update -y && \
    apt install -y \
    curl \
    git \
    vim \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Node.js 24
RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - \
    && apt update -y \
    && apt install -y nodejs

# Yarn
RUN corepack enable

COPY . .

# Build frontend
WORKDIR /source/frontend
RUN yarn install
RUN yarn build

# Build backend
WORKDIR /source/backend/Web
RUN dotnet publish Web.csproj -c Release -o /app/TaskFlow --runtime linux-x64 --no-self-contained
RUN mkdir -p /app/TaskFlow/privateroot
RUN mkdir -p /app/TaskFlow/wwwroot
RUN cp -R /source/frontend/dist/. /app/TaskFlow/privateroot


# * * * * * * * * * * * * * * * * * * * *
# Deploy Stage
# * * * * * * * * * * * * * * * * * * * *

# https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/net-core-net-framework-containers/official-net-docker-images
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/TaskFlow ./

CMD [ "dotnet", "./task-flow.dll" ]
