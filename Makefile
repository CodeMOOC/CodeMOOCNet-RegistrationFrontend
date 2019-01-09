SHELL := /bin/bash

DC := docker-compose -f docker/docker-compose.yml --project-name codemooc-reg
DC_RUN := ${DC} run --rm

include docker/config.env
export

.PHONY: install
install:
	${DC_RUN} db-client -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} < sql/database-create.sql

.PHONY: mysql
mysql: up
	${DC_RUN} db-client -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE}

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
	${DC} rm -sf web
	${DC} build web
	${DC} up -d

.PHONY: process-donations
process-donations:
	${DC} build donation-manager
	${DC_RUN} donation-manager

.PHONY: stop
stop:
	${DC} stop

.PHONY: rm
rm rmc: stop
	${DC} rm -f
