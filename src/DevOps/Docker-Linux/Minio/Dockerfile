FROM quay.io/minio/minio

COPY DevOps/Docker-Linux/Minio/certs /etc/minio/certs

ENV MINIO_HTTP_KMS_KEY_FILE=/etc/minio/certs/minio.key
ENV MINIO_HTTP_KMS_CERT_FILE=/etc/minio/certs/minio.crt
