# https://hub.docker.com/layers/library/postgres/18.3-alpine3.23/images/sha256-9972eae7320b9ceb9eca76ecb2ab4aad58132d80edd5664dcb957f2795bb8a48
FROM postgres:18.3-alpine3.23

COPY ./docker-entrypoint-initdb.d/* /docker-entrypoint-initdb.d/
