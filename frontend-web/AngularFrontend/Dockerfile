# Build Stage
FROM node:20 AS builder
WORKDIR /app

COPY package*.json ./
RUN npm install
RUN npm install @stomp/stompjs sockjs-client

COPY . .
RUN npx ng build --configuration production

# Serve Stage
FROM nginx:alpine
COPY --from=builder /app/dist/AngularFrontend/browser /usr/share/nginx/html
COPY config/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

