-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 27-09-2025 a las 04:19:12
-- Versión del servidor: 10.4.28-MariaDB
-- Versión de PHP: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--
CREATE DATABASE IF NOT EXISTS `inmobiliaria` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `inmobiliaria`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `auditoriacontratos`
--

CREATE TABLE `auditoriacontratos` (
  `IdRegistro` int(11) NOT NULL,
  `IdContrato` int(11) NOT NULL,
  `IdUsuarioCreador` int(11) NOT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `IdUsuarioFinalizador` int(11) DEFAULT NULL,
  `FechaFinalizacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `auditoriacontratos`
--

INSERT INTO `auditoriacontratos` (`IdRegistro`, `IdContrato`, `IdUsuarioCreador`, `FechaCreacion`, `IdUsuarioFinalizador`, `FechaFinalizacion`) VALUES
(1, 5, 4, '2025-09-26 23:15:33', 1, '2025-09-26 23:18:23'),
(2, 6, 4, '2025-09-26 23:16:06', NULL, NULL),
(3, 7, 2, '2025-09-26 23:17:11', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `auditoriapagos`
--

CREATE TABLE `auditoriapagos` (
  `IdRegistro` int(11) NOT NULL,
  `IdPago` int(11) NOT NULL,
  `IdUsuarioCreador` int(11) NOT NULL,
  `FechaCreacion` datetime NOT NULL DEFAULT current_timestamp(),
  `IdUsuarioAnulador` int(11) DEFAULT NULL,
  `FechaAnulacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `auditoriapagos`
--

INSERT INTO `auditoriapagos` (`IdRegistro`, `IdPago`, `IdUsuarioCreador`, `FechaCreacion`, `IdUsuarioAnulador`, `FechaAnulacion`) VALUES
(1, 3, 2, '2025-09-26 23:17:30', 1, '2025-09-26 23:18:19'),
(2, 4, 1, '2025-09-26 23:18:04', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `idContrato` int(11) NOT NULL,
  `idInmueble` int(11) NOT NULL,
  `idInquilino` int(11) NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `valorMensual` double NOT NULL,
  `vigente` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`idContrato`, `idInmueble`, `idInquilino`, `fechaInicio`, `fechaFin`, `valorMensual`, `vigente`) VALUES
(5, 2, 1, '2025-09-26', '2025-12-26', 50000, 0),
(6, 4, 1, '2025-09-30', '2025-10-30', 9999, 0),
(7, 3, 5, '2025-09-22', '2025-11-30', 789465, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `idInmueble` int(11) NOT NULL,
  `direccion` varchar(50) NOT NULL,
  `tipo` varchar(50) NOT NULL,
  `uso` varchar(20) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `latitud` decimal(9,6) DEFAULT NULL,
  `longitud` decimal(9,6) DEFAULT NULL,
  `idPropietario` int(11) NOT NULL,
  `disponible` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`idInmueble`, `direccion`, `tipo`, `uso`, `ambientes`, `latitud`, `longitud`, `idPropietario`, `disponible`) VALUES
(2, 'av suerte en esto 345', 'CASA', 'RESIENCIAL', 2, 999.999999, 999.999999, 7, 1),
(3, 'duarte quiroz 123', 'CASA', 'RESIENCIAL', 1, 999.999999, 999.999999, 7, 1),
(4, 'bv san juan', 'CASA', 'RESIENCIAL', 3, 999.999999, 999.999999, 8, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `idInquilino` int(11) NOT NULL,
  `documento` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `telefono` varchar(50) NOT NULL,
  `mail` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`idInquilino`, `documento`, `nombre`, `apellido`, `telefono`, `mail`) VALUES
(1, 12345678, 'Juan', 'Pérez', '1234-5678', 'juan.perez@mail.com'),
(2, 87654321, 'Marita', 'González', '9876-5432', 'marita.gonzalez@mail.com'),
(5, 123456789, 'Celeste', 'Sosa', '2664678988', 'cele@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `idPago` int(11) NOT NULL,
  `idContrato` int(11) NOT NULL,
  `concepto` varchar(50) NOT NULL,
  `importe` double NOT NULL,
  `fecha` datetime NOT NULL,
  `anulado` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`idPago`, `idContrato`, `concepto`, `importe`, `fecha`, `anulado`) VALUES
(3, 6, 'pago mes 1', 8000, '2025-09-26 00:00:00', 1),
(4, 7, 'adelanto', 123123, '2025-09-23 00:00:00', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `idPropietario` int(11) NOT NULL,
  `documento` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `telefono` varchar(50) NOT NULL,
  `mail` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`idPropietario`, `documento`, `nombre`, `apellido`, `telefono`, `mail`) VALUES
(7, 22334455, 'Laura', 'Fernández', '4111-2233', 'laura.fernandez@email.com'),
(8, 33445566, 'Pedro', 'Sánchez', '4222-3344', 'pedro.sanchez@email.com'),
(9, 44556677, 'Ana', 'Torres', '4333-4455', 'ana.torres@email.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `idUsuario` int(11) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(50) NOT NULL,
  `nombre` varchar(50) DEFAULT NULL,
  `rol` varchar(20) DEFAULT NULL,
  `activo` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`idUsuario`, `email`, `password`, `nombre`, `rol`, `activo`) VALUES
(1, 'admin@correo.com', '12345678', 'Admin', 'Administrador', 1),
(2, 'empleado@correo.com', '123456', 'Empleado 1', 'Empleado', 1),
(4, 'juliam@gmail.com', '1234', 'Julia Murcia', 'Empleado', 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `auditoriacontratos`
--
ALTER TABLE `auditoriacontratos`
  ADD PRIMARY KEY (`IdRegistro`),
  ADD KEY `IdContrato` (`IdContrato`),
  ADD KEY `IdUsuarioCreador` (`IdUsuarioCreador`),
  ADD KEY `IdUsuarioFinalizador` (`IdUsuarioFinalizador`);

--
-- Indices de la tabla `auditoriapagos`
--
ALTER TABLE `auditoriapagos`
  ADD PRIMARY KEY (`IdRegistro`),
  ADD KEY `IdPago` (`IdPago`),
  ADD KEY `IdUsuarioCreador` (`IdUsuarioCreador`),
  ADD KEY `IdUsuarioAnulador` (`IdUsuarioAnulador`);

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`idContrato`),
  ADD KEY `idInmueble` (`idInmueble`),
  ADD KEY `idInquilino` (`idInquilino`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`idInmueble`),
  ADD KEY `idPropietario` (`idPropietario`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`idInquilino`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`idPago`),
  ADD KEY `idContrato` (`idContrato`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`idPropietario`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`idUsuario`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `auditoriacontratos`
--
ALTER TABLE `auditoriacontratos`
  MODIFY `IdRegistro` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `auditoriapagos`
--
ALTER TABLE `auditoriapagos`
  MODIFY `IdRegistro` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `idContrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `idInmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `idInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `idPago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `idPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `idUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `auditoriacontratos`
--
ALTER TABLE `auditoriacontratos`
  ADD CONSTRAINT `auditoriacontratos_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contratos` (`idContrato`),
  ADD CONSTRAINT `auditoriacontratos_ibfk_2` FOREIGN KEY (`IdUsuarioCreador`) REFERENCES `usuarios` (`idUsuario`),
  ADD CONSTRAINT `auditoriacontratos_ibfk_3` FOREIGN KEY (`IdUsuarioFinalizador`) REFERENCES `usuarios` (`idUsuario`);

--
-- Filtros para la tabla `auditoriapagos`
--
ALTER TABLE `auditoriapagos`
  ADD CONSTRAINT `auditoriapagos_ibfk_1` FOREIGN KEY (`IdPago`) REFERENCES `pagos` (`idPago`),
  ADD CONSTRAINT `auditoriapagos_ibfk_2` FOREIGN KEY (`IdUsuarioCreador`) REFERENCES `usuarios` (`idUsuario`),
  ADD CONSTRAINT `auditoriapagos_ibfk_3` FOREIGN KEY (`IdUsuarioAnulador`) REFERENCES `usuarios` (`idUsuario`);

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`idInmueble`) REFERENCES `inmuebles` (`idInmueble`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`idInquilino`) REFERENCES `inquilinos` (`idInquilino`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`idPropietario`) REFERENCES `propietarios` (`idPropietario`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`idContrato`) REFERENCES `contratos` (`idContrato`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
