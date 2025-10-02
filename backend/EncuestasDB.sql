DROP DATABASE IF EXISTS Encuestas;
CREATE DATABASE IF NOT EXISTS Encuestas;
USE Encuestas;

-- Tabla de roles
CREATE TABLE roles (
    id INT UNIQUE,
    rol VARCHAR(10),
    name VARCHAR(255),
    normalized_name VARCHAR(255),
    concurrency_stamp VARCHAR(255),
    CONSTRAINT roles_PK PRIMARY KEY(id)
);

-- Tabla de usuarios
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT,
    username VARCHAR(20),
    normalized_username VARCHAR(255),
    rol INT,
    passwd VARCHAR(255),
    email VARCHAR(255) DEFAULT 'default@email.com',
    normalized_email VARCHAR(255) DEFAULT 'DEFAULT@EMAIL.COM',
    email_confirmed BIT DEFAULT 1,
    phone_number VARCHAR(20) DEFAULT NULL,
    phone_number_confirmed BIT DEFAULT 0,
    two_factor_enabled BIT DEFAULT 0,
    lockout_end DATETIME DEFAULT NULL,
    lockout_enabled BIT DEFAULT 0,
    access_failed_count INT DEFAULT 0,
    security_stamp VARCHAR(255),
    concurrency_stamp VARCHAR(255),
    CONSTRAINT id_PK PRIMARY KEY(id),
    CONSTRAINT rol_FK FOREIGN KEY(rol) REFERENCES roles(id) ON UPDATE CASCADE ON DELETE CASCADE
);

-- Tablas de Identity (NECESARIAS)
CREATE TABLE user_roles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT PK_user_roles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_user_roles_usuarios_UserId FOREIGN KEY (UserId) REFERENCES usuarios(id) ON DELETE CASCADE,
    CONSTRAINT FK_user_roles_roles_RoleId FOREIGN KEY (RoleId) REFERENCES roles(id) ON DELETE CASCADE
);

CREATE TABLE user_claims (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    CONSTRAINT FK_user_claims_usuarios_UserId FOREIGN KEY (UserId) REFERENCES usuarios(id) ON DELETE CASCADE
);

CREATE TABLE user_logins (
    LoginProvider VARCHAR(255) NOT NULL,
    ProviderKey VARCHAR(255) NOT NULL,
    ProviderDisplayName TEXT,
    UserId INT NOT NULL,
    CONSTRAINT PK_user_logins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_user_logins_usuarios_UserId FOREIGN KEY (UserId) REFERENCES usuarios(id) ON DELETE CASCADE
);

CREATE TABLE user_tokens (
    UserId INT NOT NULL,
    LoginProvider VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Value TEXT,
    CONSTRAINT PK_user_tokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_user_tokens_usuarios_UserId FOREIGN KEY (UserId) REFERENCES usuarios(id) ON DELETE CASCADE
);

CREATE TABLE role_claims (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RoleId INT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    CONSTRAINT FK_role_claims_roles_RoleId FOREIGN KEY (RoleId) REFERENCES roles(id) ON DELETE CASCADE
);

-- Resto de tus tablas de negocio
CREATE TABLE encuestas (
    id INT AUTO_INCREMENT,
    autor INT,
    titulo VARCHAR(50),
    descripcion VARCHAR(200),
    estado VARCHAR(20),
    cierra_en DATETIME,
    creado_en DATETIME,
    CONSTRAINT encuestas_PK PRIMARY KEY(id),
    CONSTRAINT encuestas_FK FOREIGN KEY(autor) REFERENCES usuarios(id)
);

CREATE TABLE preguntas (
    id INT AUTO_INCREMENT,
    encuesta_id INT,
    enunciado VARCHAR(50),
    tipo_pregunta VARCHAR(50),
    obligatorio BOOL,
    CONSTRAINT preguntas_PK PRIMARY KEY(id),
    CONSTRAINT preguntas_FK FOREIGN KEY(encuesta_id) REFERENCES encuestas(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE preguntas_opciones (
    id INT AUTO_INCREMENT,
    pregunta_id INT,
    position INT,
    Label VARCHAR(100),
    Value VARCHAR(100),
    CONSTRAINT Choices_PK PRIMARY KEY(id),
    CONSTRAINT Choices_FK FOREIGN KEY(pregunta_id) REFERENCES preguntas(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE respuestas(
    id INT AUTO_INCREMENT,
    usuario_respuesta INT,
    encuesta_id INT,
    pregunta_id INT,
    respuesta VARCHAR(100),
    respuesta_numeros FLOAT,
    fecha_respuesta DATETIME,
    seleccion_opcion_id INT,
    CONSTRAINT respuestas_PK PRIMARY KEY(id),
    CONSTRAINT respuestas_FK1 FOREIGN KEY(encuesta_id) REFERENCES encuestas(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT respuestas_FK2 FOREIGN KEY(pregunta_id) REFERENCES preguntas(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT respuestas_FK3 FOREIGN KEY(seleccion_opcion_id) REFERENCES preguntas_opciones(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT respuestas_FK4 FOREIGN KEY(usuario_respuesta) REFERENCES usuarios(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE respuestas_opciones (
    respuesta_id INT,
    opcion INT,
    CONSTRAINT respuestas_opciones_PK PRIMARY KEY(respuesta_id, opcion),
    CONSTRAINT respuestas_opciones_FK1 FOREIGN KEY(respuesta_id) REFERENCES respuestas(id),
    CONSTRAINT respuestas_opciones_FK2 FOREIGN KEY(opcion) REFERENCES preguntas_opciones(id)
);

-- Solo insertar roles
INSERT INTO roles (id, rol, name, normalized_name, concurrency_stamp) VALUES
(1, 'Admin', 'Admin', 'ADMIN', UUID()),
(2, 'User', 'User', 'USER', UUID());

-- NO INSERTAR USUARIOS - SE CREARÁN MEDIANTE LA APLICACIÓN

use encuestas;
select * from encuestas;