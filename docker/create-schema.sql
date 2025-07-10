--
-- PostgreSQL database dump
--
-- Dumped from database version 15.8
-- Dumped by pg_dump version 17.0
-- Started on 2025-04-06 21:41:06 CEST
SET
    statement_timeout = 0;

SET
    lock_timeout = 0;

SET
    idle_in_transaction_session_timeout = 0;

SET
    client_encoding = 'UTF8';

SET
    standard_conforming_strings = on;

SELECT
    pg_catalog.set_config ('search_path', '', false);

SET
    check_function_bodies = false;

SET
    xmloption = content;

SET
    client_min_messages = warning;

SET
    row_security = off;

CREATE USER masstransit
WITH
    SUPERUSER PASSWORD 'masstransit';

CREATE USER doadmin;

--
-- TOC entry 6602 (class 1262 OID 232110)
-- Name: masstransit; Type: DATABASE; Schema: -; Owner: masstransit
--
CREATE DATABASE "masstransit"
WITH
    TEMPLATE = template0 ENCODING = 'UTF8';

ALTER DATABASE "masstransit" OWNER TO masstransit;

\connect -reuse-previous=on "dbname='masstransit'"

