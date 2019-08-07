SHELL := /bin/bash

include config.env
export

DC := docker-compose -f docker-compose.yml -f docker-compose.custom.yml
DC_RUN := ${DC} run --rm

.PHONY: confirmation
confirmation:
	@echo -n 'Are you sure? [y|N] ' && read ans && [ $$ans == y ]

.PHONY: cmd
cmd:
	@echo 'Docker-Compose command:'
	@echo '${DC}'

.PHONY: install
install:
	${DC_RUN} db-client mysql -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} < sql/database-create.sql

.PHONY: mysql
mysql:
	${DC_RUN} db-client mysql -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE}

.PHONY: mysql-cmd
mysql-cmd:
	@echo '${DC_RUN} db-client mysql -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE}'

.PHONY: dump
dump:
	${DC_RUN} db-client mysqldump -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} --extended-insert=FALSE > dump.sql
	@echo 'Database exported to dump.sql.'

.PHONY: import-db
import-db: confirmation
	test -f dump.sql
	@echo 'Replacing database with contents from file dump.sql...'
	${DC_RUN} db-client mysqldump -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} > previous_dump.sql
	${DC_RUN} db-client mysql -h db -u ${MYSQL_USER} -p${MYSQL_PASSWORD} ${MYSQL_DATABASE} < dump.sql
	@echo 'Database replaced. Previous contents in previous_dump.sql.'

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
	${DC_RUN} donation-manager bash pre_launcher.sh

.PHONY: stop
stop:
	${DC} stop

.PHONY: rm
rm rmc: stop
	${DC} rm -f

.PHONY: logs
logs:
	docker logs -f $(shell ${DC} ps -q web)
