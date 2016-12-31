CREATE TABLE People		
(
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    TargetPerson INT Null,
	TargetedBy INT Null,
	Name NVarchar(50) unique Not Null,
    FOREIGN KEY (TargetPerson) REFERENCES People (Id),
	FOREIGN KEY (TargetedBy) REFERENCES People (Id),
);

INSERT INTO People
Values(NULL, NULL, 'A'),
(NULL, NULL, 'B'),
(NULL, NULL, 'C'),
(NULL, NULL, 'D'),
(NULL, NULL, 'E'),
(NULL, NULL, 'F'),
(NULL, NULL, 'G'),
(NULL, NULL, 'H');

UPDATE dbo.People SET TargetPerson = null, TargetedBy = null
Delete from dbo.People;
