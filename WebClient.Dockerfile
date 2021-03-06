FROM node:alpine AS builder

WORKDIR /app

COPY frontend .

RUN npm i
RUN npm i -g @angular/cli
RUN npm run build:prod

FROM nginx:alpine

COPY --from=builder /app/dist/* /usr/share/nginx/html/
COPY --from=builder /app/dist/assets/* /usr/share/nginx/html/assets/
COPY --from=builder /app/nginx.conf /etc/nginx/nginx.conf
