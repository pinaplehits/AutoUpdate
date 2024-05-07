# AutoUpdate

AutoUpdate is a C# application designed to automate the process of updating files from an SFTP server and running specified programs. It utilizes SSH.NET for SFTP operations and Newtonsoft.Json for JSON serialization/deserialization.

## Configuration

The application reads its configuration from the [autoupdate.json](AutoUpdate/autoupdate.json)  file, which should be present in the application directory. Below is an example of the configuration file structure:

```json
{
  "user": "26ZA0suT2323Mmjk=DR6",
  "password": "660YPyGP6g8LlVNviJAaz/OE2Ag==08H",
  "ip": "194.108.117.16",
  "serverPath": {
    "root": "pub/example",
    "files": [
      {
        "name": "readme.txt",
        "usingRoot": false,
        "run": true
      },
      {
        "name": "imap-console-client.png",
        "usingRoot": true
      },
      {
        "name": "winceclient.png",
        "usingRoot": true,
        "run": true
      }
    ]
  },
  "keepConsoleOpen": false
}
```

- `user`: SFTP username encrypted for security.
- `password`: SFTP password encrypted for security.
- `ip`: IP address of the SFTP server.
- `serverPath`: Configuration related to server paths and files.
  - `root`: Root directory on the SFTP server.
  - `files`: List of files to be updated.
    - `name`: Name of the file.
    - `usingRoot`: Boolean indicating whether the file path uses the root directory. Default is true.
    - `run`: Boolean indicating whether the file should be executed after downloading.
- `keepConsoleOpen`: Boolean indicating whether the console should remain open after execution.

## Functionality

The application performs the following tasks:

1. **Download From SFTP**: Connects to the specified SFTP server, downloads files according to the configuration, and logs the process.
2. **Run Program**: Executes specified programs after downloading files.
3. **Logging**: Logs messages to the console and a log file (`autoupdate.log`).

## Running the Application

To run the application, simply execute the `AutoUpdate.exe` file. Ensure that the `autoupdate.json` file is correctly configured in the application directory.
