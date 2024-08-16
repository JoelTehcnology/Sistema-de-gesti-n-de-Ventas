CREATE DATABASE DBSISTEMA_PROJECT;
GO

-- Usar la base de datos creada
USE DBSISTEMA_PROJECT;
GO

-- Crear tablas en el orden de dependencia

CREATE TABLE ROL(
    IdRol int PRIMARY KEY IDENTITY,
    Descripcion varchar(50),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE PERMISO(
    IdPermiso int PRIMARY KEY IDENTITY,
    IdRol int REFERENCES ROL(IdRol),
    NombreMenu varchar(100),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE PROVEEDOR(
    IdProveedor int PRIMARY KEY IDENTITY,
    Documento varchar(50),
    RazonSocial varchar(50),
    Correo varchar(50),
    Telefono varchar(50),
    Estado bit,
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE CLIENTE(
    IdCliente int PRIMARY KEY IDENTITY,
    Documento varchar(50),
    NombreCompleto varchar(50),
    Correo varchar(50),
    Telefono varchar(50),
    Estado bit,
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE USUARIO(
    IdUsuario int PRIMARY KEY IDENTITY,
    Documento varchar(50),
    NombreCompleto varchar(50),
    Correo varchar(50),
    Clave varchar(50),
    IdRol int REFERENCES ROL(IdRol),
    Estado bit,
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE CATEGORIA(
    IdCategoria int PRIMARY KEY IDENTITY,
    Descripcion varchar(100),
    Estado bit,
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE PRODUCTO(
    IdProducto int PRIMARY KEY IDENTITY,
    Codigo varchar(50),
    Nombre varchar(50),
    Descripcion varchar(50),
    IdCategoria int REFERENCES CATEGORIA(IdCategoria),
    Stock int NOT NULL DEFAULT 0,
    PrecioCompra decimal(10,2) DEFAULT 0,
    PrecioVenta decimal(10,2) DEFAULT 0,
    Estado bit,
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE COMPRA(
    IdCompra int PRIMARY KEY IDENTITY,
    IdUsuario int REFERENCES USUARIO(IdUsuario),
    IdProveedor int REFERENCES PROVEEDOR(IdProveedor),
    TipoDocumento varchar(50),
    NumeroDocumento varchar(50),
    MontoTotal decimal(10,2),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE DETALLE_COMPRA(
    IdDetalleCompra int PRIMARY KEY IDENTITY,
    IdCompra int REFERENCES COMPRA(IdCompra),
    IdProducto int REFERENCES PRODUCTO(IdProducto),
    PrecioCompra decimal(10,2) DEFAULT 0,
    PrecioVenta decimal(10,2) DEFAULT 0,
    Cantidad int,
    MontoTotal decimal(10,2),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE VENTA(
    IdVenta int PRIMARY KEY IDENTITY,
    IdUsuario int REFERENCES USUARIO(IdUsuario),
    TipoDocumento varchar(50),
    NumeroDocumento varchar(50),
    DocumentoCliente varchar(50),
    NombreCliente varchar(90),
    MontoPago decimal(10,2),
    MontoCambio decimal(10,2),
    MontoTotal decimal(10,2),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE DETALLE_VENTA(
    IdDetalleVenta int PRIMARY KEY IDENTITY,
    IdVenta int REFERENCES VENTA(IdVenta),
    IdProducto int REFERENCES PRODUCTO(IdProducto),
    PrecioVenta decimal(10,2),
    Cantidad int,
    SubTotal decimal(10,2),
    FechaCreacion datetime DEFAULT GETDATE()
);
GO

CREATE TABLE NEGOCIO(
    IdNegocio int PRIMARY KEY,
    Nombre varchar(60),
    RUC varchar(60),
    Direccion varchar(60),
    Logo varbinary(max) NULL
);
GO


/*************************** CREACION DE PROCEDIMIENTOS ALMACENADOS ***************************/
/*--------------------------------------------------------------------------------------------*/
CREATE PROC SP_REGISTRARUSUARIO(
@Documento varchar (50),
@NombreCompleto varchar (60),
@Correo varchar(50),
@Clave varchar (100),
@IdRol int,
@Estado bit,
@IdUsuarioResultado int output,
@Mensaje varchar (600) output
)

as 
begin

set @IdUsuarioResultado =0
	 SET @Mensaje= ''
IF NOT EXISTS (SELECT *FROM USUARIO WHERE Documento = @Documento)
begin
     INSERT INTO USUARIO(Documento,NombreCompleto,Correo,Clave,IdRol,Estado) VALUES
	 (@Documento,@NombreCompleto,@Correo,@Clave,@IdRol,@Estado)

	 set @IdUsuarioResultado = SCOPE_IDENTITY()


end
ELSE 
    SET @Mensaje= 'YA EXISTE UN USUARIO CON ESE DOCUMENTO'

end
GO


create PROC SP_EDITARUSUARIO(
@IdUsuario int,
@Documento varchar (50),
@NombreCompleto varchar (60),
@Correo varchar(50),
@Clave varchar (100),
@IdRol int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar (600) output
)

as 
begin

set @Respuesta =0
	 SET @Mensaje= ''
IF NOT EXISTS (SELECT *FROM USUARIO WHERE Documento = @Documento and idusuario != @IdUsuario )
begin
     UPDATE USUARIO set
	 Documento = @Documento,
	 NombreCompleto= @NombreCompleto,
	 Correo= @Correo,
	 Clave= @Clave,
	 IdRol=@IdRol,
	 Estado=@Estado
	where IdUsuario= @IdUsuario

	 set @Respuesta =1


end
ELSE 
    SET @Mensaje= 'YA EXISTE UN USUARIO CON ESE DOCUMENTO'

end
GO


create PROC SP_ELIMINARUSUARIO(
@IdUsuario int,
@Respuesta bit output,
@Mensaje varchar (600) output
)

as 
begin

set @Respuesta =0
	 SET @Mensaje= ''
	DECLARE @PASOREGLA BIT = 1

	 IF EXISTS (SELECT * FROM COMPRA C
	 INNER JOIN USUARIO U ON U.IdUsuario = C.IdUsuario
	 WHERE U.IDUSUARIO =@IdUsuario )
	 
	 BEGIN
	 SET  @PASOREGLA = 0
	 SET @Respuesta =0
	 SET @Mensaje= @Mensaje +'NO SE PUEDE ELIMINAR! EL USUARIO SE ENCUANTRA RELACIONADO A UNA COMPRA\N'
 END

 	 IF EXISTS (SELECT *FROM VENTA V
	 INNER JOIN USUARIO U ON U.IdUsuario = V.IdUsuario
	 WHERE U.IDUSUARIO =@IdUsuario )
	 
	 BEGIN
	 SET  @PASOREGLA = 0
	 SET @Respuesta =0
	 SET @Mensaje= @Mensaje +'NO SE PUEDE ELIMINAR! EL USUARIO SE ENCUANTRA RELACIONADO A UNA VENTA\N'
 END
 IF (  @PASOREGLA = 1)
 BEGIN
        DELETE FROM USUARIO WHERE IdUsuario =@IdUsuario
		SET  @Respuesta =1
	END
 END

 GO
 /* ---------- PROCEDIMIENTOS PARA CATEGORIA -----------------*/

 
CREATE PROCEDURE SP_REGISTRARCATEGORIA(
@Descripcion varchar(100),
@Estado bit,
@Resultado int output,
@Mensaje varchar(600) output
)as
begin
SET @Resultado= 0
IF NOT EXISTS (SELECT *FROM CATEGORIA WHERE Descripcion = @Descripcion)
begin
 insert into CATEGORIA(Descripcion,Estado) values (@Descripcion, @Estado)
set @Resultado = SCOPE_IDENTITY()
end
ELSE
set @Mensaje = 'NO SE PUEDE REPETIR LA DESCRIPCION DE UNA CATEGORIA'

end
go


CREATE PROCEDURE SP_EDITARCATEGORIA(
@IdCategoria int,
@Descripcion varchar(100),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(600) output
)as
begin
SET @Resultado= 1
IF NOT EXISTS (SELECT *FROM CATEGORIA WHERE Descripcion = @Descripcion and IdCategoria !=@IdCategoria)

update CATEGORIA  
set Descripcion = @Descripcion,
Estado = @Estado
WHERE IdCategoria = @IdCategoria
ELSE
begin
 set @Resultado = 0
 set @Mensaje = 'NO SE PUEDE REPETIR LA DESCRIPCION DE UNA CATEGORIA'
end
end

go


CREATE PROCEDURE SP_ELIMINARCATEGORIA(
@IdCategoria int,
@Resultado bit output,
@Mensaje varchar(600) output
)
as
begin
SET @Resultado= 1
IF NOT EXISTS(
select *from CATEGORIA c inner join PRODUCTO P on p.IdCategoria = c.IdCategoria
where c.IdCategoria = @IdCategoria
)
begin

DELETE TOP(1) FROM CATEGORIA where IdCategoria= @IdCategoria
end

ELSE
begin
set @Resultado = 0
 set @Mensaje = 'LA CATEGORIA SE ENCUENTRA RELACIONADA A UN PRODUCTO'
end
end
GO

/* ---------- PROCEDIMIENTOS PARA PRODUCTO -----------------*/



CREATE PROC SP_REGISTRARPRODUCTO(
@Codigo varchar (20),
@Nombre varchar (30),
@Descripcion varchar(30),
@IdCategoria int,
@Estado bit,
@Resultado int output,
@Mensaje varchar(600) output
)as
begin
SET @Resultado= 0
IF NOT EXISTS (SELECT *FROM producto WHERE Codigo= @Codigo)
begin
INSERT INTO producto(Codigo,Nombre,Descripcion,IdCategoria,Estado) values (@Codigo,@Nombre,@Descripcion,@IdCategoria,@Estado)
set @Resultado= SCOPE_IDENTITY()
end
ELSE
SET @Mensaje= 'ERROR! YA EXISTE UN PRODUCTO CON ESE CODIGO'

END 
GO

CREATE PROC SP_MODIFICARPRODUCTO(
@IdProducto int,
@Codigo varchar(30),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Estado bit,
@Resultado bit output,
@Mensaje varchar(600) output
)
as
begin
SET @Resultado =1
IF NOT EXISTS(SELECT *FROM PRODUCTO WHERE codigo= @Codigo and IdProducto != @IdProducto)

update PRODUCTO SET
codigo= @Codigo,
Nombre= @Nombre,
Descripcion = @Descripcion,
IdCategoria= @IdCategoria,
Estado= @Estado
where IdProducto = @IdProducto
ELSE
BEGIN 
SET @Resultado = 0
SET @Mensaje = 'ERROR! YA EXISTE UN PRODUCTO CON EL MISMO CODIGO'
  END

END

GO


CREATE PROC SP_ELIMINARPRODUCTO(
@IdProducto int,
@Respuesta bit output,
@Mensaje varchar(600) output
)
as
begin
set @Respuesta =0
set @Mensaje =''
declare @pasoregla bit = 1

IF EXISTS (SELECT *FROM DETALLE_COMPRA dc
INNER JOIN PRODUCTO p ON P.IdProducto = dc.IdProducto
WHERE P.IdProducto= @IdProducto
)
BEGIN
set @pasoregla = 0
set @Respuesta= 0
set @Mensaje = @Mensaje + 'ERROR! NO SE PEUDE ELIMINAR PORQUE SE ENCUENTRA RELACIONADO A UNA COMPRA'
END

IF EXISTS( SELECT *FROM DETALLE_VENTA DV
INNER JOIN PRODUCTO P ON P.IdProducto = DV.IdProducto
WHERE P.IdProducto=@IdProducto
)
BEGIN 
set @pasoregla = 0
set @Respuesta= 0
set @Mensaje = @Mensaje + '  ERROR! NO SE PEUDE ELIMINAR PORQUE SE ENCUENTRA RELACIONADO A UNA VENTA'
END
IF(@pasoregla = 1)
begin
delete from PRODUCTO WHERE IdProducto = @IdProducto
set @Respuesta = 1

   end

end
GO

/* ---------- PROCEDIMIENTOS PARA CLIENTE -----------------*/

CREATE PROC SP_REGISTRARCLIENTES(
@Documento varchar(50),
@NombreCompleto varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@Estado bit,
@Resultado int output,
@Mensaje varchar(600) output
)as
begin
SET @Resultado = 0
DECLARE @IDPERSONA INT
IF NOT EXISTS (SELECT *FROM CLIENTE WHERE Documento = @Documento)
begin
    insert into CLIENTE(Documento,NombreCompleto,Correo,Telefono,Estado) values(
	@Documento,@NombreCompleto,@Correo,@Telefono,@Estado)

	   set @Resultado = SCOPE_IDENTITY()
	   end
	   else
	      set @Mensaje = 'ERROR! EL NUMERO DE DOCUEMNTO YA EXISTE'
end 
go


CREATE PROC SP_MODIFICARCLIENTES(
@IdCliente int,
@Documento varchar(50),
@NombreCompleto varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(600) output
)as
begin
   SET @Resultado = 1
   DECLARE @IDPERSONA INT 
   IF NOT EXISTS (SELECT *FROM CLIENTE WHERE Documento = @Documento and IdCliente != @IdCliente)
   begin
      update CLIENTE set
	  Documento = @Documento,
	  NombreCompleto = @NombreCompleto,
	  Correo = @Correo,
	  Telefono = @Telefono,
	  Estado = @Estado
	  where IdCliente = @IdCliente
	  end
	  else
	  begin
	  SET @Resultado = 0
	  SET @Mensaje = 'ERRPOR! EL NUMERO DE DOCUMENTO YA EXISTE'
end
end
GO
/* ---------- PROCEDIMIENTOS PARA PROVEEDOR -----------------*/

create PROC SP_REGISTRARPROVEEDOR(
@Documento varchar(50),
@RazonSocial varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@Estado bit,
@Resultado int output,
@Mensaje varchar(600) output
)as
begin
     SET @Resultado = 0
	 DECLARE @IDPERSONA INT 
	 IF NOT EXISTS (SELECT *FROM PROVEEDOR WHERE Documento = @Documento)
	 begin
	 insert into PROVEEDOR(Documento,RazonSocial,Correo,Telefono,Estado) values (
	 @Documento,@RazonSocial,@Correo,@Telefono,@Estado) 

	 set @Resultado = SCOPE_IDENTITY()
  end
  else   
      set @Mensaje = 'ERROR! El NUMERO DE DOCUMENTO YA EXISTE'
end
go


create PROC SP_MODIFICARPROVEEDOR(
@IdProveedor int,
@Documento varchar(50),
@RazonSocial varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(600) output
)as
begin
    SET @Resultado = 1
	DECLARE @IDPERSONA INT
	IF NOT EXISTS (SELECT *FROM PROVEEDOR WHERE Documento = @Documento and IdProveedor != @IdProveedor)
	begin
	     update PROVEEDOR set
		 Documento = @Documento,
		 RazonSocial = @RazonSocial,
		 Correo = @Correo,
		 Telefono = @Telefono,
		 Estado = @Estado 
		 where IdProveedor = @Idproveedor
  end
  else
  begin 
     SET @Resultado = 0
	 SET @Mensaje = 'ERROR! EL NUMER ODE DOCUMENTO YA EXISTE'
  end
end

go

CREATE PROCEDURE SP_ELIMINARPROVEEDOR(
@IdProveedor int,
@Resultado bit output,
@Mensaje varchar(600) output
)
as
begin
    SET @Resultado = 1
	IF NOT EXISTS (
	select *FROM PROVEEDOR p
	inner join COMPRA c on p.IdProveedor = c.IdProveedor
	WHERE p.IdProveedor = @IdProveedor
	)
	begin
	  delete top(1) FROM PROVEEDOR WHERE IdProveedor = @IdProveedor
	  end
	  ELSE
	  begin
	     SET @Resultado = 0
	     SET @Mensaje = 'ERROR! EL PROVEEDOR SE ENCUENTRA RELACIONADO A UNA COMPRA'
	 end
   end
GO


/* PROCESOS PARA REGISTRAR UNA COMPRA */


   CREATE TYPE [dbo].[EDetalle_Compra] AS TABLE(
   [IdProducto] int NULL,
   [PrecioCompra] decimal(18,2)NULL,
   [PrecioVenta] decimal(18,2)NULL,
   [Cantidad] int NULL,
   [MontoTotal] decimal(18,2)NULL

   )
   GO
   

   CREATE PROCEDURE SP_RegistrarCompra(
   @IdUsuario int,
   @IdProveedor int,
   @TipoDocumento varchar(500),
   @NumeroDocumento varchar(500),
   @MontoTotal decimal(18,2),
   @DetalleCompra [EDetalle_Compra] READONLY,
   @Resultado bit output,
   @Mensaje varchar(500) output
   )
   as 
   begin
       begin try
	      declare @idcompra int = 0
		  set @Resultado = 1
		  set @Mensaje = ''

		  begin transaction registro 

		insert into COMPRA(IdUsuario,IdProveedor,TipoDocumento,NumeroDocumento,MontoTotal)
		values(@IdUsuario,@IdProveedor,@TipoDocumento,@NumeroDocumento,@MontoTotal)

		set @idcompra = SCOPE_IDENTITY()

		insert into DETALLE_COMPRA(IdCompra,IdProducto,PrecioCompra,PrecioVenta,Cantidad,MontoTotal)
		select @idcompra,IdProducto,PrecioCompra,PrecioVenta,Cantidad,MontoTotal from @DetalleCompra

	

		UPDATE p SET  p.Stock = p.Stock + dc.Cantidad,
		p.PrecioCompra = dc.PrecioCompra,
		p.PrecioVenta = dc.PrecioVenta
		from PRODUCTO p
		inner join @DetalleCompra dc on dc.IdProducto= p.IdProducto

		  commit transaction registro

	   end try
	   begin catch

	   SET @Resultado = 0
	   SET @Mensaje = ERROR_MESSAGE()

	   rollback transaction registro

	   end catch



   end
   GO

   /* PROCESOS PARA REGISTRAR UNA VENTA */

   
  CREATE TYPE [dbo].[EDetalle_Venta] AS TABLE(
  [IdProducto] int NULL,
  [PrecioVenta] decimal(18,2) NULL,
  [Cantidad] int NULL,
  [SubTotal] decimal(18,2) NULL
  )
  GO



  CREATE PROCEDURE USP_REGISTRARVENTA(
  @IdUsuario int,
  @TipoDocumento varchar(500),
  @NumeroDocumento varchar(500),
  @DocumentoCliente varchar(500),
  @NombreCliente varchar(500),
  @MontoPago decimal(18,2),
  @MontoCambio decimal(18,2),
  @MontoTotal decimal(18,2),
  @DetalleVenta [EDetalle_Venta] READONLY,
  @Resultado bit output,
  @Mensaje varchar(500) output
  )
  as 
  begin
		begin try 

			declare @idventa int = 0
			set @Resultado = 1
			set @Mensaje = ''

			begin transaction registro

			insert into VENTA(IdUsuario,TipoDocumento,NumeroDocumento,DocumentoCliente,NombreCliente,MontoPago,MontoCambio,MontoTotal)
			values(@IdUsuario,@TipoDocumento,@NumeroDocumento,@DocumentoCliente,@NombreCliente,@MontoPago,@MontoCambio,@MontoTotal)

			set @idventa = SCOPE_IDENTITY()

			insert into DETALLE_VENTA(IdVenta,IdProducto,PrecioVenta,Cantidad,SubTotal)
			select @idventa,IdProducto,PrecioVenta,Cantidad,SubTotal from @DetalleVenta

			commit transaction registro

			end try
			begin catch
				set @Resultado = 0
				set @Mensaje = ERROR_MESSAGE()
				ROLLBACK TRANSACTION registro
			end catch
end

GO


create proc sp_ReporteCompras(
@fechainicio date,
@fechafin date,
@idproveedor int
)
AS 
begin
    select
    CONVERT(char(10),c.FechaCreacion,103) [FechaCreacion],
    c.TipoDocumento,
    c.NumeroDocumento,
    c.MontoTotal,
    u.NombreCompleto [UsuarioRegistro],
    pr.Documento [DocumentoProveedor],
    pr.RazonSocial,
    p.Codigo [CodigoProducto],
    p.Nombre [NombreProducto],
    ca.Descripcion [Categoria],
    dc.PrecioCompra,
    dc.PrecioVenta,
    dc.Cantidad,
    dc.MontoTotal [SubTotal]
    from COMPRA c
    inner join USUARIO u on u.IdUsuario = c.IdUsuario
    inner join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor
    inner join DETALLE_COMPRA dc on dc.IdCompra = c.IdCompra
    inner join PRODUCTO p on p.IdProducto = dc.IdProducto
    inner join CATEGORIA ca on ca.IdCategoria = p.IdCategoria
    where CONVERT(date, c.FechaCreacion) between @fechainicio and @fechafin
    and pr.IdProveedor = iif(@idproveedor=0, pr.IdProveedor, @IdProveedor);
end
go



CREATE PROCEDURE sp_ReporteVentas
(
    @fechainicio DATE,
    @fechafin DATE
)
AS
BEGIN
    SET DATEFORMAT dmy;  -- Configurar el formato de fecha

    SELECT
        CONVERT(char(10), v.FechaCreacion, 103) AS [FechaCreacion],
        v.TipoDocumento,
        v.NumeroDocumento,
        v.MontoTotal,
        u.NombreCompleto AS [UsuarioRegistro],
        v.DocumentoCliente,
        v.NombreCliente,
        p.Codigo AS [CodigoProducto],
        p.Nombre AS [NombreProducto],
        ca.Descripcion AS [Categoria],
        dv.PrecioVenta,
        dv.Cantidad,
        dv.SubTotal
    FROM VENTA v
    INNER JOIN USUARIO u ON u.IdUsuario = v.IdUsuario
    INNER JOIN DETALLE_VENTA dv ON dv.IdVenta = v.IdVenta
    INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
    INNER JOIN CATEGORIA ca ON ca.IdCategoria = p.IdCategoria
    WHERE CONVERT(date, v.FechaCreacion) BETWEEN @fechainicio AND @fechafin;
END
GO

/****************** INSERTAMOS REGISTROS A LAS TABLAS ******************/



INSERT INTO ROL (Descripcion)
VALUES ('ADMINISTRADOR'), ('EMPLEADO');

GO

-- Insertar Usuario
INSERT INTO USUARIO (Documento, NombreCompleto, Correo, Clave, IdRol, Estado)
VALUES ('1', 'Joel ADMIN', 'Joelj@gmail.com', '2', '1', '1');

GO

INSERT INTO USUARIO (Documento, NombreCompleto, Correo, Clave, IdRol, Estado)
VALUES ('3', 'JoelEMPLEADO', 'JoelTC@gmail.com', '4', '2', '1');

GO



INSERT INTO PERMISO (IdRol, NombreMenu)
VALUES
    (1, 'menuusuarios'),
    (1, 'menumantenedor'),
    (1, 'menuventas'),
    (1, 'menucompras'),
    (1, 'menuclientes'),
    (1, 'menuproveedores'),
    (1, 'menureportes'),
    (1, 'menuacercade'),
    (2, 'menuventas'),
    (2, 'menucompras'),
    (2, 'menuclientes'),
    (2, 'menuproveedores'),
    (2, 'menuacercade');

	GO

INSERT INTO PERMISO(IdRol,NombreMenu)values
(2,'menuventas'),
(2,'menucompras'),
(2,'menuclientes'),
(2,'menuproveedores'),
(2,'menuacercade')

GO


insert into NEGOCIO (IdNegocio,Nombre,RUC,Direccion) values (1,'JA Software','0342','BONAO,RESC DON MARIO')


