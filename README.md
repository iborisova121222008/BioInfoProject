# Стартиране на проекта

## 1. Ако имате проекта от CD/USB

1. Копирайте цялата папка на проекта на компютъра.

2. Инсталирайте нужния софтуер:
   - .NET 8 SDK
   - Visual Studio или VS Code по желание

3. Отворете терминал в папката на проекта.

4. Възстановете пакетите:

```powershell
dotnet restore
```

5. Компилирайте проекта:

```powershell
dotnet build
```

6. Стартирайте проекта:

```powershell
dotnet run --project src/BioInfoProject.Server/BioInfoProject.Server.csproj
```

7. Отворете в браузър:

```text
http://localhost:5293
```

## 2. Ако използвате GitHub

1. Инсталирайте нужния софтуер:
   - .NET 8 SDK
   - Git
   - Visual Studio или VS Code по желание

2. Клонирайте хранилището:

```powershell
git clone <адрес-на-хранилището>
```

3. Влезте в папката на проекта:

```powershell
cd BioInfoProject
```

4. Възстановете пакетите:

```powershell
dotnet restore
```

5. Компилирайте проекта:

```powershell
dotnet build
```

6. Стартирайте проекта:

```powershell
dotnet run --project src/BioInfoProject.Server/BioInfoProject.Server.csproj
```

7. Отворете в браузър:

```text
http://localhost:5293
```
