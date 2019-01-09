FROM composer:1.8.0

RUN apk add --no-cache freetype libpng libjpeg-turbo freetype-dev libpng-dev libjpeg-turbo-dev && \
    docker-php-ext-configure gd && \
    docker-php-ext-install gd && \
    apk del --no-cache freetype-dev libpng-dev libjpeg-turbo-dev

COPY . /code
WORKDIR /code

CMD composer install && php launcher.php
