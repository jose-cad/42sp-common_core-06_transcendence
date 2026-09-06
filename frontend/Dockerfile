# Exemplo/ponto de partida — ajustar quando o projeto React existir de verdade.
# Assume build com Vite (pasta de saida "dist"); trocar se usarem outra ferramenta.
# nginx.conf ainda precisa ser criado (TODO) para servir com HTTPS.

FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json .
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80 443
