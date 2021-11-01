# secretsanta-lottery
This simple console application assigns a secret santa to each participant and sends out an email with the assignment information.

## Build

`` dotnet build .\SecretSanta.sln``

## Test

``dotnet test .\SecretSanta.sln``

## Run

### Configuration

The SMTP settings for sending out emails are configured via `appsettings.json`

```
"Smtp": {
        "Host": "smtp.gmail.com",
        "Port": 587
    }
```

There are also some secret settings which should be configured using safe storage (see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows):
```
dotnet user-secrets set "Smtp:User" "johndoe@gmail.com"
dotnet user-secrets set "Smtp:Password" "secret-app-key"
dotnet user-secrets set "Smtp:Sender" "johndoe@gmail.com"
```

### Execution

```
cd .\SecretSanta.Console\
dotnet run --file .\santas.csv --launch-profile Development
```

The path to the csv file containing the santas is passed in the `--file` option. The csv file has the name of the santa in the first column and the email in the second column (without headers).