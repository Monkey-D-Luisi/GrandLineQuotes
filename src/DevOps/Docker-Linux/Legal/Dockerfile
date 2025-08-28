FROM nginx:alpine

# HTML
COPY Front/Legal/privacy_en.html /usr/share/nginx/html/privacy_en.html
COPY Front/Legal/privacy_es.html /usr/share/nginx/html/privacy_es.html
COPY Front/Legal/robots.txt      /usr/share/nginx/html/robots.txt

# Config de Nginx (en http{}): map + server
COPY DevOps/Docker-Linux/Legal/default.conf /etc/nginx/conf.d/default.conf
