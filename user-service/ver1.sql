IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL DEFAULT ((newid())),
    [FullName] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NOT NULL,
    CONSTRAINT [PK__Users__3214EC07278337D2] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [UserAddresses] (
    [Id] uniqueidentifier NOT NULL DEFAULT ((newid())),
    [UserId] uniqueidentifier NOT NULL,
    [AddressLine1] nvarchar(255) NOT NULL,
    [AddressLine2] nvarchar(255) NULL,
    [City] nvarchar(100) NOT NULL,
    [District] nvarchar(100) NULL,
    [PostalCode] nvarchar(20) NULL,
    [IsDefault] bit NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime NOT NULL,
    CONSTRAINT [PK__UserAddr__3214EC07183EAB96] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserAddresses_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_UserAddresses_UserId] ON [UserAddresses] ([UserId]);
GO

CREATE UNIQUE INDEX [UQ__Users__A9D10534D338BAE6] ON [Users] ([Email]) WHERE [Email] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260104102002_v0', N'8.0.22');
GO

COMMIT;
GO

