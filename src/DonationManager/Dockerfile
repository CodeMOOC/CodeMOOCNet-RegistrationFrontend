FROM composer:1.10

# Install PHP extensions
RUN apk add --no-cache freetype libpng libjpeg-turbo freetype-dev libpng-dev libjpeg-turbo-dev && \
    docker-php-ext-configure gd && \
    docker-php-ext-install gd && \
    docker-php-ext-install mysqli && \
    docker-php-ext-enable mysqli && \
    apk del --no-cache freetype-dev libpng-dev libjpeg-turbo-dev

# Compose dependencies
COPY composer.json /code/composer.json
COPY composer.lock /code/composer.lock
WORKDIR /code
RUN composer install

# Copy in rest of the code
COPY *.php /code/
COPY models /code/models
COPY *.sh /code/

CMD ["true"]
