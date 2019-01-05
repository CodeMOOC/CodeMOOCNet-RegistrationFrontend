SHELL := /bin/bash

DC := docker-compose -f docker/docker-compose.yml --project-name codemooc-reg
DC_RUN := ${DC} run --rm

include docker/config.env
export

.PHONY: confirmation
confirmation:
	@echo -n 'Are you sure? [y|N] ' && read ans && [ $$ans == y ]

.PHONY: create_volumes drop_volumes
create_volumes:
	docker volume create codemooc-reg-db
	@echo 'External volumes created'

drop_volumes: confirmation
	docker volume rm codemooc-reg-db
	@echo 'External volumes dropped'

.PHONY: install
install:
	${DC_RUN} codemooc-reg-db-client -h codemooc-reg-db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} < sql/database-create.sql

.PHONY: mysql
mysql: up
	${DC_RUN} codemooc-reg-db-client -h codemooc-reg-db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE}

.PHONY: up
up:
	${DC} up -d
	${DC} ps
	@echo
	@echo 'Service is now up'
	@echo

.PHONY: ps
ps:
	${DC} ps

.PHONY: rs
rs:
	${DC} restart

.PHONY: rebuild
rebuild:
	${DC} rm -sf codemooc-reg-web
	${DC} build codemooc-reg-web
	${DC} up -d

.PHONY: stop
stop:
	${DC} stop

.PHONY: rm
rm rmc: stop
	${DC} rm -f
