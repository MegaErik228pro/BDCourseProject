SQLCMD -S (localdb)\MSSQLLocalDB -E -Q "BACKUP DATABASE Delivery TO DISK = 'C:\Users\Erik\Desktop\3course\Delivery\DBBackup\BackUpDelivery_Mon.bak' WITH INIT, NOFORMAT, SKIP, NOUNLOAD"