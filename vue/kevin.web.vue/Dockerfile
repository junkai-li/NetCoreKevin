# 前端镜像：Node 构建 Vue 产物 -> Nginx 托管并反向代理后端
FROM node:20-alpine AS build
WORKDIR /app

# 同源部署时留空：axios / SignalR / SSE 全部走相对路径，由 Nginx 转发到 api 容器
# 如需前端直连后端，构建时传入绝对地址，例如 VUE_APP_API_BASE_URL=http://192.168.1.10:9901
ARG VUE_APP_API_BASE_URL=
ENV VUE_APP_API_BASE_URL=$VUE_APP_API_BASE_URL
# vue-cli 打包比较吃内存，给 Node 留足堆空间
ENV NODE_OPTIONS=--max-old-space-size=4096

# npm 源，国内网络可保留默认镜像；如需官方源传入 --build-arg NPM_REGISTRY=https://registry.npmjs.org
ARG NPM_REGISTRY=https://registry.npmmirror.com

COPY package*.json ./
RUN npm config set registry $NPM_REGISTRY \
    && npm install --no-audit --no-fund

COPY . .
RUN npm run build

FROM nginx:1.27-alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
