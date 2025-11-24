FROM node:22-alpine as build
WORKDIR /app

RUN npm install -g @angular/cli

COPY ./package.json .
RUN npm install

COPY . .

RUN npm run build --prod

FROM nginx AS runtime
COPY --from=build /app/dist/Client/browser /usr/share/nginx/html
