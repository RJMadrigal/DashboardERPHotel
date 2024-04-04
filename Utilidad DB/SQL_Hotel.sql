CREATE DATABASE db_adminHotel;


USE db_adminHotel;

CREATE TABLE usuario(
usuarioID int primary key,
nombreUsuario varchar(30),
contraseña varchar(100),
estado bit,
rol varchar(20)
);



CREATE TABLE bitacora (
sesionID INT PRIMARY KEY IDENTITY,
usuarioID INT null,
fecha DATE,
horaEntrada TIME,
horaSalida TIME NULL,
comentario VARCHAR(50) NULL,
FOREIGN KEY (usuarioID) REFERENCES usuario(usuarioID)
);


CREATE TABLE operaciones(
operacionID INT PRIMARY KEY IDENTITY,
tipo_operacion varchar(20),
sesionID INT,
FOREIGN KEY (sesionID) REFERENCES bitacora(sesionID)
);

CREATE TABLE pedidos(
pedidoID INT PRIMARY KEY IDENTITY,
usuarioID INT,
inventarioID INT null,
producto VARCHAR(30),
detalle VARCHAR(50),
cantidad INT,
seguimiento Varchar(10) null,
estado BIT default 1,
FOREIGN KEY (inventarioID) REFERENCES inventario(inventarioID),
FOREIGN KEY (usuarioID) REFERENCES usuario(usuarioID)
);


CREATE TABLE inventario(
inventarioID INT PRIMARY KEY IDENTITY,
producto VARCHAR(30),
cantidadDisponible INT
);



CREATE TABLE habitacion(
habitacionID INT PRIMARY KEY IDENTITY,
tipo VARCHAR(30),
detalle varchar (50),
capacidad INT,
idEstadoHabitacion int,
estado bit default 1,
precioXpersona INT,
FOREIGN KEY (idEstadoHabitacion) REFERENCES estado_habitacion(idEstadoHabitacion)
);



CREATE TABLE  estado_habitacion(
idEstadoHabitacion int primary key,
descripcion varchar(50),
estado bit default 1,
)

CREATE TABLE reservacion(
reservacionID INT PRIMARY KEY,
habitacionID INT,
usuarioID INT,
fechaEntrada DATETIME DEFAULT getdate(),
fechaSalida DATE,
cantidadHuespedes INT,
nombreCliente VARCHAR(50),
FOREIGN KEY (usuarioID) REFERENCES usuario(usuarioID),
FOREIGN KEY (habitacionID) REFERENCES habitacion(habitacionID)
);


CREATE TABLE pago (
    pagoID INT PRIMARY KEY IDENTITY,
    usuarioID INT,
    reservacionID INT,
    fechaPago DATETIME DEFAULT getdate(),
    FOREIGN KEY (usuarioID) REFERENCES Usuario(usuarioID),
    FOREIGN KEY (reservacionID) REFERENCES Reservacion(reservacionID)
);

CREATE TABLE factura (
    facturaID INT PRIMARY KEY IDENTITY,
    pagoID INT,
	detalle varchar(100),
    montoTotal FLOAT,
	iv FLOAT,
	montoPagado FLOAT,
    montoVuelto FLOAT null,
    observaciones VARCHAR(30),
    fechaFactura DATETIME DEFAULT getdate(),
	MontoDolares FLOAT NULL,
    MontoColones FLOAT NULL,
    MontoTarjeta FLOAT NULL,
    FOREIGN KEY (pagoID) REFERENCES pago(pagoID)

);




/*PROCEDIMIENTOS ALMACENADOS*/

