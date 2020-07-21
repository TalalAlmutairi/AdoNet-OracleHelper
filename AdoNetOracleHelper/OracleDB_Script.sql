
CREATE TABLE EMP_TS.EMPLOYEES
(
  EMPID      INTEGER,
  FIRSTNAME  VARCHAR2(20 BYTE),
  LASTNAME   VARCHAR2(20 BYTE),
  AGE        INTEGER,
  COUNTRYID  INTEGER
)
TABLESPACE EMP_TS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX EMP_TS.EMPLOYEES_PK ON EMP_TS.EMPLOYEES
(EMPID)
LOGGING
TABLESPACE TS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE EMP_TS.EMPLOYEES ADD (
  CONSTRAINT EMPLOYEES_PK
 PRIMARY KEY
 (EMPID));
 
 

 CREATE TABLE EMP_TS.COUNTRY
(
  COUNTRYID    INTEGER,
  COUNTRYDESC  VARCHAR2(20 BYTE)
)
TABLESPACE EMP_TS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX EMP_TS.COUNTRY_PK ON EMP_TS.COUNTRY
(COUNTRYID)
LOGGING
TABLESPACE EMP_TS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;




Insert into COUNTRY
   (COUNTRYID, COUNTRYDESC)
 Values
   (1, 'Saudi Arabia');
Insert into COUNTRY
   (COUNTRYID, COUNTRYDESC)
 Values
   (2, 'Kuwait');
Insert into COUNTRY
   (COUNTRYID, COUNTRYDESC)
 Values
   (3, 'United Arab Emirates');


CREATE SEQUENCE EMP_TS.SEQ_EMPID
START WITH 1
INCREMENT BY 1
MINVALUE 0
MAXVALUE 9999999
NOCACHE 
NOCYCLE 
NOORDER 


CREATE OR REPLACE PACKAGE EMP_TS.GET_MULTIPLE_TABLES AS
TYPE refcur IS REF CURSOR;
PROCEDURE GET_TABLES(CurEmp OUT GET_MULTIPLE_TABLES.refcur);
END GET_MULTIPLE_TABLES;

CREATE OR REPLACE PACKAGE BODY EMP_TS.GET_MULTIPLE_TABLES IS 
PROCEDURE GET_TABLES(CurEmp OUT GET_MULTIPLE_TABLES.refcur,CurCountry OUT GET_MULTIPLE_TABLES.refcur) IS
BEGIN
OPEN CurEmp FOR SELECT * FROM Employees;
OPEN CurCountry FOR SELECT * FROM Country;
END GET_TABLES;
END;
/


CREATE OR REPLACE PACKAGE EMP_TS.GET_ALL_EMPLOYEES AS
TYPE refcur IS REF CURSOR;
PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur);
END GET_ALL_EMPLOYEES;


CREATE OR REPLACE PACKAGE BODY EMP_TS.GET_ALL_EMPLOYEES IS 
PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur) IS
BEGIN
OPEN CurEmp FOR SELECT * FROM Employees;
END GET_EMPLOYEES_INFO;
END;
/