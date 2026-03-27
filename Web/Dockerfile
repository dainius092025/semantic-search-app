FROM node:22-alpine AS build
WORKDIR /app

# Suppress npm update notices
ENV NPM_CONFIG_UPDATE_NOTIFIER=false

# Copy package files first to leverage Docker cache
COPY package*.json ./

# Clean install dependencies based on package-lock.json
# We use 'npm ci' for reproducible builds
RUN npm ci

# Copy the rest of the source code
# .dockerignore will prevent node_modules from being overwritten
COPY . .

# Build argument for the API URL
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# Run the build
RUN npm run build

# Production stage
FROM nginx:alpine
# Copy built assets from build stage
COPY --from=build /app/dist /usr/share/nginx/html
# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]