CREATE PROCEDURE RegistrarPagoYFactura
    @usuarioID INT,
    @reservacionID INT,
    @detalle VARCHAR(100),
    @montoTotal FLOAT,
    @iv FLOAT, 
    @montoPagado FLOAT,
    @montoVuelto FLOAT = NULL,
    @observaciones VARCHAR(30),
    @MontoDolares FLOAT = NULL,
    @MontoColones FLOAT = NULL,
    @MontoTarjeta FLOAT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Variable para almacenar el ID del pago
    DECLARE @pagoID INT;

    BEGIN TRY
        -- Inicio de la transacción
        BEGIN TRANSACTION;

        -- Insertar el registro en la tabla pago
        INSERT INTO pago (usuarioID, reservacionID, fechaPago)
        VALUES (@usuarioID, @reservacionID, GETDATE());

        -- Obtener el ID del pago recién insertado
        SET @pagoID = SCOPE_IDENTITY();

        -- Insertar el registro en la tabla factura asociado al pago
        INSERT INTO factura (pagoID, detalle, montoTotal, iv, montoPagado, montoVuelto, observaciones, fechaFactura, MontoDolares, MontoColones, MontoTarjeta)
        VALUES (@pagoID, @detalle, @montoTotal, @iv, @montoPagado, @montoVuelto, @observaciones, GETDATE(), @MontoDolares, @MontoColones, @MontoTarjeta);
        
        -- Actualizar el estado de la habitación
        UPDATE habitacion
        SET idEstadoHabitacion = 3
        FROM habitacion
        INNER JOIN reservacion ON habitacion.habitacionID = reservacion.habitacionID
        WHERE reservacion.reservacionID = @reservacionID;

        -- Confirmar la transacción
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Algo salió mal, rollback de la transacción
        IF @@TRANCOUNT > 0
            ROLLBACK;

        -- Propagar el error
        THROW;
    END CATCH;
END;



/*Procedicimiento para registrar reservacion */
CREATE PROCEDURE RegistrarReservacion
    @habitacionID INT,
    @usuarioID INT,
    @fechaEntrada DATETIME,
    @fechaSalida DATE,
    @cantidadHuespedes INT,
    @nombreCliente VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Generar un reservacionID aleatorio de 3 dígitos
    DECLARE @reservacionID INT;
    SET @reservacionID = ABS(CHECKSUM(NEWID())) % 1000;

    -- Insertar la nueva reservación
    INSERT INTO reservacion (reservacionID, habitacionID, usuarioID, fechaEntrada, fechaSalida, cantidadHuespedes, nombreCliente)
    VALUES (@reservacionID, @habitacionID, @usuarioID, @fechaEntrada, @fechaSalida, @cantidadHuespedes, @nombreCliente);

    -- Actualizar el estado de la habitación a "Ocupada" (idEstadoHabitacion = 2)
    UPDATE habitacion
    SET idEstadoHabitacion = 2
    WHERE habitacionID = @habitacionID;
END








--PROCEDIMIENTO PARA INSERTAR REGISTRO DE BITACORA DE SESION
CREATE PROCEDURE InsertarBitacora
    @usuarioID INT,
    @fecha DATE,
    @horaEntrada TIME
AS
BEGIN
    -- Insertar el registro en la tabla bitacora
    INSERT INTO bitacora (usuarioID, fecha, horaEntrada)
    VALUES (@usuarioID, @fecha, @horaEntrada);

    -- Devolver todos los valores del registro insertado
    SELECT SCOPE_IDENTITY() AS sesionID, @usuarioID AS usuarioID, @fecha AS fecha, @horaEntrada AS horaEntrada, NULL AS horaSalida;
END;




--PROCEDIMIENYO PARA REGISTRAR PRODUCTO EN INVENTARIO
CREATE PROCEDURE RegistrarProducto
    @producto VARCHAR(30),
    @cantidadDisponible INT,
	@Resultado bit output
AS
BEGIN
   set @Resultado= 1
	IF NOT EXISTS (SELECT * FROM inventario WHERE producto = @producto) 
    INSERT INTO inventario (producto, cantidadDisponible)
    VALUES (@producto, @cantidadDisponible);
	ELSE
	set @Resultado = 0
END;

DECLARE @Resultado bit;
EXEC RegistrarProducto @producto = 'Velas', @cantidadDisponible = 10, @Resultado = @Resultado OUTPUT;
SELECT @Resultado AS 'Resultado';

drop procedure RegistrarProducto


--PROCEDIMIENTO PARA MODIFICAR PRODUCTO EN INVENTARIO
create procedure ModificarProducto(
@inventarioID int,
@producto varchar(30),
@cantidadDisponible int,
@Resultado bit output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM inventario WHERE producto =@producto and inventarioID != @inventarioID)
		
		update inventario set 
		producto = @producto,
		cantidadDisponible = @cantidadDisponible
		
		where inventarioID = @inventarioID
	ELSE
		SET @Resultado = 0

end

/*Triggers*/


 /* Trigger para actualizar inventario al aprobar un pedido*/
