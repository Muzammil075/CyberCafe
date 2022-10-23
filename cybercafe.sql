CREATE DATABASE IF NOT EXISTS cybercafe;
USE cybercafe;

DROP TABLE IF EXISTS tblcustomers;
CREATE TABLE IF NOT EXISTS tblcustomers (
  id int(11) NOT NULL AUTO_INCREMENT,
  FullName varchar(120) DEFAULT NULL,
  Username varchar(120) DEFAULT NULL,
  UserEmail varchar(200) DEFAULT NULL,
  Password varchar(250) DEFAULT NULL,
  RegDate timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (id)
);
COMMIT;