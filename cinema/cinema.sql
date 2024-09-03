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