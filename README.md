# **Multi-Backend Object Storage API**

---

## **Overview**
Simple Drive is a multi-backend object storage API built with **.NET Core 7**, designed to handle storing and retrieving blobs (binary objects) with configurable storage backends. It supports **Amazon S3**, **local file system**, **database**, and **FTP** as storage backends, and provides seamless switching between these backends via configuration.

---

## **Features**
- **Store and Retrieve Blobs**: APIs to upload and fetch blobs using a unique identifier.
- **Dynamic Storage Backend Configuration**:
  - Amazon S3
  - Local File System
  - Relational Database
  - FTP
- **Metadata Tracking**: Tracks blob size, storage type, and creation time.
- **Bearer Token Authentication**: Secure APIs using JWT-based authentication.


## **Configuration**

To configure the storage backends, update the `appsettings.json` file with the appropriate settings.

### **General Configuration**
The `Storage` section in `appsettings.json` allows you to specify which backend to use and its corresponding settings. The `Backend` field determines the active storage backend. Supported values are:
- `"S3"`: Amazon S3-compatible object storage.
- `"Local"`: Local file system.
- `"FTP"`: File Transfer Protocol.
- `"Database"`: Relational database storage.

### **Example Configuration**
```json
"Storage": {
  "Backend": "S3",
  "S3": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "BucketName": "your-bucket",
    "Region": "your-region"
  },
  "Local": {
    "StoragePath": "D:\\StorageDrive\\LocalStorage"
  },
  "FTP": {
    "ServerUrl": "ftp://ftp.example.com",
    "Username": "ftp-username",
    "Password": "ftp-password",
    "BasePath": "/"
  },
  "Database": {
  }
}
```

### **Switching the Storage Backend**

The storage backend can be switched dynamically by modifying the `Backend` value in the `Storage` section of `appsettings.json`. Each backend type requires specific configuration settings, as detailed below.

#### **Steps to Switch the Backend**

1. Open the `appsettings.json` file.
2. Locate the `"Backend"` key under the `"Storage"` section.
3. Set the value of `"Backend"` to one of the following supported backends:
   - `"S3"`: For Amazon S3 or any S3-compatible object storage.
   - `"Local"`: For storing files on the local file system.
   - `"FTP"`: For storing files on an FTP server.
   - `"Database"`: For storing files in a relational database.

4. Update the corresponding backend configuration (e.g., S3 credentials, FTP details, local storage path).

