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

CREATE TABLE [Payments] (
    [Id] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(10) NOT NULL,
    [PaymentMethod] nvarchar(50) NOT NULL,
    [Provider] nvarchar(50) NULL,
    [TransactionId] nvarchar(100) NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK__Payments__3214EC075A0B52BC] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PaymentTransactions] (
    [Id] uniqueidentifier NOT NULL,
    [PaymentId] uniqueidentifier NOT NULL,
    [ProviderTransactionId] nvarchar(100) NULL,
    [Status] nvarchar(30) NULL,
    [RawResponse] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK__PaymentT__3214EC075ED04D2B] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentTransactions_Payments] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id])
);
GO

CREATE TABLE [Refunds] (
    [Id] uniqueidentifier NOT NULL,
    [PaymentId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [ProviderRefundId] nvarchar(100) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK__Refunds__3214EC072E1FDF02] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Refunds_Payments] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id])
);
GO

CREATE INDEX [IX_PaymentTransactions_PaymentId] ON [PaymentTransactions] ([PaymentId]);
GO

CREATE INDEX [IX_Refunds_PaymentId] ON [Refunds] ([PaymentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260825174352_v1', N'8.0.22');
GO

COMMIT;
GO

