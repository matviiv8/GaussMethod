FROM node:14 as build-step
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build

FROM nginx:alpine
COPY --from=build-step /app/dist/gauss-method.angular /usr/share/nginx/html