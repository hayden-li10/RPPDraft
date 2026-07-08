Run the solution:

1. In root, run this command: cd /src/RPP.Presentation/ElsaServer

2. In root, run this command: dotnet run --urls https://localhost:5001

3. Open https://localhost:5001 in browser
  
4. username: admin   password: password


Deploy distributed clustering environment:

1. In src/RPP.Presentation/ElsaServer/Program.cs, uncomment the Distributed Clustering Configuration Region and comment out the app.UseHttpsRedirection();

2. In root, run this command: dotnet publish src/RPP.Presentation/ElsaServer/ElsaServer.csproj -c Release -o ./publish /p:UseAppHost=false

3. While docker desktop is running, run this command: docker compose up --build --force-recreate -d

4. Open http://localhost:8180 in browser
