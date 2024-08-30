CREATE TABLE Peliculas (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100) NOT NULL,
    Duracion INT NOT NULL,
    Genero NVARCHAR(50) NULL,
    Clasificacion NVARCHAR(10) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Poster VARBINARY(MAX) NULL
);