CREATE TRIGGER trg_UpdateInventarios
ON pedidos
AFTER UPDATE
AS
BEGIN
    IF UPDATE(seguimiento)
    BEGIN
        DECLARE @inventarioID INT;
        DECLARE @cantidadPedido INT;

        SELECT @inventarioID = inventarioID, @cantidadPedido = cantidad
        FROM inserted
        WHERE seguimiento = 'APROBADA';

        UPDATE inventario
        SET cantidadDisponible = cantidadDisponible + @cantidadPedido
        WHERE inventarioID = @inventarioID;

        -- Actualizar el estado del pedido a 0
        UPDATE pedidos
        SET estado = 0
        WHERE pedidoID = (SELECT pedidoID FROM inserted);
    END
END;







/*Tablas creadas pero no en uso */



CREATE TABLE tipoPago (
    tipoPagoID INT PRIMARY KEY IDENTITY,
	facturaID INT,
	monedaID INT,   
	monto INT,
	FOREIGN KEY (monedaID) REFERENCES moneda(monedaID),
	FOREIGN KEY (facturaID) REFERENCES factura(facturaID)


);


CREATE TABLE moneda (
    monedaID INT PRIMARY KEY IDENTITY,
    nombreMoneda VARCHAR(30) UNIQUE,
	tipoCambio FLOAT null
);




/*Inserts */


INSERT INTO habitacion(tipo,detalle,capacidad,IdEstadoHabitacion,precioXpersona) values
('Habitacion estandar','WIFI + BAÑO + TV + CABLE',2,1,20000),
('Habitacion estandar','WIFI + BAÑO + TV + CABLE',2,1,20000),
('Habitacion estandar','WIFI + BAÑO + TV + CABLE',2,3,20000),
('Habitacion estandar','WIFI + BAÑO + TV + CABLE',2,1,20000),

('Habitacion estandar Plus','WIFI + BAÑO + TV + CABLE + JACUZZI',3,1,20000),
('Habitacion estandar Plus','WIFI + BAÑO + TV + CABLE + JACUZZI',3,1,20000),
('Habitacion estandar Plus','WIFI + BAÑO + TV + CABLE + JACUZZI',3,1,20000),
('Habitacion estandar Plus','WIFI + BAÑO + TV + CABLE + JACUZZI',3,3,20000),

('Suite','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',3,1,50000),
('Suite','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',3,1,50000),
('Suite','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',3,1,50000),

('Suite plus','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',6,1,50000),
('Suite plus','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',6,1,50000),
('Suite plus','WIFI + BAÑO + TV + CABLE + JACUZZI + MINIBAR',6,1,50000),


('Habitacion familiar','WIFI + BAÑO + TV + CABLE ',4,1,15000),
('Habitacion familiar','WIFI + BAÑO + TV + CABLE ',4,2,15000),
('Habitacion familiar','WIFI + BAÑO + TV + CABLE ',4,1,15000),
('Habitacion familiar','WIFI + BAÑO + TV + CABLE ',4,1,15000),

('Habitacion familiar plus','WIFI + BAÑO + TV + CABLE + JACUZZI',6,1,15000),
('Habitacion familiar plus','WIFI + BAÑO + TV + CABLE + JACUZZI',6,1,15000),
('Habitacion familiar plus','WIFI + BAÑO + TV + CABLE + JACUZZI',6,2,15000),
('Habitacion familiar plus','WIFI + BAÑO + TV + CABLE + JACUZZI',6,1,15000)


insert into estado_habitacion (idEstadoHabitacion,descripcion) values
(1,'DISPONIBLE'),
(2,'OCUPADO'),
(3,'LIMPIEZA')


INSERT INTO moneda (nombreMoneda,tipoCambio) VALUES ('Dolares',515);

INSERT INTO moneda (nombreMoneda,tipoCambio) VALUES ('Colones', null);



INSERT INTO inventario (producto, cantidadDisponible) VALUES
('Sábanas', 200),
('Papel Higiénico', 150),
('Cepillos de Dientes', 100),
('Pasta Dental', 120),
('Secador de Pelo', 20),
('Plancha', 20),
('Perchas', 80),
('Café', 300),
('Té', 250),
('Azúcar', 400),
('Toallas', 100),
('Almohadas', 50),
('Champú', 200),
('Jabón', 300);


/*CREAR USUARIO ADMIN*/


INSERT INTO usuario (usuarioID, nombreUsuario, contraseña, rol)
VALUES (1, 'Admin', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Admin');