# Use the latest LTS version of Node.js
FROM node:22.14-alpine as build
 
# Set the working directory inside the container
WORKDIR /app
 
# Copy package.json and package-lock.json
COPY ./package.json ./
 
# Install dependencies
RUN npm install
 
# Copy the rest of your application files
COPY . .

RUN npm run build

FROM nginx
COPY --from=build /app/dist /usr/share/nginx/html
 
CMD ["nginx", "-g", "daemon off;"]