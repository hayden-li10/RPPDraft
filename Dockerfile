# Targeting .NET 10.0 runtime (No SDK required since we pre-compiled)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
USER app
WORKDIR /app
EXPOSE 8080

# Force UTC timezone for clustered Quartz timers
ENV TZ=UTC

# Copy the pre-compiled files from your laptop directly into the container
COPY ./publish .

# Start the Elsa Server host
ENTRYPOINT ["dotnet", "ElsaServer.dll"]
