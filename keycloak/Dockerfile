# Documentation:
#  https://www.keycloak.org/server/containers

ARG KEYCLOAK_VERSION=latest

FROM quay.io/keycloak/keycloak:$KEYCLOAK_VERSION AS builder

# Enable health and metrics support
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true

# Configure a database vendor
ENV KC_DB=postgres
ENV KC_DB_URL=jdbc:postgresql://postgres/keycloak
ENV KC_DB_USERNAME=keycloak
ENV KC_DB_PASSWORD=password
ENV KC_DB_SCHEMA=public

# Add the provider JAR file to the providers directory
# ADD --chown=keycloak:keycloak --chmod=644 scim-for-keycloak-kc-26.3-3.5.4-free.jar /opt/keycloak/providers/scim-for-keycloak-kc-26.3-3.5.4-free.jar

WORKDIR /opt/keycloak
# for demonstration purposes only, please make sure to use proper certificates in production instead
#RUN keytool -genkeypair -storepass password -storetype PKCS12 -keyalg RSA -keysize 2048 -dname "CN=server" -alias server -ext "SAN:c=DNS:localhost,IP:127.0.0.1" -keystore conf/server.keystore
RUN /opt/keycloak/bin/kc.sh build --features="docker,dpop,token-exchange,recovery-codes,passkeys"

FROM quay.io/keycloak/keycloak:$KEYCLOAK_VERSION

LABEL image.version=$KEYCLOAK_VERSION

COPY --from=builder /opt/keycloak/ /opt/keycloak/

# If any themes
# COPY themes/<nice-themes> /opt/keycloak/themes/<nice-themes>

# https://github.com/keycloak/keycloak/issues/19185#issuecomment-1480763024
USER root
RUN sed -i '/disabledAlgorithms/ s/ SHA1,//' /etc/crypto-policies/back-ends/java.config
USER keycloak

RUN /opt/keycloak/bin/kc.sh show-config

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]