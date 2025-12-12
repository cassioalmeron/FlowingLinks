# ========================================
# Stage 1: Build the React application
# ========================================
FROM node:20-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files for dependency installation
COPY package*.json ./

# Install dependencies with clean install for reproducible builds
RUN npm ci

# Copy source code
COPY . .

# CRITICAL: Place ARG/ENV AFTER source copy for optimal caching
# This prevents invalidating npm ci cache when environment variables change
ARG VITE_API_URL=http://localhost:5000
ARG VITE_APP_NAME=Flowing Links

# Set environment variables for Vite build
ENV VITE_API_URL=${VITE_API_URL}
ENV VITE_APP_NAME=${VITE_APP_NAME}

# Build the application
RUN npm run build

# ========================================
# Stage 2: Serve with Nginx
# ========================================
FROM nginx:alpine

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built assets from builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
