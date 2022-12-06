-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Dec 03, 2022 at 07:45 PM
-- Server version: 10.4.25-MariaDB
-- PHP Version: 8.1.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: cybercafe
--
CREATE DATABASE IF NOT EXISTS cybercafe DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE cybercafe;

-- --------------------------------------------------------

--
-- Table structure for table tblbooking
--

DROP TABLE IF EXISTS tblbooking;
CREATE TABLE IF NOT EXISTS tblbooking (
  BookingId int(11) NOT NULL AUTO_INCREMENT,
  CustId int(11) NOT NULL,
  Service varchar(150) NOT NULL,
  TotHours int(11) NOT NULL,
  TotAmt decimal(10,2) DEFAULT 0.00,
  billstatus varchar(50) DEFAULT 'NOT PAID',
  PCNo int(11) DEFAULT NULL,
  BookingDate date DEFAULT NULL,
  Amt decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (BookingId),
  KEY fk1 (CustId)
);
 
--
-- Table structure for table tblcustomers
--

DROP TABLE IF EXISTS tblcustomers;
CREATE TABLE IF NOT EXISTS tblcustomers (
  id int(11) NOT NULL AUTO_INCREMENT,
  FullName varchar(120) DEFAULT NULL,
  Username varchar(120) DEFAULT NULL,
  UserEmail varchar(200) DEFAULT NULL,
  Password varchar(250) DEFAULT NULL,
  RegDate timestamp NULL DEFAULT current_timestamp(),
  Role varchar(50) DEFAULT NULL,
  PRIMARY KEY (id)
);
 
--
-- Table structure for table tblservices
--

DROP TABLE IF EXISTS tblservices;
CREATE TABLE IF NOT EXISTS tblservices (
  id int(11) NOT NULL AUTO_INCREMENT,
  ServiceName varchar(250) DEFAULT NULL,
  Description varchar(250) DEFAULT NULL,
  Rate varchar(10) DEFAULT NULL,
  Image varchar(150) DEFAULT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table tblservices
--

INSERT INTO tblservices (id, ServiceName, Description, Rate, Image) VALUES
(1, 'Game Playing', 'Hourly 10$ Game Playing', '10', '/Images/1.jpg'),
(2, 'Game Playing', 'Hourly 5$  Web Surfing', '5', '/Images/2.jpg'),
(3, 'Game + Web Surfing', 'Hourly 12$ Game + Web Surfing', '12', '/Images/4.jpg');

-- --------------------------------------------------------

--
-- Table structure for table tblsystem
--

DROP TABLE IF EXISTS tblsystem;
CREATE TABLE IF NOT EXISTS tblsystem (
  id int(11) NOT NULL AUTO_INCREMENT,
  SystemNo varchar(120) DEFAULT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table tblsystem
--

INSERT INTO tblsystem (id, SystemNo) VALUES
(1, '1'),
(2, '2'),
(3, '3'),
(4, '4'),
(5, '5');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
