CREATE TABLE Etat(
    idEtat INT NOT NULL,
    nomEtat VARCHAR(30) NOT NULL,
    CONSTRAINT CP_Etat PRIMARY KEY (idEtat)
)


CREATE TABLE Utilisateur (
    idUser INT NOT NULL,
    nomUser VARCHAR(30) NOT NULL,
    prenomUser VARCHAR(30) NOT NULL,
    CONSTRAINT CP_Utilisateur PRIMARY KEY (idUser)
)
CREATE TABLE Partie(
    idPartie INT NOT NULL,
    dateHeurePartie VARCHAR(30) NOT NULL,
    CONSTRAINT CP_Partie PRIMARY KEY (idPartie)
)


CREATE TABLE Jouer (
    idUser INT NOT NULL,
    idEtat INT NOT NULL,
    idPartie INT NOT NULL,
    listeCombine VARCHAR(200) NOT NULL,
    CONSTRAINT CP_unePartie PRIMARY KEY (idUser , idEtat),
    CONSTRAINT CE_monUser FOREIGN KEY (idUser) REFERENCES Utilisateur (idUser),
    CONSTRAINT CE_maPartie FOREIGN KEY (idPartie) REFERENCES Partie (idPartie),
CONSTRAINT CE_monEtat FOREIGN KEY (idEtat) REFERENCES Etat (idEtat)
)
