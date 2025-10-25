#!/bin/bash

# Generar un tag único basado en fecha y hora
TAG="demo-api:$(date +%Y%m%d%H%M%S)"
DOCKERHUB_TAG="soabel/poc-net-api:$(date +%Y%m%d%H%M%S)"

# Eliminar todos los contenedores basados en la imagen demo-api
docker ps -a --filter ancestor=demo-api --format "{{.ID}}" | xargs -r docker rm -f

# Eliminar la imagen local demo-api si existe
docker rmi demo-api || true

# Construir la imagen nuevamente con el tag único
docker build -t $TAG .

# Etiquetar la imagen para Docker Hub con el tag único
docker tag $TAG $DOCKERHUB_TAG

# Ejecutar un nuevo contenedor (opcional, descomentado si se desea probar)
# docker run -d --name demo-api-container -p 8080:80 $TAG

# Publicar la imagen en Docker Hub con el tag único
docker push $DOCKERHUB_TAG