FROM composer:1.8.0

# Install PHP extensions
RUN apk add --no-cache freetype libpng libjpeg-turbo freetype-dev libpng-dev libjpeg-turbo-dev && \
    docker-php-ext-configure gd && \
    docker-php-ext-install gd && \
    apk del --no-cache freetype-dev libpng-dev libjpeg-turbo-dev

# Compose dependencies
COPY composer.json /code/composer.json
WORKDIR /code
RUN composer install

# Copy in rest of the code
COPY *.php /code/
COPY models /code/models
COPY db /code/db

CMD php launcher.php
