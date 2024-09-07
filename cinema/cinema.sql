CREATE TABLE Peliculas (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100) NOT NULL,
    Duracion INT NOT NULL,
    Genero NVARCHAR(50) NULL,
    Clasificacion NVARCHAR(10) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Poster VARBINARY(MAX) NULL
);

CREATE TABLE Menu (
    Id INT PRIMARY KEY IDENTITY(1,1),   
    Foto VARBINARY(MAX),                    
    Articulo NVARCHAR(100) NOT NULL,   
    Descripcion NVARCHAR(255),             
    Precio DECIMAL(10, 2) NOT NULL,
	Categoria VARCHAR(50) NOT NULL,
);

CREATE TABLE Salas (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL,
    TipoSala NVARCHAR(50) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL
)

CREATE TABLE Asientos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fila CHAR(1) NOT NULL,
    Columna INT NOT NULL,
    Ocupado BIT NOT NULL,
    SalaId INT NULL
)

DECLARE @fila CHAR(1)
DECLARE @columna INT
DECLARE @ocupado BIT = 0

-- Recorremos las filas de 'A' a 'J'
SET @fila = 'A'
WHILE @fila <= 'J'
BEGIN
    -- Insertamos 15 asientos por fila
    SET @columna = 1
    WHILE @columna <= 15
    BEGIN
        INSERT INTO Asientos (Fila, Columna, Ocupado)
        VALUES (@fila, @columna, @ocupado)
        SET @columna = @columna + 1
    END
    -- Pasamos a la siguiente fila
    SET @fila = CHAR(ASCII(@fila) + 1)
END

ALTER TABLE Asientos
ADD CONSTRAINT FK_Asientos_Salas FOREIGN KEY (SalaId) REFERENCES Salas(Id);